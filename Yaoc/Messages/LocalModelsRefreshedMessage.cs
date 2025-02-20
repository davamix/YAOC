using CommunityToolkit.Mvvm.Messaging.Messages;
using OllamaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaoc.Messages
{
    public class LocalModelsRefreshedMessage : ValueChangedMessage<IEnumerable<Model>> {
        public LocalModelsRefreshedMessage(IEnumerable<Model> value) : base(value) {
        }
    }
}
