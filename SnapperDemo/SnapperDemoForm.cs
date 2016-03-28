namespace SnapperDemo
{
    using System.Linq;
    using System.Windows.Forms;
    using Snapper;
    using SnapperDemoPlumbing;
    using TinyIoC;
    using TinyMessenger;

    public partial class SnapperDemoForm : Form
    {
        TinyIoCContainer container = TinyIoCContainer.Current;
        ITinyMessengerHub messengerHub;

        PluginManager<IDataSourcePlugin> pluginManager;

        public SnapperDemoForm()
        {
            InitializeComponent();
            messengerHub = container.Resolve<ITinyMessengerHub>();

            // Refresh all of the plugins.
            RefreshPlugins();

            // Subscribe to the NewPluginDetected message defining what to do when a new plugin appears.
            messengerHub.Subscribe<NewPluginDetected>((s) => { RefreshPlugins(); }, 
                new ControlInvokeTinyMessageProxy(this)); // ensure that this message is invoked on the UI thread.
        }

        private void UpdateDataSourceInUI(PluginDatasourceChange message)
        {
            ListBox pluginListBox = GetListBox((IDataSourcePlugin)message.Sender);
            pluginListBox.BeginUpdate();
            pluginListBox.DataSource = null;
            pluginListBox.DataSource = message.Content;
            pluginListBox.EndUpdate();
        }
        private void RefreshPlugins()
        {
            pluginManager = container.Resolve<PluginManager<IDataSourcePlugin>>("DataSourcePlugin");
            pluginManager
                //.FindPluginsInFolder("MyPluginFolder")
                .LoadPlugins(plugin =>
                {
                    // build the plugin UI.
                    BuildUI(plugin);
                    // Subscribe to the PluinDataSourceChange message.
                    messengerHub.Subscribe<PluginDatasourceChange>((message) =>
                    {
                        UpdateDataSourceInUI(message);
                    }, new ControlInvokeTinyMessageProxy(this)); // ensure that this message is invoked on the UI thread.
                })
                .RefreshUsingFileSystemWatcher((sender, eventHandler) =>
                {
                    messengerHub.Publish<NewPluginDetected>(new NewPluginDetected());
                });
        }

        private void BuildUI(IDataSourcePlugin x)
        {
            TabPage tabPage = new TabPage(x.Name) { Tag = x.Name };
            tabControl.TabPages.Add(tabPage);
            ListBox listBox = new ListBox();
            tabPage.Controls.Add(listBox);
            listBox.Parent = tabPage;
            x.DataSource.Add(string.Format("Hello everyone from {0}...", x.Name));
            listBox.DataSource = x.DataSource;
            listBox.Dock = DockStyle.Fill;
        }

        private ListBox GetListBox(IDataSourcePlugin plugin)
        {
            TabPage tabPage = (from TabPage tp in tabControl.TabPages where tp.Tag.ToString() == plugin.Name select tp).FirstOrDefault<TabPage>();
            if (tabPage != null)
            {
                Control listBox = (from Control control in tabPage.Controls where control.GetType() == typeof(ListBox) select control).FirstOrDefault<Control>();
                return (ListBox)listBox;
            }
            return null;
        }
    }
}
