using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using Yaoc.Behaviours;
using Yaoc.Core.Models;
using Yaoc.Core.Plugins;
using Yaoc.ViewModels;

namespace Yaoc.Controls {

    /// <summary>
    /// Interaction logic for TextBoxInputPrompt.xaml
    /// </summary>
    public partial class TextBoxInputPrompt : UserControl {

        public event EventHandler<AttachedResourceNotAllowedEventArgs> AttachedResourceNotAllowed;
        public event EventHandler<ResourceAttachedEventArgs> ResourceAttached;

        public TextBoxInputPrompt() {
            InitializeComponent();
        }

        private void TextBoxFileDropBehavior_ResourceAttached(object sender, ResourceAttachedEventArgs e) {
            ResourceAttached?.Invoke(this, e);
        }

        private void TextBoxFileDropBehavior_AttachedResourceNotAllowed(object sender, AttachedResourceNotAllowedEventArgs e) {
            AttachedResourceNotAllowed?.Invoke(this, e);
        }
    }
}
