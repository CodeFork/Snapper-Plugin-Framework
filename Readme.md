## Welcome to Snapper Plugin Framework &nbsp; &nbsp; &nbsp; ![](http://i.imgur.com/f9sKJ4r.png)

### Overview ###

Welcome to the Snapper Plugin Framework which provides boilerplate code for creating a snap-in/plugin style architecture; Snapper leaves you to focus on your code and architecture.

Snapper is minimalist, providing just enough framework to ease the burden of loading plugins with minimal complexity.

### Key Features ###

* Loads plugin assemblies and creates each plugin automatically.
* Allows searching for plugins in a named directory - defaults to the application directory.
* [Optional] Allows loading of new plugins via setting a FileSystemWatch and loads new plugin assemblies at runtime.
* Provides a list of plugins that are loaded.

### Show me the code ###

	// create the plugin manager using TinyIOC.
    pluginManager = container.Resolve<PluginManager<IDataSourcePlugin>>("DataSourcePlugin");
    pluginManager
       .FindPluginsInFolder("MyPluginFolder")
       .LoadPlugins(plugin =>
        {
            // initialize the plugin
        })
       .RefreshUsingFileSystemWatcher((sender, eventHandler) =>
        {
            // Refresh the plugin list, 
            // adding any new plugins which may have been added.
        });

In this short example:
* We use the TinyIOC container to create our plugin.
* Call FindPluginsInFolders looking for plugin assemblies in the "MyPluginFolder".
* Call LoadPlugins passing an Action to create and initialize the plugins.
* Call RefreshUsingFileSystemWatcher to load plugins at runtime.

### To see a fully functional demo see the SnapperDemo project. ###
----------
***Snapper*** comes from a snapping turtle that resided in a local pond where I lived as a youth.

turtle by Christy Presler from the Noun Project

