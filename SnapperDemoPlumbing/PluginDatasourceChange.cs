namespace SnapperDemoPlumbing
{
    using TinyMessenger;

    public class PluginDatasourceChange : GenericTinyMessage<object>
    {
        public PluginDatasourceChange(object sender, object content) : base(sender, content) { }
    }
}
