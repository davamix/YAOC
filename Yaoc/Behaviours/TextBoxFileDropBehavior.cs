using Microsoft.Xaml.Behaviors;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Yaoc.Core.Plugins;

namespace Yaoc.Behaviours;

public class AttachedResourceNotAllowedEventArgs : EventArgs {
    public string Message { get; set; }

    public AttachedResourceNotAllowedEventArgs(string message) {
        Message = message;
    }
}

public class ResourceAttachedEventArgs : RoutedEventArgs {
    public string ResourcePath { get; set; }

    public ResourceAttachedEventArgs(string resourcePath) {
        ResourcePath = resourcePath;
    }
}

public class TextBoxFileDropBehavior:Behavior<TextBox>
{
    public event EventHandler<AttachedResourceNotAllowedEventArgs> AttachedResourceNotAllowed;
    public event EventHandler<ResourceAttachedEventArgs> ResourceAttached;

    protected override void OnAttached() {
        base.OnAttached();

        AssociatedObject.Loaded += AssociatedObject_Loaded;
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        AssociatedObject.Loaded -= AssociatedObject_Loaded;
    }

    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {
        var textBox = sender as TextBox;

        if (textBox != null) {
            textBox.PreviewDragOver += TextBox_PreviewDragOver;
            textBox.PreviewDrop += TextBox_PreviewDrop;
        }
    }

    private void TextBox_PreviewDragOver(object sender, DragEventArgs e) {
        e.Handled = true;
    }

    private void TextBox_PreviewDrop(object sender, DragEventArgs e) {
        e.Handled = true;

        var data = e.Data.GetData(DataFormats.FileDrop);

        if (data != null) {
            var resourcePath = ((string[])data)[0];

            if (IsFileFormatAllowed(resourcePath)) {
                RaiseResourceAttached(resourcePath);
            } else {
                RaiseAttachedResourceNotAllowed(Path.GetExtension(resourcePath).ToLower());
            }
        }
    }

    private bool IsFileFormatAllowed(string resourcePath) {
        var allowedFormats = PluginsLoader.GetFileParserPlugins().Values.SelectMany(x => x.Extensions);
        var extension = Path.GetExtension(resourcePath).ToLower();

        return allowedFormats.Contains(extension);
    }

    private void RaiseAttachedResourceNotAllowed(string extension) {
        var message = $"Attached resource format is not allowed ({extension}).\n";
        message += $"Allowed formats are: {string.Join(", ", PluginsLoader.GetFileParserPlugins().Values.SelectMany(x => x.Extensions))}";

        OnAttachedResourceNotAllowed(new AttachedResourceNotAllowedEventArgs(message));
    }

    private void RaiseResourceAttached(string resourcePath) {
        OnResourceAttached(new ResourceAttachedEventArgs(resourcePath));
    }

    protected virtual void OnAttachedResourceNotAllowed(AttachedResourceNotAllowedEventArgs e) {
        AttachedResourceNotAllowed?.Invoke(this, e);
    }

    protected virtual void OnResourceAttached(ResourceAttachedEventArgs e) {
        ResourceAttached?.Invoke(this, e);
    }
}
