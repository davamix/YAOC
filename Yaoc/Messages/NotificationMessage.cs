using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Yaoc.Messges
{
    public class NotificationMessage:ValueChangedMessage<string>
    {
        public NotificationMessage(string message):base(message) {
            
        }
    }
}
