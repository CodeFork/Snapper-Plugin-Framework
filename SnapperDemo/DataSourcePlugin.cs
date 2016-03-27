namespace SanpperDemo
{
    using System.Collections.Generic;
    using Snapper;
    using SnapperDemo;

    internal class DataSourcePlugin : Plugin, IDataSourcePlugin
    {
        public List<string> DataSource { get; private set; }
    }
}
