using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yaoc.ViewModels;

namespace Yaoc.Views;

/// <summary>
/// Interaction logic for KnowledgeView.xaml
/// </summary>
public partial class KnowledgeView : UserControl
{
    public KnowledgeView()
    {
        InitializeComponent();

        this.DataContext = App.Host.Services.GetService(typeof(KnowledgeViewModel));
    }
}
