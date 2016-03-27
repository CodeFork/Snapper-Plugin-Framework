namespace SnapperDemo
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Snapper;
    using SnapperDemoPlumbing;
    using TinyIoC;
    using TinyMessenger;

    public class PluginZ : IPlugin, IDataSourcePlugin
    {
        private readonly System.Timers.Timer pluginTimer;
        private TinyIoCContainer container;
        private List<string> dataSource = new List<string>();
        private ITinyMessengerHub messengerHub;

        public PluginZ()
        {
            container = TinyIoCContainer.Current;
            messengerHub = container.Resolve<ITinyMessengerHub>();

            pluginTimer = new System.Timers.Timer(10000) { AutoReset = true };
            pluginTimer.Elapsed += plugintimer_Elapsed;
            pluginTimer.Start();
        }

        public Assembly Assembly { get { return GetType().Assembly; } }

        public List<string> DataSource { get { return dataSource; } }

        public bool Loaded { get; set; }

        public string Name { get { return "PluginZ"; } }

        public object PluginInstance { get { return this; } }

        public Type Type { get { return GetType(); } }

        private void plugintimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string value = string.Format("{0} reporting: It is {1} and all is well", Name, DateTime.Now);
            DataSource.Add(value);
            messengerHub.Publish<PluginDatasourceChange>(new PluginDatasourceChange(this, DataSource));
        }
    }
}
