namespace Yaoc.Plugins;

public interface IPluginBase {
    string Id { get; } // Guid
    string Name { get; }
    string Description { get; }
}
