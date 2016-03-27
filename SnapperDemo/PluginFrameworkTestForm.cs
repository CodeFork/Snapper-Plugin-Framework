namespace SnapperDemo
{
    using System.Linq;
    using System.Windows.Forms;
    using Snapper;
    using SnapperDemoPlumbing;
    using TinyIoC;
    using TinyMessenger;

    public partial class PluginFrameworkTestForm : Form
    {
        TinyIoCContainer container = TinyIoCContainer.Current;
        ITinyMessengerHub messengerHub;

        PluginManager<IDataSourcePlugin> pluginManager;

        public PluginFrameworkTestForm()
        {
            InitializeComponent();
            messengerHub = container.Resolve<ITinyMessengerHub>();

            RefreshPlugins();
            messengerHub.Subscribe<NewPluginDetected>((s) => { RefreshPlugins(); }, new SnapperDemoPlumbing.ControlInvokeTinyMessageProxy(this));
        }

        private void RefreshPlugins()
        {
            pluginManager = container.Resolve<PluginManager<IDataSourcePlugin>>("DataSourcePlugin");
            pluginManager
                .FindPluginsInFolder()
                .RefreshUsingFileSystemWatcher((s, e) =>
                {
                    messengerHub.Publish<NewPluginDetected>(new NewPluginDetected());
                })
                .LoadPlugins(x =>
                {
                    BuildUI(x);
                    messengerHub.Subscribe<PluginDatasourceChange>((m) =>
                    {
                        ListBox pluginListBox = GetListBox((IDataSourcePlugin)m.Sender);
                        pluginListBox.BeginUpdate();
                        pluginListBox.DataSource = null;
                        pluginListBox.DataSource = m.Content;
                        pluginListBox.EndUpdate();
                    }, new ControlInvokeTinyMessageProxy(this));
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
