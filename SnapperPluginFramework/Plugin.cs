namespace Snapper
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Class Plugin which implements the minimum plugin information. Can also be used as a plugin base class.
    /// </summary>
    /// <seealso cref="Snapper.IPlugin" />
    public class Plugin : IPlugin
    {
        /// <summary>
        /// Gets the assembly where the plugin was found.
        /// </summary>
        /// <value>The plugin assembly.</value>
        /// <remarks>An assembly may contain multiple plugins.</remarks>
        public Assembly Assembly { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IPlugin" /> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; set; }
        /// <summary>
        /// Gets the plugin name which should be unique.
        /// </summary>
        /// <value>The plugin name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets the plugin instance which is created when the plugin is loaded.
        /// </summary>
        /// <value>The loaded plugin instance.</value>
        public object PluginInstance { get; set; }
        /// <summary>
        /// Gets the type of the plugin.
        /// </summary>
        /// <value>The plugin type.</value>
        public Type Type { get; set; }
    }
}
