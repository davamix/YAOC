using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Yaoc.Core.Models;
using Yaoc.Core.Plugins;
using Yaoc.Core.Services;

namespace Yaoc.ViewModels;

public partial class SettingsViewModel : ObservableObject {
    private readonly IOllamaService _ollamaService;
    private readonly IConfiguration _configuration;

    [ObservableProperty]
    private string _ollamaServerUrl;

    [ObservableProperty]
    private bool? _isConnectionValid;

    public ObservableCollection<Plugin> PluginsAvailable { get; private set; } = new();

    public SettingsViewModel(
        IOllamaService ollamaService,
        IConfiguration configuration) {

        _ollamaService = ollamaService;
        _configuration = configuration;

        OllamaServerUrl = _configuration["AppSettings:OllamaServer"];

        LoadPluginsAvailable();
    }

    [RelayCommand]
    public async Task TestConnection() {
        IsConnectionValid = await _ollamaService.TestConnection(OllamaServerUrl);
        RefreshProperty(nameof(IsConnectionValid));
    }

    private void LoadPluginsAvailable() {
        PluginsAvailable.Clear();
        var plugins = PluginsLoader.GetFileParserPlugins().Values.Select(x => new { Name = x.Name, Description = x.Description });

        foreach(var p in plugins) {
            PluginsAvailable.Add(new Plugin(p.Name, p.Description));
        }
    }

    private void RefreshProperty(string propertyName) {
        OnPropertyChanged(propertyName);
    }
}
