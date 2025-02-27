using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using Yaoc.Core.Models;
using Yaoc.Core.Plugins;
using Yaoc.ViewModels;

namespace Yaoc.Controls {
    public class AttachedResourceNotAllowedEventArgs : EventArgs {
        public string Message { get; set; }

        public AttachedResourceNotAllowedEventArgs(string message) {
            Message = message;
        }
    }

    /// <summary>
    /// Interaction logic for TextBoxInputPrompt.xaml
    /// </summary>
    public partial class TextBoxInputPrompt : UserControl {

        public event EventHandler<AttachedResourceNotAllowedEventArgs> AttachedResourceNotAllowed;

        public TextBoxInputPrompt() {
            InitializeComponent();
        }

        private void inputTextBox_PreviewDragOver(object sender, DragEventArgs e) {
            e.Handled = true;
        }

        private void inputTextBox_PreviewDrop(object sender, DragEventArgs e) {
            var data = e.Data.GetData(DataFormats.FileDrop);

            if (data != null) {
                var resourcePath = ((string[])data)[0];

                var context = this.DataContext as ConversationsViewModel;
                if (context != null) {
                    if (IsFileFormatAllowed(resourcePath)) {
                        context.AttachedResources.Add(
                            new MessageResource {
                                Path = resourcePath,
                                Name = System.IO.Path.GetFileName(resourcePath)
                            });
                    } else {
                        RaiseAttachedResourceNotAllowed(System.IO.Path.GetExtension(resourcePath).ToLower());
                    }
                }
            }
        }

        private bool IsFileFormatAllowed(string resourcePath) {
            var allowedFormats = PluginsLoader.GetFileParserPlugins().Values.SelectMany(x => x.Extensions);
            var extension = System.IO.Path.GetExtension(resourcePath).ToLower();

            return allowedFormats.Contains(extension);
        }

        private void RaiseAttachedResourceNotAllowed(string extension) {
            var message = $"Attached resource format is not allowed ({extension}).\n";
            message += $"Allowed formats are: {string.Join(", ", PluginsLoader.GetFileParserPlugins().Values.SelectMany(x => x.Extensions))}";
            
            OnAttachedResourceNotAllowed(new AttachedResourceNotAllowedEventArgs(message));
        }

        protected virtual void OnAttachedResourceNotAllowed(AttachedResourceNotAllowedEventArgs e) {
            AttachedResourceNotAllowed?.Invoke(this, e);
        }
    }
}
