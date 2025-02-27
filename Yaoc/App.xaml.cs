using Yaoc.Extensions;
using Yaoc.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Yaoc.Core.Plugins;

namespace Yaoc {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static IHost Host { get; private set; }

        public App() {
            Host = new HostBuilder()
                .ConfigureAppConfiguration((context, config) => {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) => {
                    services.RegisterServices();
                    services.RegisterViewModels();
                    services.RegisterViews();
                    services.RegisterDialogs();
                    services.RegisterProviders();
                    //services.RegisterPlugins();
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            PluginsLoader.Initialize(Host.Services.GetService<IConfiguration>());

            var mainWindow = Host.Services.GetService<MainWindow>();
            mainWindow?.Show();
        }

        protected override async void OnExit(ExitEventArgs e) {
            using (Host) {
                await Host.StopAsync();
            }

            base.OnExit(e);
        }
    }

}
