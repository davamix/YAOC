﻿using System.Reflection;
using System.Runtime.Loader;

namespace Yaoc.Core.Plugins;

public class PluginLoadContext : AssemblyLoadContext {

    private AssemblyDependencyResolver _resolver;
    public PluginLoadContext(string pluginPath) {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }
    protected override Assembly? Load(AssemblyName assemblyName) {
        string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        
        if (assemblyPath != null) {
            return base.LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }
    //protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) {
    //    string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
    //    if (libraryPath != null) {
    //        return LoadUnmanagedDllFromPath(libraryPath);
    //    }
    //    return IntPtr.Zero;
    //}
}
