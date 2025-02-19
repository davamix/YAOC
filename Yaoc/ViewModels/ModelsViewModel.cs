using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OllamaSharp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Yaoc.Messages.Snackbar;
using Yaoc.Messges;
using Yaoc.Services;

namespace Yaoc.ViewModels;

public partial class ModelsViewModel : BaseViewModel {
    private readonly IOllamaService _ollamaService;
    private readonly IDialogService _dialogService;

    public ObservableCollection<Model> LocalModels { get; private set; }

    public ModelsViewModel(IOllamaService ollamaService, IDialogService dialogService) {
        _ollamaService = ollamaService;
        _dialogService = dialogService;
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
    private async Task DeleteLocalModel(Model model) {
        var result = await _dialogService.ShowYesNoDialog("Delete model", $"Do you want to delete {model.Name} model?");

        if (!result) return;

        try {
            await _ollamaService.DeleteLocalModelAsync(model.Name);

            LocalModels.Remove(model);

            Messenger.Send(new ModelDeletedMessage(model, $"Model deleted: {model.Name}"));
        } catch (Exception ex) {
            Debug.WriteLine(ex.Message);
            NotifyException(ex);
        }
    }
}
