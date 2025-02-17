using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using Yaoc.Data;
using Yaoc.Models;
using Yaoc.Services;

namespace Yaoc.ViewModels;

public partial class ConversationsViewModel : ObservableObject {

    public ObservableCollection<string> Models { get; } = [];
    public ObservableCollection<Conversation> Conversations { get; } = [];

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

    [ObservableProperty]
    private bool _canSelectModel;

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

    [ObservableProperty]
    private bool _showErrorMessage = false;

    [ObservableProperty]
    private SnackbarMessageQueue _errorMessageQueue;

    private readonly IOllamaService _ollamaService;
    private readonly IStorageProvider _storageProvider;
    private readonly IDialogService _dialogService;
    private Chat _currentChat;

    public ConversationsViewModel(
        IOllamaService ollamaService,
        IStorageProvider storageProvider,
        IDialogService dialogService) {

        _ollamaService = ollamaService;
        _storageProvider = storageProvider;
        _dialogService = dialogService;
        _botMessageStream = new StringBuilder();
        _errorMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));

        Task.WhenAll(
            Task.Run(LoadModels),
            Task.Run(LoadConversations));
    }

    private async Task LoadModels() {
        foreach (var model in await _ollamaService.GetModelsAsync()) {
            Models.Add(model);
        }
    }

    private async Task LoadConversations() {
        var conversations = await _storageProvider.LoadConversations();
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

        if (CurrentConversation != null) {
            _currentChat.Model = _currentConversation.Model;
            SelectedModel = _currentConversation.Model;

            foreach (var m in CurrentConversation.Messages) {
                _currentChat.Messages.Add(m);
            }

            CanSelectModel = !CurrentConversation.Messages.Any(x => x.Role == ChatRole.User);
        }
    }

    [RelayCommand]
    private void SendMessage() {
        if (!string.IsNullOrEmpty(UserMessage)) {
            var message = UserMessage;
            UserMessage = string.Empty;

            _currentConversation.Messages.Add(new Message(ChatRole.User, message));
            CanSelectModel = false;

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
                    DisplayErrorMessage(ex);
                }
            }).ContinueWith(_ => _storageProvider.SaveConversations(Conversations));
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

        await _storageProvider.SaveConversations(Conversations);
    }

    [RelayCommand]
    private async Task DeleteConversation(Conversation conversation) {
        if (conversation == null) return;

        var message = $"Are you sure you want to delete conversation '{conversation.Name}'?";

        try {
            var result = await _dialogService.ShowYesNoDialog("Delete conversation", message);

            if (result) {
                Conversations.Remove(conversation);

                await _storageProvider.SaveConversations(Conversations);
            }
        } catch (InvalidOperationException ex) {
            Debug.WriteLine(ex.Message);
            DisplayErrorMessage(ex, true);
        } catch (Exception ex) {
            Debug.WriteLine(ex.Message);
            DisplayErrorMessage(ex, true);
        }
    }

    [RelayCommand]
    private async Task ChangeConversationName(string conversationName) {
        CurrentConversation.Name = conversationName;

        await _storageProvider.SaveConversations(Conversations);
    }

    private void StartWaitingForResponse() {
        IsWaitingForResponse = true;
        RefreshProperty(nameof(IsWaitingForResponse));
    }

    private void StopWaitingForResponse() {
        IsWaitingForResponse = false;
        RefreshProperty(nameof(IsWaitingForResponse));
    }

    private void DisplayErrorMessage(string message, bool showError = false) {
        ErrorMessageQueue.Enqueue(message, "COPY", () => Clipboard.SetData(DataFormats.Text, message));
        ShowErrorMessage = showError;
        RefreshProperty(nameof(ErrorMessage));
    }

    private void DisplayErrorMessage(Exception ex, bool showError = false) {
        ErrorMessageQueue.Enqueue(ex.Message, "COPY", () => Clipboard.SetData(DataFormats.Text, ex.ToString()));
        ShowErrorMessage = showError;
        RefreshProperty(nameof(ErrorMessage));
    }

    private void RefreshProperty(string propertyName) {
        OnPropertyChanged(propertyName);
    }
}
