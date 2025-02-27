namespace Yaoc.Plugins.Parsers;

public interface IFileParserPlugin : IPluginBase {

    string[] Extensions { get; }

    Task<string> Parse(string filePath);
    string GetFileFilters();
}
