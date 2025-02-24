using OllamaSharp.Models;
using Yaoc.Core.Data;
using Yaoc.Core.Models;

namespace Yaoc.Core.Services;

public interface IConversationsService {
    Task<IEnumerable<Conversation>> LoadConversations();
    Task<IEnumerable<ChatMessage>> LoadMessages(string conversationId);
    Task<Conversation> SaveConversation(Conversation conversation);
    Task<IEnumerable<string>> GetConversationsWithModel(Model model);
    Task<ChatMessage> SaveMessage(ChatMessage message, string conversationId);
    Task DeleteCoversation(string conversationId);
}

public class ConversationsService : IConversationsService {
    private readonly IStorageProvider _storageProvider;

    public ConversationsService(IStorageProvider storageProvider) {
        _storageProvider = storageProvider;
    }

    public async Task<IEnumerable<Conversation>> LoadConversations() => await _storageProvider.LoadConversations();
    public async Task<IEnumerable<ChatMessage>> LoadMessages(string conversationId) => await _storageProvider.LoadMessages(conversationId);

    public async Task<Conversation> SaveConversation(Conversation conversation) => await _storageProvider.SaveConversation(conversation);

    // Return a list of conversations that use the model
    public async Task<IEnumerable<string>> GetConversationsWithModel(Model model) {
        var conversations = await LoadConversations();

        return conversations.Where(c => c.Model == model.Name).Select(c => c.Name);
    }

    public async Task<ChatMessage> SaveMessage(ChatMessage message, string conversationId) => await _storageProvider.SaveMessage(message, conversationId);

    public async Task DeleteCoversation(string conversationId) => await _storageProvider.DeleteConversation(conversationId);
}
