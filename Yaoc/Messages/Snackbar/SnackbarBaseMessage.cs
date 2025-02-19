using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Yaoc.Messages.Snackbar;

public class SnackbarBaseMessage<T> : ValueChangedMessage<T> {
    public SnackbarAction Action { get; }

    public SnackbarBaseMessage(T value, SnackbarAction action) : base(value) {
        Action = action;
    }
}
