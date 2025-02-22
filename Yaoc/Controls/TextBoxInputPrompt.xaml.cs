using System.Windows;
using System.Windows.Controls;
using Yaoc.ViewModels;

namespace Yaoc.Controls {
    /// <summary>
    /// Interaction logic for TextBoxInputPrompt.xaml
    /// </summary>
    public partial class TextBoxInputPrompt : UserControl {
        public TextBoxInputPrompt() {
            InitializeComponent();
        }

        private void inputTextBox_PreviewDragOver(object sender, DragEventArgs e) {
            e.Handled = true;
        }

        private void inputTextBox_PreviewDrop(object sender, DragEventArgs e) {
            var data = e.Data.GetData(DataFormats.FileDrop);

            if(data != null) {
                var filePath = ((string[])data)[0];

                var context = this.DataContext as ConversationsViewModel;
                if(context != null) {
                    if (IsFileFormatAllowed(filePath)) {
                        context.AttachedFiles.Add(filePath);
                    }
                }
            }
        }

        private bool IsFileFormatAllowed(string filePath) {
            var allowedFormats = new List<string> {
                ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".tif", ".tiff",
                ".txt", ".csv", ".log", ".md", ".pdf", ".xml", ".json"
            };
            var extension = System.IO.Path.GetExtension(filePath).ToLower();
            return allowedFormats.Contains(extension);
        }
    }
}
