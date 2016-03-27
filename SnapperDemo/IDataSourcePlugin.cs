namespace SnapperDemo
{
    using System.Collections.Generic;
    using Snapper;

    public interface IDataSourcePlugin : IPlugin
    {
        List<string> DataSource { get; }
    }
}