using Yaoc.Core.Data;
using Yaoc.Core.Services;
using Yaoc.ViewModels;
using Yaoc.Views;
using Microsoft.Extensions.DependencyInjection;
using Yaoc.Dialogs;
using System.Security.Policy;
using Yaoc.Core.Plugins;
using Yaoc.Core.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using Yaoc.Core.Data.ChromaDb;

namespace Yaoc.Extensions;
public static class Configuration {
    public static IServiceCollection RegisterServices(this IServiceCollection services) {
        services.AddSingleton<IOllamaService, OllamaService>();
        services.AddSingleton<IDialogManager, DialogManager>();
        services.AddSingleton<IConversationsService, ConversationsService>();
        services.AddSingleton<IChromaDbService, ChromaDbService>();
        services.AddSingleton<IKnowledgeService, KnoledgeService>();

        return services;
    }

    public static IServiceCollection RegisterViewModels(this IServiceCollection services) {
        services.AddScoped<MainViewModel>();
        services.AddScoped<ConversationsViewModel>();
        services.AddScoped<SettingsViewModel>();
        services.AddScoped<ModelsViewModel>();
        services.AddScoped<KnowledgeViewModel>();

        return services;
    }

    public static IServiceCollection RegisterViews(this IServiceCollection services) {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<ConversationsView>();
        services.AddSingleton<SettingsView>();
        services.AddSingleton<ModelsView>();
        services.AddSingleton<KnowledgeView>();

        return services;
    }

    public static IServiceCollection RegisterDialogs(this IServiceCollection services) {
        services.AddSingleton<YesNoDialog>();

        return services;
    }

    public static IServiceCollection RegisterProviders(this IServiceCollection services) {
        //services.AddSingleton<IStorageProvider, FileSystemProvider>();
        services.AddSingleton<IStorageProvider, SqliteProvider>();
        services.AddSingleton<IChromaDbProvider, ChromaDbProvider>();

        return services;
    }

    //public static IServiceCollection RegisterPlugins(this IServiceCollection services) {
    //    services.AddSingleton<IPluginsLoader, PluginsLoader>();
    //    return services;
    //}
}
