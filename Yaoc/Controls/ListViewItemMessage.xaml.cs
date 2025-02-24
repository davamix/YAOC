using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using OllamaSharp.Models.Chat;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Yaoc.Core.Models;

namespace Yaoc.Controls;
/// <summary>
/// Interaction logic for ListViewItemMessage.xaml
/// </summary>
public partial class ListViewItemMessage : ListViewItem {

    private Theme _theme;

    public ListViewItemMessage() {
        InitializeComponent();

        _theme = new PaletteHelper().GetTheme();
    }

    private void ListViewItem_Loaded(object sender, RoutedEventArgs e) {
        var control = sender as ListViewItemMessage;

        if (control != null) {
            var context = control.DataContext as ChatMessage;

            if (context != null) {
                var lvimTemplate = CreateListViewItemMessageTemplate();
                var messageParts = ExtractMessageParts(context.OllamaMessage.Content);
                var messageBlock = CreateMessageBlock(messageParts, context.OllamaMessage.Role);

                this.Template = lvimTemplate;
                this.Content = messageBlock;
            }
        }
    }

    private ControlTemplate CreateListViewItemMessageTemplate() {
        ControlTemplate template = new ControlTemplate(typeof(ListViewItemMessage));
        FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));

        borderFactory.SetValue(Border.BorderBrushProperty, Brushes.LightGray);
        borderFactory.SetValue(Border.BackgroundProperty, Brushes.Transparent);
        borderFactory.SetValue(Border.MarginProperty, new Thickness(5, 10, 5, 10));

        borderFactory.AppendChild(new FrameworkElementFactory(typeof(ContentPresenter)));
        template.VisualTree = borderFactory;

        return template;
    }

    private IEnumerable<ContentMessage> ExtractMessageParts(string? content) {
        if (string.IsNullOrEmpty(content)) return Enumerable.Empty<ContentMessage>();

        var contentMessages = new List<ContentMessage>();

        if (HasCode(content)) {
            // If content message contains code inside, use regex to split regular text from code
            var messageParts = new List<string>();

            var pattern = @"`{3}(.*\n)([^~]*?)`{3}";
            var matches = Regex.Matches(content, pattern);
            int index = 0;

            // Based on the regex result, process the regular text and the code text
            foreach (Match m in matches) {
                if (index < m.Index) {
                    // Regular text
                    contentMessages.Add(new ContentMessage {
                        Message = content.Substring(index, m.Index - index),
                        IsCode = false
                    });

                    // Code text
                    contentMessages.Add(new ContentMessage {
                        Message = m.Groups[2].Value,
                        IsCode = true
                    });

                    index = m.Groups[2].Index + m.Groups[2].Length + 3;
                }
            }

            // Once finished the regex result processing, continue with the rest of the text (only regular text)
            contentMessages.Add(new ContentMessage {
                Message = content.Substring(index),
                IsCode = false
            });
        } else {
            // If content message has no code, just add regular text
            contentMessages.Add(new ContentMessage {
                Message = content,
                IsCode = false
            });
        }

        return contentMessages;
    }

    private UIElement CreateMessageBlock(IEnumerable<ContentMessage> messages, ChatRole? role) {
        var mainStack = new StackPanel();

        bool hasTopBorder = true;

        foreach (var cm in messages) {
            var rtb = new RichTextBox(
                new FlowDocument(CreateMessageParagraph(role, hasTopBorder, cm))) {

                Style = CreateRichTextBoxStyle(role, cm)
            };

            mainStack.Children.Add(rtb);
            hasTopBorder = false;
        }

        return mainStack;
    }

    private Paragraph CreateMessageParagraph(ChatRole? role, bool hasTopBorder, ContentMessage cm) {
        return new Paragraph(new Run(RemoveNewLineCharacters(cm.Message))) {
            Style = CreateParagraphStyle(role, hasTopBorder, cm)
        };
    }

    private Style CreateParagraphStyle(ChatRole? role, bool hasTopBorder, ContentMessage cm) {
        var paragraphStyle = new Style(typeof(Paragraph));
        paragraphStyle.Setters.Add(new Setter(Paragraph.FontFamilyProperty, cm.IsCode ? new FontFamily("Consolas") : this.FontFamily));
        paragraphStyle.Setters.Add(new Setter(Paragraph.BackgroundProperty, GetParagraphColor(role, cm.IsCode)));
        paragraphStyle.Setters.Add(new Setter(Paragraph.PaddingProperty, cm.IsCode ? new Thickness(20) : new Thickness(5, 10, 5, 10)));
        paragraphStyle.Setters.Add(new Setter(Paragraph.BorderThicknessProperty, SetMessageBorders(role, hasTopBorder)));
        paragraphStyle.Setters.Add(new Setter(Paragraph.BorderBrushProperty, GetBorderMessageColor(role)));
        return paragraphStyle;
    }

    private Style CreateRichTextBoxStyle(ChatRole? role, ContentMessage cm) {
        var rtbStyle = new Style(typeof(RichTextBox));
        rtbStyle.Setters.Add(new Setter(RichTextBox.BackgroundProperty, GetParagraphColor(role, cm.IsCode)));
        rtbStyle.Setters.Add(new Setter(RichTextBox.BorderThicknessProperty, new Thickness(0)));
        rtbStyle.Setters.Add(new Setter(RichTextBox.MarginProperty, new Thickness(0, -1, 0, 0)));
        rtbStyle.Setters.Add(new Setter(RichTextBox.IsReadOnlyProperty, true));

        return rtbStyle;
    }

    private bool HasCode(string value) {
        var patter = @"```*[\n]+";
        var match = Regex.Match(value, patter);

        return match.Success;
    }

    private Thickness SetMessageBorders(ChatRole? role, bool hasTopBorder) {
        if (role == ChatRole.System) return new Thickness(0);
        if (role == ChatRole.User) return new Thickness(5, hasTopBorder ? 5 : 0, 0, 0);
        if (role == ChatRole.Assistant) return new Thickness(5, hasTopBorder ? 5 : 0, 0, 0);

        return new Thickness(0);
    }
    private Brush GetBorderMessageColor(ChatRole? role) {
        if (role == ChatRole.User)
            return new SolidColorBrush(_theme.SecondaryDark.Color);

        if (role == ChatRole.Assistant)
            return new SolidColorBrush(_theme.PrimaryDark.Color);

        return Brushes.Transparent;
    }
    private Brush GetParagraphColor(ChatRole? role, bool isCode = false) {
        var brush = new SolidColorBrush(Colors.Transparent);

        if (role == ChatRole.System)
            brush.Color = SwatchHelper.Lookup[MaterialDesignColor.BlueGrey200];

        //if (role == ChatRole.User)
        //    brush.Color = Colors.LightGreen;

        //if (role == ChatRole.Assistant)
        //    brush.Color = Colors.LightBlue;

        if (isCode) {
            brush.Color = SwatchHelper.Lookup[MaterialDesignColor.Grey300];
            brush.Opacity = 0.3;
        }

        return brush;
    }

    private string RemoveNewLineCharacters(string message) {
        return message.Trim();
    }
}


public class ContentMessage {
    public string Message { get; set; }
    public bool IsCode { get; set; }
    public ChatRole Role { get; set; }
}