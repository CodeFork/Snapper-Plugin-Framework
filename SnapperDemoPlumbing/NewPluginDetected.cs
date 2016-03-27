namespace SnapperDemoPlumbing
{
    using TinyMessenger;

    public class NewPluginDetected : ITinyMessage
    {
        public object Sender { get; set; }
    }
}
