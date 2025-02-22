using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using Yaoc.Messages;
using Yaoc.Messages.Snackbar;
using Yaoc.Core.Models;
using Yaoc.Core.Services;
using Yaoc.Dialogs;

namespace Yaoc.ViewModels;

public partial class ConversationsViewModel : BaseViewModel {

    public ObservableCollection<string> Models { get; } = [];
    public ObservableCollection<Conversation> Conversations { get; } = [];
    public ObservableCollection<string> AttachedFiles { get; set; } = [];

    //[ObservableProperty]
    private string _selectedModel = string.Empty;
    public string SelectedModel {
        get => _selectedModel;
        set {
            _selectedModel = value;
            SetSelectedModel();
            OnPropertyChanged();
        }
    }

    private Conversation _currentConversation;
    public Conversation CurrentConversation {
        get => _currentConversation;
        set {
            _currentConversation = value;
            InitializeConversation();
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    //[NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    private string _userMessage = string.Empty;

    private StringBuilder _botMessageStream;
    public string BotMessage {
        get => _botMessageStream.ToString();
        set {
            _botMessageStream.Append(value);
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    private bool _isWaitingForResponse = false;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    private readonly IOllamaService _ollamaService;
    private readonly IConversationsService _conversationsService;
    private readonly IDialogManager _dialogService;

    private Chat _currentChat;

    public ConversationsViewModel(
        IOllamaService ollamaService,
        IConversationsService conversationsService,
        IDialogManager dialogService) {

        _ollamaService = ollamaService;
        _conversationsService = conversationsService;
        _dialogService = dialogService;
        _botMessageStream = new StringBuilder();


        Task.WhenAll(
            Task.Run(LoadModels),
            Task.Run(LoadConversations));
    }

    protected override void OnActivated() {
        WeakReferenceMessenger.Default.Register<ConversationsViewModel, ModelDeletedMessage>(this, async (r, m) => {
            await LoadModels();
        });

        WeakReferenceMessenger.Default.Register<ConversationsViewModel, LocalModelsRefreshedMessage>(this, async (r, m) => {
            var currentModel = CurrentConversation?.Model;

            RefreshModelsList(m.Value.Select(m => m.Name));

            if (CurrentConversation != null) {
                SelectedModel = currentModel;
            }
        });
    }

    private async Task LoadModels() {
        var models = await _ollamaService.GetLocalModelNamesAsync();

        RefreshModelsList(models);
    }

    private void RefreshModelsList(IEnumerable<string> modelNames) {
        Models.Clear();

        foreach (var model in modelNames) {
            Models.Add(model);
        }
    }

    private async Task LoadConversations() {
        var conversations = await _conversationsService.LoadConversations();
        foreach (var conversation in conversations) {
            Conversations.Add(conversation);
        }
    }

    private void SetSelectedModel() {
        _currentConversation.Model = SelectedModel;
        _currentChat.Model = SelectedModel;
    }

    private void InitializeConversation() {
        _currentChat = _ollamaService.CreateChat();

        if (CurrentConversation == null) return;
        
        _currentChat.Model = _currentConversation.Model;

        if (Models.Contains(_currentConversation.Model)) {
            SelectedModel = _currentConversation.Model;
        } else {
            SelectedModel = null;
        }

        foreach (var m in CurrentConversation.Messages) {
            _currentChat.Messages.Add(m);
        }

    }

    [RelayCommand]
    private void SendMessage() {
        if (!string.IsNullOrEmpty(UserMessage)) {
            var message = UserMessage;
            UserMessage = string.Empty;

            _currentConversation.Messages.Add(new Message(ChatRole.User, message));

            _currentChat.Model = SelectedModel;

            Application.Current.Dispatcher.Invoke(async () => {
                StartWaitingForResponse();

                try {
                    await foreach (var response in _ollamaService.SendMessage(_currentChat, message)) {
                        if (IsWaitingForResponse) {
                            StopWaitingForResponse();
                        }

                        await Task.Delay(1); // Needed to prevent UI from freezing. Task.Yield() doesn't work.
                        BotMessage = response;
                    }

                    _currentConversation.Messages.Add(_currentChat.Messages.Last());
                    _botMessageStream.Clear();
                    RefreshProperty(nameof(BotMessage));

                } catch (Exception ex) {
                    Debug.WriteLine(ex.Message);
                    StopWaitingForResponse();
                    DisplayConversationErrorMessage(ex.Message);
                }
            }).ContinueWith(_ => _conversationsService.SaveConversations(Conversations));
        }
    }

    [RelayCommand]
    private async Task CreateConversation() {
        _currentConversation = new Conversation() {
            Name = DateTime.Now.ToShortDateString(),
            CreatedAt = DateTime.Now
        };

        _currentChat = _ollamaService.CreateChat();
        var initialMessage = new Message(ChatRole.System, "You are a helpful AI assistant");

        _currentChat.Messages.Add(initialMessage);
        _currentConversation.Messages.Add(initialMessage);

        Conversations.Add(_currentConversation);

        await _conversationsService.SaveConversations(Conversations);
    }

    [RelayCommand]
    private async Task DeleteConversation(Conversation conversation) {
        if (conversation == null) return;

        var message = $"Are you sure you want to delete conversation '{conversation.Name}'?";

        try {
            var result = await _dialogService.ShowYesNoDialog("Delete conversation", message);

            if (result) {
                Conversations.Remove(conversation);

                await _conversationsService.SaveConversations(Conversations);
            }
        } catch (InvalidOperationException ex) {
            base.NotifyException(ex);
        } catch (Exception ex) {
            base.NotifyException(ex);
        }
    }

    [RelayCommand]
    private async Task ChangeConversationName(string conversationName) {
        CurrentConversation.Name = conversationName;

        await _conversationsService.SaveConversations(Conversations);
    }

    [RelayCommand]
    private void RemoveAttachedFile(string filePath) {
        AttachedFiles.Remove(filePath);
    }

    [RelayCommand]
    private void OpenAttachFileDialog() {
        var result = _dialogService.ShowSelectionFileDialog();

        if(result != string.Empty) {
            AttachedFiles.Add(result);
        }
    }

    private void StartWaitingForResponse() {
        IsWaitingForResponse = true;
        RefreshProperty(nameof(IsWaitingForResponse));
    }

    private void StopWaitingForResponse() {
        IsWaitingForResponse = false;
        RefreshProperty(nameof(IsWaitingForResponse));
    }

    // Show the error message on chat
    private void DisplayConversationErrorMessage(string message) {
        ErrorMessage = message;
        RefreshProperty(nameof(ErrorMessage));
    }

    private void RefreshProperty(string propertyName) {
        OnPropertyChanged(propertyName);
    }
}
