namespace Yaoc.Messages.Snackbar;

public class AttachResourceNotAllowedMessage : SnackbarBaseMessage<string> {
    public AttachResourceNotAllowedMessage(string value, SnackbarAction action) : base(value, action) {
    }
}
