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
    private readonly IChromaDbService _chromaDbService;
    private readonly IConfiguration _configuration;

    [ObservableProperty]
    private string _ollamaServerUrl;
    [ObservableProperty]
    private bool? _isOllamaConnectionValid;
    [ObservableProperty]
    private string _chromaDbServerUrl;
    [ObservableProperty]
    private bool? _isChromaDbConnectionValid;

    public ObservableCollection<Plugin> PluginsAvailable { get; private set; } = new();

    public SettingsViewModel(
        IOllamaService ollamaService,
        IChromaDbService chromaDbService,
        IConfiguration configuration) {

        _ollamaService = ollamaService;
        _chromaDbService = chromaDbService;
        _configuration = configuration;

        OllamaServerUrl = _configuration["AppSettings:OllamaServer"];
        ChromaDbServerUrl = _configuration["AppSettings:ChromaDb:Server"];

        LoadPluginsAvailable();
    }

    [RelayCommand]
    public async Task TestOllamaConnection() {
        IsOllamaConnectionValid = await _ollamaService.TestConnection(OllamaServerUrl);
        RefreshProperty(nameof(IsOllamaConnectionValid));
    }

    [RelayCommand]
    public async Task TestChromaDbConnection() {
        IsChromaDbConnectionValid = await _chromaDbService.TestConnection(ChromaDbServerUrl);
        RefreshProperty(nameof(IsChromaDbConnectionValid));
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
