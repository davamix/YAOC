using Yaoc.Data;
using Yaoc.Services;
using Yaoc.ViewModels;
using Yaoc.Views;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;

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

        return services;
    }

    public static IServiceCollection RegisterViews(this IServiceCollection services) {
        services.AddSingleton<MainWindow>();

        return services;
    }

    public static IServiceCollection RegisterChatClients(this IServiceCollection services) {
        var client = new OllamaApiClient(new Uri("http://127.0.0.1:11434"));

        services.AddSingleton<IOllamaApiClient>(client);
        services.AddSingleton(new Chat(client));


        return services;
    }
}
