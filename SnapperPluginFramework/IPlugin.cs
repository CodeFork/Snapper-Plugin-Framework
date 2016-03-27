namespace Snapper
{
    using System;
    using System.Reflection;

    public interface IPlugin
    {
        Assembly Assembly { get; }

        bool Loaded { get; set; }

        string Name { get; }

        object PluginInstance { get; }

        Type Type { get; }
    }
}