using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Yaoc.Extensions.UI;

namespace Yaoc.Controls;

public class ConversationNameChangedEventArgs : RoutedEventArgs {
    public string ConversationName { get; set; }

    public ConversationNameChangedEventArgs(RoutedEvent routedEvent, object source, string name) 
        :base(routedEvent, source){
        ConversationName = name;
    }
}

/// <summary>
/// Interaction logic for ListViewItemEditable.xaml
/// </summary>
public partial class ListViewItemConversation : ListViewItem {

    public static readonly RoutedEvent ConversationNameChangedEvent =
        EventManager.RegisterRoutedEvent(
            "ConversationNameChanged",
             RoutingStrategy.Bubble,
            typeof(EventHandler<string>),
            typeof(ListViewItemConversation));

    public event RoutedEventHandler ConversationNameChanged {
        add { AddHandler(ConversationNameChangedEvent, value); }
        remove { RemoveHandler(ConversationNameChangedEvent, value); }
    }

    public ListViewItemConversation() {
        InitializeComponent();
    }

    private void TextBlock_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
        e.Handled = true;

        // 2 click -> edit mode
        if (e.ClickCount == 2) {
            valueLabel.Visibility = Visibility.Collapsed;
            valueTextBox.Visibility = Visibility.Visible;

            valueTextBox.Text = (string)valueLabel.Content;
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
        valueLabel.Visibility = Visibility.Visible;
        valueLabel.Content = valueTextBox.Text;

        RaiseEvent(new ConversationNameChangedEventArgs(ConversationNameChangedEvent, this, valueTextBox.Text));
    }
}
