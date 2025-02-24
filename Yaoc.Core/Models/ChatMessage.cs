using OllamaSharp.Models.Chat;

namespace Yaoc.Core.Models;

public class ChatMessage
{
    public string Id { get; set; }
    public string ConversationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Message OllamaMessage { get; set; }
}
