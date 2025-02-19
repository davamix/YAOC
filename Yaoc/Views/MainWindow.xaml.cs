using System.Windows;
using Yaoc.ViewModels;

namespace Yaoc.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();

        this.DataContext = App.Host.Services.GetService(typeof(MainViewModel));
    }
}