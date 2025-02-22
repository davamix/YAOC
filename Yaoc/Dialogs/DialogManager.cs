using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Text;

namespace Yaoc.Dialogs;

public interface IDialogManager {
    Task<bool> ShowYesNoDialog(string title, string message);
    string ShowSelectionFileDialog();
}

public class DialogManager : IDialogManager {
    private readonly IHost _host;

    public DialogManager(IHost host) {
        _host = host;
    }

    public async Task<bool> ShowYesNoDialog(string title, string message) {
        var dialog = _host.Services.GetService<YesNoDialog>();

        ThrowIfNull(dialog, typeof(YesNoDialog));

        dialog.DataContext = new { Title = title, Message = message };

        object? result = await DialogHost.Show(dialog, "RootDialogHost");

        return result switch {
            true => true,
            false => false,
            _ => false
        };
    }

    public string ShowSelectionFileDialog() {
        var ofd = new OpenFileDialog();
        var sbFilters = new StringBuilder();
        sbFilters.Append("Images Files (*.bmp;*.gif;*.jpg;.jpeg;*.png;*.tif;*.tiff)|*.bmp;*.gif;*.jpg;.jpeg;*.png;*.tif;*.tiff");
        sbFilters.Append("|Text Files (*.txt;*.csv;*.log;*.md;*.pdf;*.xml;*.json)|*.txt;*.csv;*.log;*.md;*.pdf;*.xml;*.json");

        var fiter = sbFilters.ToString();

        ofd.Filter = sbFilters.ToString();
        if (ofd.ShowDialog() == true) {
            return ofd.FileName;
        }

        return string.Empty;
    }

    private static void ThrowIfNull(object? dialog, Type dialogType) {
        if (dialog == null) throw new InvalidOperationException($"Cannot open {dialogType.FullName}");
    }
}
