namespace Snapper
{
    using System;
    using System.Reflection;

    public class Plugin : IPlugin
    {
        public Assembly Assembly { get; set; }

        public bool Loaded { get; set; }

        public string Name { get; set; }

        public object PluginInstance { get; set; }

        public Type Type { get; set; }
    }
}
