namespace SanpperDemo
{
    using System;
    using System.Windows.Forms;
    using Snapper;
    using SnapperDemo;
    using TinyIoC;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TinyIoCContainer container = TinyIoCContainer.Current;
            container.Register<PluginManager<IDataSourcePlugin>>("DataSourcePlugin").AsSingleton();
            container.Register<PluginManager<IPlugin>>("Plugin").AsSingleton();

            Application.Run(new SnapperDemoForm());
        }
    }
}
