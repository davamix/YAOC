using System.Windows.Controls;
using Yaoc.ViewModels;

namespace Yaoc.Views;

public partial class ConversationsView : UserControl {
    public ConversationsView()
    {
        InitializeComponent();

        DataContext = App.Host.Services.GetService(typeof(ConversationsViewModel));
    }
}
