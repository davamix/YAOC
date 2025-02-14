using Yaoc.Dialogs;
using MaterialDesignThemes.Wpf;

namespace Yaoc.Services;

public interface IDialogService {
    Task<bool> ShowYesNoDialog(string title, string message);
}

public class DialogService : IDialogService {
    public async Task<bool> ShowYesNoDialog(string title, string message) {
        object? dialog = new YesNoDialog() {
            DataContext = new { Title = title, Message = message }
        };

        object? result = await DialogHost.Show(dialog, "RootDialogHost");

        return result switch {
            true => true,
            false => false,
            _ => false
        };
    }
}
