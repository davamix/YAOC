using Yaoc.Core.Models;

namespace Yaoc.Core.Data;

public interface IStorageProvider
{
    /// <summary>
    /// Load all conversations from the storage without messages
    /// </summary>
    /// <returns>List of Conversations</returns>
    Task<IEnumerable<Conversation>> LoadConversations();
    /// <summary>
    /// Load all messages for a conversation
    /// </summary>
    /// <param name="conversationId"></param>
    /// <returns>List of Messages</returns>
    Task<IEnumerable<ChatMessage>> LoadMessages(string conversationId);
    /// <summary>
    /// Save conversation data only, no messages
    /// </summary>
    /// <param name="conversation"></param>
    /// <returns>Return the saved conversation with the new Id if is a new conversation</returns>
    Task<Conversation> SaveConversation(Conversation conversation);
    /// <summary>
    /// Save the message conversation
    /// </summary>
    /// <param name="message"></param>
    /// <param name="conversationId"></param>
    /// <returns>Return a completed Task, messages have no Id's</returns>
    Task<ChatMessage> SaveMessage(ChatMessage message, string conversationId);

    /// <summary>
    /// Delete a conversation and all its messages
    /// </summary>
    /// <param name="conversationId"></param>
    /// <returns>Return a completed Task</returns>
    Task DeleteConversation(string conversationId);
    
}
