using System.Windows.Controls;
using Yaoc.ViewModels;

namespace Yaoc.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();

            this.DataContext = App.Host.Services.GetService(typeof(SettingsViewModel));
        }
    }
}
