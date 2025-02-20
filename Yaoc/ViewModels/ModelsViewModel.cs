using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OllamaSharp.Models;
using OllamaSharp.Models.Chat;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Data;
using Yaoc.Messages;
using Yaoc.Messages.Snackbar;
using Yaoc.Messges;
using Yaoc.Services;

namespace Yaoc.ViewModels;

public partial class ModelsViewModel : BaseViewModel {
    private readonly IOllamaService _ollamaService;
    private readonly IDialogService _dialogService;
    private readonly IConversationsService _conversationsService;

    public ObservableCollection<Model> LocalModels { get; private set; }

    public ModelsViewModel(
        IOllamaService ollamaService, 
        IDialogService dialogService,
        IConversationsService conversationsService) {
        _ollamaService = ollamaService;
        _dialogService = dialogService;
        _conversationsService = conversationsService;
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
        var conversations = await _conversationsService.GetConversationsWithModel(model);
        var dialogMessage = string.Empty;

        if(conversations.Any()) {
            dialogMessage = $"Model {model.Name} is in use in conversations: {string.Join(", ", conversations)}.\n\nDo you want to delete the model {model.Name} anyways?";
        } else {
            dialogMessage = $"Do you want to delete {model.Name} model?";
        }

        var dialogResult = await _dialogService.ShowYesNoDialog("Delete model", dialogMessage);

        if (!dialogResult) return;

        try {
            await _ollamaService.DeleteLocalModelAsync(model.Name);

            LocalModels.Remove(model);

            Messenger.Send(new ModelDeletedMessage(model, $"Model deleted: {model.Name}"));
        } catch (Exception ex) {
            Debug.WriteLine(ex.Message);
            NotifyException(ex);
        }
    }

    [RelayCommand]
    private async Task RefreshLocalModels() {
        await LoadLocalModels();

        Messenger.Send(new LocalModelsRefreshedMessage(LocalModels));
    }
}
