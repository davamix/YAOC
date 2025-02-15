using Yaoc.Dialogs;
using MaterialDesignThemes.Wpf;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Yaoc.Services;

public interface IDialogService {
    Task<bool> ShowYesNoDialog(string title, string message);
    Task<bool> ShowSettingsDialog();
}

public class DialogService : IDialogService {
    private readonly IHost _host;

    public DialogService(IHost host) {
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

    public async Task<bool> ShowSettingsDialog() {
        var dialog = _host.Services.GetService<SettingsDialog>();

        ThrowIfNull(dialog, typeof(SettingsDialog));

        object? result = await DialogHost.Show(dialog, "RootDialogHost");

        return result switch {
            true => true,
            false => false,
            _ => false
        };
    }

    private static void ThrowIfNull(object? dialog, Type dialogType) {
        if (dialog == null) throw new InvalidOperationException($"Cannot open {dialogType.FullName}");
    }
}
