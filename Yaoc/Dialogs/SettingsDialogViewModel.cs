using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaoc.Services;

namespace Yaoc.Dialogs
{
    public partial class SettingsDialogViewModel: ObservableObject {
        private readonly IOllamaService _ollamaService;
        private readonly IConfiguration _configuration;
        [ObservableProperty]
        private string _ollamaServerUrl;

        [ObservableProperty]
        private bool? _isConnectionValid;

        public SettingsDialogViewModel(
            IOllamaService ollamaService,
            IConfiguration configuration) {
            _ollamaService = ollamaService;
            _configuration = configuration;

            OllamaServerUrl = _configuration["AppSettings:OllamaServer"];
            //RefreshProperty(nameof(OllamaServerUrl));
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
}
