using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using Yaoc.Messages.Snackbar;

namespace Yaoc.ViewModels {
    public abstract class BaseViewModel : ObservableRecipient {
        public BaseViewModel() {
            IsActive = true;
        }

        internal virtual void NotifyException(Exception ex) {
            var sba = () => Clipboard.SetData(DataFormats.Text, ex.ToString());

            Messenger.Send(new ConversationErrorOccurredMessage(ex.Message, SnackbarActions.CopyAction(sba)));
        }
    }
}


