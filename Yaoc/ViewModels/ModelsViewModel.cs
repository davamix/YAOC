using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OllamaSharp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Yaoc.Services;

namespace Yaoc.ViewModels;

public partial class ModelsViewModel : ObservableObject {
    private readonly IOllamaService _ollamaService;

    public ObservableCollection<Model> LocalModels { get; private set; }

    public ModelsViewModel(IOllamaService ollamaService) {
        _ollamaService = ollamaService;

        LocalModels = new ObservableCollection<Model>();

        Task.WhenAll(
            Task.Run(LoadLocalModels));
    }

    private async Task LoadLocalModels() {
        LocalModels.Clear();
        var models = await _ollamaService.GetLocalModelsAsync();

        foreach (Model model in models) {
            LocalModels.Add(model);
        }
    }

    [RelayCommand]
    private void DeleteLocalModel(Model model) {
        Debug.WriteLine($"Delete local model: {model.Name}");
    }
}
