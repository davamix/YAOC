using OllamaSharp.Models;
using Yaoc.Core.Data;
using Yaoc.Core.Models;

namespace Yaoc.Core.Services;

public interface IConversationsService {
    Task<IEnumerable<Conversation>> LoadConversations();
    Task SaveConversations(IEnumerable<Conversation> conversations);
    Task<IEnumerable<string>> GetConversationsWithModel(Model model);
}

public class ConversationsService : IConversationsService {
    private readonly IStorageProvider _storageProvider;

    public ConversationsService(IStorageProvider storageProvider) {
        _storageProvider = storageProvider;
    }

    public async Task<IEnumerable<Conversation>> LoadConversations() => await _storageProvider.LoadConversations();

    public async Task SaveConversations(IEnumerable<Conversation> conversations) => await _storageProvider.SaveConversations(conversations);

    // Return a list of conversations that use the model
    public async Task<IEnumerable<string>> GetConversationsWithModel(Model model) {
        var conversations = await LoadConversations();

        return conversations.Where(c => c.Model == model.Name).Select(c => c.Name);
    }
}
