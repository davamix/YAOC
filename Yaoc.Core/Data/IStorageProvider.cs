using Yaoc.Core.Models;

namespace Yaoc.Core.Data;

public interface IStorageProvider
{
    Task SaveConversation(Conversation conversation);
    Task SaveConversations(IEnumerable<Conversation> conversations);
    Task<IEnumerable<Conversation>> LoadConversations();
}
