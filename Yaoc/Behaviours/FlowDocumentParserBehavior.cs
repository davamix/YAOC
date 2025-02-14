using Microsoft.Xaml.Behaviors;
using OllamaSharp.Models.Chat;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Yaoc.Behaviours;

public class FlowDocumentParserBehavior : Behavior<RichTextBox> {
    protected override void OnAttached() {
        base.OnAttached();

        AssociatedObject.Loaded += AssociatedObject_Loaded;
        //AssociatedObject.Initialized += AssociatedObject_Initialized;
    }



    protected override void OnDetaching() {
        base.OnDetaching();

        AssociatedObject.Loaded -= AssociatedObject_Loaded;
        //AssociatedObject.Initialized -= AssociatedObject_Initialized;
    }

    private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e) {
        var rtb = sender as RichTextBox;

        if (rtb != null) {
            rtb.IsReadOnly = true;

            var context = rtb.DataContext as Message;

            if (context != null) {
                var document = new FlowDocument();

                rtb.Background = GetRoleColor(context.Role);

                var paragraph = CreateParagraph(context);
                document.Blocks.Add(paragraph);

                rtb.Document = document;
            }
        }
    }

    private Brush GetRoleColor(ChatRole? role) {
        if (role == ChatRole.System)
            return new SolidColorBrush(Colors.LightGray);

        if (role == ChatRole.User)
            return new SolidColorBrush(Colors.LightGreen);

        if (role == ChatRole.Assistant)
            return new SolidColorBrush(Colors.LightBlue);

        return Brushes.Transparent;

    }

    private Paragraph CreateParagraph(Message message) {
        var p = ParagraphFactory.Create(message.Role);
        p.Configure(message.Content);

        return p as Paragraph;
    }
}

public static class ParagraphFactory {
    public static IParagraph Create(ChatRole? role) {
        if (role == ChatRole.System) return new SystemParagraph();
        if (role == ChatRole.User) return new UserParagraph();
        if (role == ChatRole.Assistant) return new AssistantParagraph();

        return new EmptyParagraph();
    }
}

public interface IParagraph {
    void Configure(string message);
}

public class SystemParagraph : Paragraph, IParagraph {
    public void Configure(string message) {
        var r = new Italic(new Run(message));
        this.Inlines.Add(r);
    }
}

public class UserParagraph : Paragraph, IParagraph {
    public void Configure(string message) {
        var r = new Run(message);
        this.Inlines.Add(r);
    }
}

public class AssistantParagraph : Paragraph, IParagraph {
    public void Configure(string message) {

        if (HasCode(message)) {
            var messageParts = new List<string>();

            var pattern = @"`{3}(.*\n)([^~]*?)`{3}";
            var matches = Regex.Matches(message, pattern);
            int index = 0;

            foreach (Match m in matches) {
                if(index < m.Index) {
                    this.Inlines.Add(new Run(message.Substring(index, m.Index - index)));
                    this.Inlines.Add(CreateCodeSpan(m.Groups[2].Value));

                    index = m.Groups[2].Index + m.Groups[2].Length + 3;
                }
            }
        } else {
            var r = new Run(message);
            this.Inlines.Add(r);
        }
    }

    private bool HasCode(string value) {
        var patter = @"```*[\n]+";
        var match = Regex.Match(value, patter);

        return match.Success;
    }


    private Span CreateCodeSpan(string value) {
        //var codeSpan = new InlineUIContainer(new RichTextBox() {
        //    Document = new FlowDocument(new Paragraph(new Run(value))),
        //    FontFamily = new FontFamily("Consolas"),
        //    Background = new SolidColorBrush(Colors.WhiteSmoke),
        //    Width = Double.NaN

        //});

        Style spanStyle = new Style(typeof(Span));
        //spanStyle.Setters.Add(new Setter(BorderThicknessProperty, new Thickness(3)));
        //spanStyle.Setters.Add(new Setter(BorderBrushProperty, new SolidColorBrush(Colors.Khaki)));
        //spanStyle.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Colors.WhiteSmoke)));

        var codeSpan = new Span(new Run(value)) {
            FontFamily = new FontFamily("Consolas"),
            OverridesDefaultStyle =true,
            Style = spanStyle
        };

        return codeSpan;
    }
}

public class EmptyParagraph : Paragraph, IParagraph {
    public void Configure(string message) {
        var r = new Run(message);
        this.Inlines.Add(r);
    }
}