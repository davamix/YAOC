using CommunityToolkit.Mvvm.Messaging.Messages;
using OllamaSharp.Models;

namespace Yaoc.Messages;

public class LocalModelsRefreshedMessage : ValueChangedMessage<IEnumerable<Model>> {
    public LocalModelsRefreshedMessage(IEnumerable<Model> value) : base(value) {
    }
}
