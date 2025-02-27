using Microsoft.Extensions.Configuration;
using System.Reflection;
using Yaoc.Plugins;
using Yaoc.Plugins.Parsers;

namespace Yaoc.Core.Plugins;

public static class PluginsLoader {
    private static IConfiguration _configuration;

    static Dictionary<string, IFileParserPlugin> _parserPlugins = new();

    public static void Initialize(IConfiguration configuration) {
        _configuration = configuration;
        LoadFileParserPlugins();
    }

    public static Dictionary<string, IFileParserPlugin> GetFileParserPlugins() {
        return _parserPlugins;
    }

    private static void LoadFileParserPlugins() {
        var pluginsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configuration["AppSettings:PluginsFolder"]);
        var pluginsPath = Directory.GetFiles(pluginsFolder, "*.dll");

        foreach (var path in pluginsPath) {
            var plugin = LoadPlugin<IFileParserPlugin>(path);

            _parserPlugins.Add(plugin.Id, plugin);
        }
    }

    private static T LoadPlugin<T>(string pluginPath) where T : class, IPluginBase {
        var loadContext = new PluginLoadContext(pluginPath);
        var assembly = loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(pluginPath));
        
        foreach (Type type in assembly.GetTypes()) {
            if (typeof(T).IsAssignableFrom(type)) {
                T plugin = Activator.CreateInstance(type) as T;

                return plugin;
            }
        }
        return null;
    }
}
