using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MaterialDesignThemes.Wpf;
using System.Diagnostics;
using Yaoc.Messages.Snackbar;
using Yaoc.Messges;

namespace Yaoc.ViewModels;


public partial class MainViewModel : BaseViewModel {

    [ObservableProperty]
    private bool _showNotificationMessage = true;

    [ObservableProperty]
    private SnackbarMessageQueue _notificationMessageQueue;

    public MainViewModel() {
        _notificationMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));
    }

    protected override void OnActivated() {
        WeakReferenceMessenger.Default.Register<MainViewModel, ConversationErrorOccurredMessage>(this, (r, m) => {
            NotificationMessageQueue.Enqueue(m.Value, m.Action.Label, m.Action.Action);
        });

        WeakReferenceMessenger.Default.Register<MainViewModel, ModelDeletedMessage>(this, (r, m) => {
            NotificationMessageQueue.Enqueue(m.Message);
        });
    }
}
