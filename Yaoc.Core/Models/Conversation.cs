using OllamaSharp.Models.Chat;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Yaoc.Core.Models;
public class Conversation {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public ObservableCollection<Message> Messages { get; set; }
    public DateTime CreatedAt { get; set; }

    public Conversation() : this(Guid.NewGuid().ToString()) { }

    [JsonConstructor]
    public Conversation(string id) {
        Messages = new ObservableCollection<Message>();
        Id = id;

    }
}
