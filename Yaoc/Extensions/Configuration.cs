using Yaoc.Core.Data;
using Yaoc.Core.Services;
using Yaoc.ViewModels;
using Yaoc.Views;
using Microsoft.Extensions.DependencyInjection;
using Yaoc.Dialogs;

namespace Yaoc.Extensions;
public static class Configuration {
    public static IServiceCollection RegisterServices(this IServiceCollection services) {
        services.AddSingleton<IOllamaService, OllamaService>();
        services.AddSingleton<IDialogManager, DialogManager>();
        services.AddSingleton<IConversationsService, ConversationsService>();

        return services;
    }

    public static IServiceCollection RegisterViewModels(this IServiceCollection services) {
        services.AddScoped<MainViewModel>();
        services.AddScoped<ConversationsViewModel>();
        services.AddScoped<SettingsViewModel>();
        services.AddScoped<ModelsViewModel>();

        return services;
    }

    public static IServiceCollection RegisterViews(this IServiceCollection services) {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<ConversationsView>();
        services.AddSingleton<SettingsView>();
        services.AddSingleton<ModelsView>();

        return services;
    }

    public static IServiceCollection RegisterDialogs(this IServiceCollection services) {
        services.AddSingleton<YesNoDialog>();

        return services;
    }

    public static IServiceCollection RegisterProviders(this IServiceCollection services) {
        services.AddSingleton<IStorageProvider, FileSystemProvider>();
        
        return services;
    }
}
