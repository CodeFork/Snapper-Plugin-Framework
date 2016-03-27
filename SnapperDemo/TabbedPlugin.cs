namespace SanpperDemo
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Snapper;
    using SnapperDemo;

    internal class TabbedPlugin : IPlugin, IDataSourcePlugin
    {
        public Assembly Assembly { get; private set; }

        public List<string> DataSource { get; private set; }

        public bool Loaded { get; set; }

        public string Name { get; private set; }

        public object PluginInstance { get; private set; }

        public Type Type { get; private set; }
    }
}
