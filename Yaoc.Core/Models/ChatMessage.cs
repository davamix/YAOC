using OllamaSharp.Models.Chat;

namespace Yaoc.Core.Models;

public class ChatMessage
{
    public string Id { get; set; }
    public string ConversationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Message { get; set; }
    public Message OllamaMessage { get; set; }
    public List<MessageResource> AttachedResources { get; set; }

    public ChatMessage() {
        AttachedResources = new List<MessageResource>();
    }

}
