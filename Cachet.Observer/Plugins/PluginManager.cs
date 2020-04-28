using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using CachetObserver.Config;
using CachetObserver.SDK;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CachetObserver.Plugins
{
    class PluginManager : IPluginManager
    {
        private const SupportedSDKVersion ServerSDK = SupportedSDKVersion.v1;

        private readonly List<IPlugin> _pluginsloaded;
        public ReadOnlyCollection<IPlugin> PluginsLoaded { get; }

        private readonly List<Type> _modulesloaded;
        public ReadOnlyCollection<Type> ModulesLoaded { get; }

        private readonly ILoggerFactory _loggerfactory;
        private readonly ILogger _logger;

        public PluginManager(ILoggerFactory loggerFactory)
        {
            _loggerfactory = loggerFactory;
            _logger = _loggerfactory.CreateLogger<PluginManager>();
            _logger.LogInformation("Initializing PluginManager");


            _modulesloaded = new List<Type>();
            ModulesLoaded = _modulesloaded.AsReadOnly();

            _pluginsloaded = new List<IPlugin>();
            PluginsLoaded = _pluginsloaded.AsReadOnly();

            Loaders = new List<PluginLoader>();
            if(IsPluginFolderReady())
            {
                LoadDlls();
                LoadPlugins();
            }
        }

        private const string PluginsFolder = "plugins";
        private readonly List<PluginLoader> Loaders;


		private bool IsPluginFolderReady()
        {
            _logger.LogDebug("Searching for folder");
            var path = PluginsFolder;
            try
            {
                if (Directory.Exists(path))
                {
                    _logger.LogInformation("Plugin folder found");
                    return true;
                }
                else
                {
                    _logger.LogInformation("Plugin folder not found. Attempting to create...");
                    try
                    {
                        Directory.CreateDirectory(path);
                        _logger.LogInformation("Plugin folder created");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Couldn't create plugin folder. Exception: ");
                        _logger.LogError(ex.ToString());
                    }
                }
            }
			catch (Exception ex)
            {
                _logger.LogError("Couldn't access plugin folder. Exception: ");
                _logger.LogError(ex.ToString());
            }
            return false;
        }

        private string GetPluginsFolder()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "plugins";
        }

        private void LoadDlls()
        {
            _logger.LogDebug("Looking for *.dll in plugins folder");
            foreach (var file in Directory.GetFiles(GetPluginsFolder(), "*.dll", SearchOption.TopDirectoryOnly))
            {
                if (File.Exists(file))
                {
                    _logger.LogDebug("Found dll: {0}", file);
                    var loader = PluginLoader.CreateFromAssemblyFile(
                        file,
                        sharedTypes: new[] { typeof(IPlugin) });
                    
                    try
                    {
                        _logger.LogDebug("Adding dll: {0}", file);
                        Loaders.Add(loader);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Failed on adding dll: {0}", file);
                        _logger.LogTrace(ex.Message);
                    }
                }
            }
        }

        private void LoadPlugins()
        {
            foreach (var loader in Loaders)
            {
                try
                {
                    foreach (var pluginType in loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract))
                    {
                        _logger.LogDebug("Trying to add: {0}", pluginType.FullName);
                        IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType, _loggerfactory.CreateLogger(loader.LoadDefaultAssembly().GetName().Name));
                        _logger.LogDebug("Succesfully added plugin {0} to server runtime", Convert.ToString(plugin.PluginInformation.Name));

                        _logger.LogDebug("Checking compatibility with the server");
                        if (plugin.PluginInformation.SupportedSDK == ServerSDK)
                        {
                            _pluginsloaded.Add(plugin);
                            _logger.LogInformation("Loaded plugin {0} version {1}", plugin.PluginInformation.Name, plugin.PluginInformation.Version);

                            if(plugin.PluginModules == null || plugin.PluginModules.Count == 0)
                            {
                                _logger.LogWarning("There is no modules in plugin {0}", plugin.PluginInformation.Name);
                            }
                            else
                            {
                                if(_logger.IsEnabled(LogLevel.Debug))
                                {
                                    foreach (var module in plugin.PluginModules)
                                    {
                                        _logger.LogDebug("Found module {0}", module.Name);
                                    }
                                }

                                _modulesloaded.AddRange(plugin.PluginModules);
                                _logger.LogInformation("Added {0} modules", plugin.PluginModules.Count);
                            }
                        }
                        else
                        {
                            _logger.LogError("Mismatched SDK version (server:{0}) - ({1}:{2})", ServerSDK, plugin.PluginInformation.Name, plugin.PluginInformation.SupportedSDK);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

            }
        }
    }
}
