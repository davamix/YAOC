using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Yaoc.Extensions.UI;

namespace Yaoc.Controls;

public class CollectionNameChangedEventArgs : RoutedEventArgs {
    public string CollectionName { get; set; }

    public CollectionNameChangedEventArgs(RoutedEvent routedEvent, object source, string name)
        : base(routedEvent, source) {
        CollectionName = name;
    }
}

/// <summary>
/// Interaction logic for ListViewItemCollection.xaml
/// </summary>
public partial class ListViewItemCollection : ListViewItem {
    public static readonly RoutedEvent CollectionNameChangedEvent =
        EventManager.RegisterRoutedEvent(
            "CollectionNameChanged",
             RoutingStrategy.Bubble,
            typeof(EventHandler<string>),
            typeof(ListViewItemCollection));

    public event RoutedEventHandler CollectionNameChanged {
        add { AddHandler(CollectionNameChangedEvent, value); }
        remove { RemoveHandler(CollectionNameChangedEvent, value); }
    }

    public ListViewItemCollection() {
        InitializeComponent();
    }

    private void TextBlock_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
        e.Handled = true;

        // 2 click -> edit mode
        if (e.ClickCount == 2) {
            txtName.Visibility = Visibility.Collapsed;
            valueTextBox.Visibility = Visibility.Visible;

            valueTextBox.Text = txtName.Text;
            valueTextBox.Focus();
            valueTextBox.SelectAll();
        } else {
            var listView = (sender as DependencyObject).FindAncestor<ListView>();

            if (listView != null) {
                listView.SelectedItem = this.DataContext;
            }
        }
    }

    private void TextBox_LostKeyboardFocus(object sender, RoutedEventArgs e) {
        ApplyValue();
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            // If ApplyValue is call from here, the visibility changes will raise the
            // LostKeyboardFocus event and ApplyValue will call for the second time.
            // Just move the focus to the hiddenControl so the LostKeyboardFocus will call.
            valueTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }

    private void ApplyValue() {
        valueTextBox.Visibility = Visibility.Collapsed;
        txtName.Visibility = Visibility.Visible;
        txtName.Text = valueTextBox.Text;

        RaiseEvent(new CollectionNameChangedEventArgs(CollectionNameChangedEvent, this, valueTextBox.Text));
    }
}
