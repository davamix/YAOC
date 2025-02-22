using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Yaoc.Extensions.UI;

namespace Yaoc.Controls
{
    /// <summary>
    /// Interaction logic for ListViewItemLocalModel.xaml
    /// </summary>
    public partial class ListViewItemLocalModel : ListViewItem
    {
        public ListViewItemLocalModel()
        {
            InitializeComponent();

        }

        private void ListViewItem_MouseUp(object sender, MouseButtonEventArgs e) {
            e.Handled = true;

            var listView = (sender as DependencyObject)?.FindAncestor<ListView>();

            if (listView != null) {
                listView.SelectedItem = this.DataContext;
            }
        }
    }
}
