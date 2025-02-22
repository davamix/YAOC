using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Yaoc.Core.Services;

namespace Yaoc.ViewModels;

public partial class SettingsViewModel : ObservableObject {
    private readonly IOllamaService _ollamaService;
    private readonly IConfiguration _configuration;

    [ObservableProperty]
    private string _ollamaServerUrl;

    [ObservableProperty]
    private bool? _isConnectionValid;

    public SettingsViewModel(
        IOllamaService ollamaService,
        IConfiguration configuration) {

        _ollamaService = ollamaService;
        _configuration = configuration;

        OllamaServerUrl = _configuration["AppSettings:OllamaServer"];
    }

    [RelayCommand]
    public async Task TestConnection() {
        IsConnectionValid = await _ollamaService.TestConnection(OllamaServerUrl);
        RefreshProperty(nameof(IsConnectionValid));
    }

    private void RefreshProperty(string propertyName) {
        OnPropertyChanged(propertyName);
    }
}
