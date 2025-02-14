using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Yaoc.Behaviours
{
    public class TextBoxAddNewLineBehavior: Behavior<TextBox>
    {

        protected override void OnAttached() {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching() {
            base.OnDetaching();

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            var textBox = sender as TextBox;

            if (textBox != null) {
                textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Shift) {
                TextBox textBox = sender as TextBox;
                if (textBox != null) {
                    int caretIndex = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Insert(caretIndex, "\n");
                    textBox.CaretIndex = caretIndex + 1;
                    e.Handled = true; // Prevents the default behavior
                }
            }
        }
    }
}
