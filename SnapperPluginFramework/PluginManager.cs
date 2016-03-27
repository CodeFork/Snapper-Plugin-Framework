namespace Snapper
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class PluginManager<T>
    {
        private FileSystemWatcher fileSystemWatcher;

        public PluginManager()
        {
            PluginList = new List<IPlugin>();
        }

        public string PluginFolder { get; private set; }

        public List<IPlugin> PluginList { get; private set; }

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
                catch (Exception ex)
                {

                }
            }

            return this;
        }

        private bool AssemblyLoaded(string file)
        {
            IEnumerable<IPlugin> plugins = from p in PluginList where p.Assembly.Location.ToLower() == file.ToLower() select p;
            return plugins.Count<IPlugin>() > 0;
        }

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
