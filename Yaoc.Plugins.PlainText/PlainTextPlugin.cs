using Yaoc.Plugins.Parsers;

namespace Yaoc.Plugins.PlainText;

public class PlainTextPlugin : IFileParserPlugin {
    public string Id { get; }
    public string Name => "Plain Text Plugin";
    public string Description => "Plugin to extract text from plain text files";
    public string[] Extensions => [".txt"];

    public PlainTextPlugin() {
        Id = Guid.NewGuid().ToString();
    }

    public async Task<string> Parse(string filePath) {
        if (!File.Exists(filePath)) {
            throw new FileNotFoundException($"[PlainTextPlugin] File not found: {filePath}");
        }

        if (!Extensions.Contains(Path.GetExtension(filePath))) {
            string message = $"[PlainTextPlugin] File extension not supported: {Path.GetExtension(filePath)}";
            message += $" Supported extensions: {string.Join(", ", Extensions)}";

            throw new NotSupportedException(message);
        }

        var content = $"# FILE: {Path.GetFileName(filePath)}\n";
        content += await File.ReadAllTextAsync(filePath);
        content += "# EOF";

        return content;
    }

    public string GetFileFilters() {
        var filters = string.Empty;

        
        foreach (var extension in Extensions) {
            filters += $"(*{extension})|*{extension}|";
        }

        // Image files (*.bmp, *.jpg)|*.bmp;*.jpg
        return $"Plain Text Files {filters}".TrimEnd('|');
    }
}
