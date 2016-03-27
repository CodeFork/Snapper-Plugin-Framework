namespace Snapper
{
    using System;
    using System.Reflection;

    /// <summary>
    /// IPlugin defines the minimal information that a Snapper plugin requires.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets the assembly where the plugin was found.
        /// </summary>
        /// <value>The plugin assembly.</value>
        /// <remarks>An assembly may contain multiple plugins.</remarks>
        Assembly Assembly { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IPlugin"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        bool Loaded { get; set; }

        /// <summary>
        /// Gets the plugin name which should be unique.
        /// </summary>
        /// <value>The plugin name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the plugin instance which is created when the plugin is loaded.
        /// </summary>
        /// <value>The loaded plugin instance.</value>
        object PluginInstance { get; }

        /// <summary>
        /// Gets the type of the plugin.
        /// </summary>
        /// <value>The plugin type.</value>
        Type Type { get; }
    }
}