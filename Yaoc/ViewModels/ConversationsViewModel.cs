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
using Yaoc.Core.Plugins;
using System.IO;
using System.Windows.Data;

namespace Yaoc.ViewModels;

public partial class ConversationsViewModel : BaseViewModel {

    public ObservableCollection<string> Models { get; } = [];
    public ObservableCollection<Conversation> Conversations { get; } = [];
    public ObservableCollection<MessageResource> AttachedResources { get; set; } = [];

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
            InitializeSelectedConversation();
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
    //private readonly IPluginsLoader _pluginsLoader;
    private Chat _currentChat;

    public ConversationsViewModel(
        IOllamaService ollamaService,
        IConversationsService conversationsService,
        IDialogManager dialogService
        ) {

        _ollamaService = ollamaService;
        _conversationsService = conversationsService;
        _dialogService = dialogService;
        //_pluginsLoader = pluginsLoader;
        _botMessageStream = new StringBuilder();


        //Task.WhenAll(
        //    Task.Run(LoadModels),
        //    Task.Run(LoadConversations));
        Task.Run(LoadModels).Wait();
        Task.Run(LoadConversations).Wait();
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
        CurrentConversation.Model = SelectedModel;
        _currentChat.Model = SelectedModel;

        Task.Run(async () => await _conversationsService.SaveConversation(CurrentConversation));
    }

    private async void InitializeSelectedConversation() {
        if (CurrentConversation == null) return;

        _currentChat = _ollamaService.CreateChat();

        if (Models.Contains(CurrentConversation.Model)) {
            SelectedModel = CurrentConversation.Model;
            _currentChat.Model = CurrentConversation.Model;
        } else {
            // Because the model was deleted.
            SelectedModel = null;
        }

        var messages = await _conversationsService.LoadMessages(CurrentConversation.Id);
        CurrentConversation.Messages = new ObservableCollection<ChatMessage>(messages);

        foreach (var m in CurrentConversation.Messages) {
            _currentChat.Messages.Add(m.OllamaMessage);
        }
    }

    [RelayCommand]
    private async Task SendMessage() {
        if (string.IsNullOrEmpty(UserMessage)) return;

        var message = UserMessage;
        UserMessage = string.Empty;

        var userChatMessage = AttachedResources.Any() ?
            await CreateUserChatMessageWithFiles(message) :
            CreateUserChatMessage(message);

        var savedUserMessage = await _conversationsService.SaveMessage(userChatMessage, CurrentConversation.Id);

        AttachedResources.Clear();

        CurrentConversation.Messages.Add(savedUserMessage);

        _currentChat.Model = SelectedModel;

        _ = Application.Current.Dispatcher.Invoke(async () => {
            StartWaitingForResponse();

            try {
                await ProcessAssistantResponse(userChatMessage.OllamaMessage.Content);


            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                StopWaitingForResponse();
                DisplayConversationErrorMessage(ex.Message);
            }
        });
    }

    [RelayCommand]
    private async Task CreateConversation() {
        var newConversation = await _conversationsService.SaveConversation(new() {
            Name = DateTime.Now.ToShortDateString()
        });

        var systemPrompt = """
            - You are a helpful AI assistant. 
            - Content files will be attached to the conversation in this format: # FILE file_name {content file} # EOF
            """;
        var initialMessage = new ChatMessage() {
            ConversationId = newConversation.Id,
            Message = systemPrompt,
            OllamaMessage = new Message(ChatRole.System, systemPrompt)
        };

        await _conversationsService.SaveMessage(initialMessage, newConversation.Id);

        Conversations.Add(newConversation);

        _currentConversation = newConversation;

        // Ollama chat object initialization
        _currentChat = _ollamaService.CreateChat();
        _currentChat.Messages.Add(initialMessage.OllamaMessage);
    }

    [RelayCommand]
    private async Task DeleteConversation(Conversation conversation) {
        if (conversation == null) return;

        var message = $"Are you sure you want to delete conversation '{conversation.Name}'?";

        try {

            var result = await _dialogService.ShowYesNoDialog("Delete conversation", message);

            if (result) {
                Conversations.Remove(conversation);

                await _conversationsService.DeleteCoversation(conversation.Id);
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

        await _conversationsService.SaveConversation(CurrentConversation);
    }

    [RelayCommand]
    private void RemoveAttachedFile(MessageResource resource) {
        AttachedResources.Remove(resource);
    }

    [RelayCommand]
    private void OpenAttachFileDialog() {
        var extensionAvailable = string.Join(";", PluginsLoader.GetFileParserPlugins().Values.Select(x => x.GetFileFilters()));
        var result = _dialogService.ShowSelectionFileDialog(extensionAvailable);

        if (result != string.Empty) {
            AttachedResources.Add(
                new MessageResource() {
                    Path = result,
                    Name = Path.GetFileName(result)
                });
        }
    }

    [RelayCommand]
    private void AttachedResourceNotAllowed(string message) {
        var errorMessage = new AttachResourceNotAllowedMessage(message, SnackbarActions.NoneAction());

        base.NotifyErrorMessage(errorMessage);
    }

    [RelayCommand]
    private void ResourceAttached(string resourcePaht) {
        var message = new MessageResource {
            Path = resourcePaht,
            Name = Path.GetFileName(resourcePaht)
        };

        AttachedResources.Add(message);
    }

    private async Task ProcessAssistantResponse(string message) {
        await foreach (var response in _ollamaService.SendMessage(_currentChat, message)) {
            if (IsWaitingForResponse) {
                StopWaitingForResponse();
            }

            await Task.Delay(1); // Needed to prevent UI from freezing. Task.Yield() doesn't work.
            BotMessage = response;
        }

        ChatMessage assistantMessage = CreateAssistantChatMessage();

        var savedAssistantMessage = await _conversationsService.SaveMessage(assistantMessage, CurrentConversation.Id);

        CurrentConversation.Messages.Add(savedAssistantMessage);
        _botMessageStream.Clear();
        RefreshProperty(nameof(BotMessage));
    }

    private ChatMessage CreateUserChatMessage(string message) {
        return new ChatMessage() {
            ConversationId = CurrentConversation.Id,
            Message = message,
            OllamaMessage = new Message(ChatRole.User, message)
        };
    }

    private async Task<ChatMessage> CreateUserChatMessageWithFiles(string message) {
        var parsedTexts = new List<string>();
        parsedTexts.Add(message);

        foreach (var resource in AttachedResources) {
            var parser = PluginsLoader.GetFileParserPlugins().Values.FirstOrDefault(p => p.Extensions.Contains(Path.GetExtension(resource.Path)));
            if (parser != null) {
                var parsedText = await parser.Parse(resource.Path);
                parsedTexts.Add(parsedText);
            }
        }

        return new ChatMessage() {
            ConversationId = CurrentConversation.Id,
            Message = message,
            AttachedResources = AttachedResources.ToList(),
            OllamaMessage = new Message(ChatRole.User, string.Join("\n", parsedTexts))
        };
    }

    private ChatMessage CreateAssistantChatMessage() {
        return new ChatMessage() {
            ConversationId = CurrentConversation.Id,
            Message = _currentChat.Messages.Last().Content,
            OllamaMessage = _currentChat.Messages.Last()
        };
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
