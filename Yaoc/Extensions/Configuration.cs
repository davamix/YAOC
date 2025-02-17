using Yaoc.Data;
using Yaoc.Services;
using Yaoc.ViewModels;
using Yaoc.Views;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddScoped<ConversationsViewModel>();
        services.AddScoped<SettingsViewModel>();

        return services;
    }

    public static IServiceCollection RegisterViews(this IServiceCollection services) {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<ConversationsView>();
        services.AddSingleton<SettingsView>();

        return services;
    }

    public static IServiceCollection RegisterDialogs(this IServiceCollection services) {
        services.AddSingleton<YesNoDialog>();

        return services;
    }
}
