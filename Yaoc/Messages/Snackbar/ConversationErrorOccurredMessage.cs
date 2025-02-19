namespace Yaoc.Messages.Snackbar;

public class ConversationErrorOccurredMessage : SnackbarBaseMessage<string> {
    public ConversationErrorOccurredMessage(string value, SnackbarAction action) : base(value, action) {
    }
}
