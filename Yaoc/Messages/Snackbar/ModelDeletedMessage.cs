using OllamaSharp.Models;

namespace Yaoc.Messages.Snackbar;

public class ModelDeletedMessage : SnackbarBaseMessage<Model> {
    public string Message { get; }
    public ModelDeletedMessage(Model model, string message) : base(model, SnackbarActions.NoneAction()) {
        Message = message;
    }
}
