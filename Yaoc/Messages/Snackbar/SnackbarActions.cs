namespace Yaoc.Messages.Snackbar;

public record SnackbarAction(string Label, Action Action);

public static class SnackbarActions
{
    public static SnackbarAction NoneAction() =>
        new SnackbarAction(string.Empty, () => { });

    public static SnackbarAction CopyAction(Action action) =>
        new SnackbarAction("COPY", action);
}
