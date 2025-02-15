﻿using Yaoc.Extensions;
using Yaoc.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Yaoc {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private readonly IHost _host;

        public App() {
            _host = new HostBuilder()
                .ConfigureAppConfiguration((context, config) => {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) => {
                    services.RegisterServices();
                    services.RegisterViewModels();
                    services.RegisterViews();
                    services.RegisterDialogs();
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var mainWindow = _host.Services.GetService<MainWindow>();
            mainWindow?.Show();
        }

        protected override async void OnExit(ExitEventArgs e) {
            using (_host) {
                await _host.StopAsync();
            }

            base.OnExit(e);
        }
    }

}
