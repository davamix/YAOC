using Yaoc.Data;
using Yaoc.Services;
using Yaoc.ViewModels;
using Yaoc.Views;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using Yaoc.Dialogs;

namespace Yaoc.Extensions;
public static class Configuration {
    public static IServiceCollection RegisterServices(this IServiceCollection services) {
        services.AddSingleton<IOllamaService, OllamaService>();
        services.AddSingleton<IStorageProvider, FileSystemProvider>();
        services.AddSingleton<IDialogService, DialogService>();

        return services;
    }

    public static IServiceCollection RegisterViewModels(this IServiceCollection services) {
        services.AddScoped<MainViewModel>();
        services.AddScoped<SettingsDialogViewModel>();

        return services;
    }

    public static IServiceCollection RegisterViews(this IServiceCollection services) {
        services.AddSingleton<MainWindow>();

        return services;
    }

    public static IServiceCollection RegisterDialogs(this IServiceCollection services) {
        services.AddSingleton<YesNoDialog>();
        services.AddSingleton<SettingsDialog>();

        return services;
    }
}
