namespace Snapper
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Class PluginManager. 
    /// </summary>
    /// <typeparam name="T">The expected interface that this plugin implements; 
    /// must be an interface and must descend from or implement <see cref="IPlugin"/>.</typeparam>
    /// <remarks>Note: checking that T is an interface is done at runtime.</remarks>
    public class PluginManager<T>
        where T : class, IPlugin
    {
        private FileSystemWatcher fileSystemWatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManager{T}"/> class.
        /// </summary>
        public PluginManager()
        {
            PluginList = new List<IPlugin>();
        }

        /// <summary>
        /// Gets the folder where the plugins reside.
        /// </summary>
        /// <value>The folder where the plugins reside.</value>
        public string PluginFolder { get; private set; }

        /// <summary>
        /// Gets the list of loaded plugins.
        /// </summary>
        /// <value>The list of plugin that have been loaded into this plugin manager.</value>
        public List<IPlugin> PluginList { get; private set; }

        /// <summary>
        /// Finds the plugins in the supplied folder.
        /// </summary>
        /// <param name="pluginFolder">The plugin folder in which to search for plugins.</param>
        /// <returns>PluginManager&lt;T&gt; for fluent API.</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public PluginManager<T> FindPluginsInFolder(string pluginFolder = null)
        {
            if (Directory.Exists(pluginFolder))
                PluginFolder = pluginFolder;
            else
            {
                string assemblyFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                if (String.IsNullOrEmpty(pluginFolder))
                    PluginFolder = assemblyFolder;
                else
                {
                    string tempFolder = Path.Combine(assemblyFolder, pluginFolder);
                    if (Directory.Exists(tempFolder))
                        PluginFolder = tempFolder;
                }
            }
            if (String.IsNullOrWhiteSpace(PluginFolder))
                throw new DirectoryNotFoundException(String.Format("Could not find or derive Plugin Folder: {0}", pluginFolder));
            return this;
        }

        /// <summary>
        /// Loads and initializes the plugins that have been found.
        /// </summary>
        /// <param name="initializePlugin">The <see cref="Action<T>"/> which is run to initialize the plugins as they are loaded.</param>
        /// <returns>PluginManager&lt;T&gt; for fluent API.</returns>
        /// <exception cref="BadImageFormatException"></exception>
        /// <exception cref="FileLoadException"></exception>
        public PluginManager<T> LoadPlugins(Action<T> initializePlugin)
        {
            string[] files = Directory.GetFiles(PluginFolder, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                try
                {
                    if (!AssemblyLoaded(file))
                    {
                        Assembly assembly = Assembly.LoadFile(file);
                        if (assembly != null)
                        {
                            LoadAssembly(initializePlugin, assembly);
                        }
                    }
                }
                catch (BadImageFormatException ex)
                {
                    // log exception
                }
                catch (FileLoadException ex)
                {
                    // log exception
                }
            }

            return this;
        }

        /// <summary>
        /// Checks for new plugins when a file is added to the watched directory.
        /// </summary>
        /// <param name="eventHandler">The <see cref="FileSystemEventArgs"/> event handler.</param>
        /// <returns>PluginManager&lt;T&gt; for fluent API.</returns>
        public PluginManager<T> RefreshUsingFileSystemWatcher(FileSystemEventHandler eventHandler)
        {
            if (fileSystemWatcher == null)
            {
                fileSystemWatcher = new FileSystemWatcher(PluginFolder)
                {
                    NotifyFilter = NotifyFilters.FileName,
                    Filter = "*.dll",
                    EnableRaisingEvents = true
                };

                fileSystemWatcher.Created += eventHandler;
            }
            return this;
        }

        private void AddToPluginList(Action<T> initializePlugin, Assembly assembly, ConstructorInfo constructor)
        {
            object obj = constructor.Invoke(Type.EmptyTypes);
            initializePlugin((T)obj);
            IPlugin pluginSource = obj as IPlugin;
            IPlugin plugin = new Plugin()
            {
                Name = pluginSource.Name,
                Type = pluginSource.Type,
                Assembly = assembly,
                PluginInstance = obj,
            };
            PluginList.Add(plugin);
            plugin.Loaded = true;
        }

        private bool AssemblyLoaded(string file)
        {
            IEnumerable<IPlugin> plugins = from p in PluginList where p.Assembly.Location.ToLower() == file.ToLower() select p;
            return plugins.Count<IPlugin>() > 0;
        }

        private void LoadAssembly(Action<T> initializePlugin, Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(T).IsAssignableFrom(type) && typeof(IPlugin).IsAssignableFrom(type))
                {
                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor != null)
                    {
                        AddToPluginList(initializePlugin, assembly, constructor);
                    }
                }
            }
        }
    }
}
