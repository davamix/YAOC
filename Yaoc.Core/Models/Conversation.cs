using System.Collections.ObjectModel;

namespace Yaoc.Core.Models;
public class Conversation {
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public ObservableCollection<ChatMessage> Messages { get; set; }
    public DateTime CreatedAt { get; set; }

    public Conversation() {
        Messages = new ObservableCollection<ChatMessage>();
    }
}
