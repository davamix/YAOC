using Yaoc.ViewModels;
using System.Windows;

namespace Yaoc.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow(MainViewModel viewModel) {
        InitializeComponent();

        this.DataContext = viewModel;
    }
}