using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CachetObserver.SDK
{
    public abstract class PluginBase : IPlugin
    {
        private readonly ILogger Logger;
        public abstract IPluginInformation PluginInformation { get; }

        private readonly List<Type> _pluginmodules;
        public ReadOnlyCollection<Type> PluginModules { get; }

        public PluginBase(ILogger logger)
        {
            Logger = logger;
            _pluginmodules = new List<Type>();
            PluginModules = _pluginmodules.AsReadOnly();
        }

        protected bool AddModule<T>() where T : IPluginModule
        { 
            Logger.LogDebug("Attempting to add module {0}");
            if (_pluginmodules.Contains(typeof(T)))
            {
                Logger.LogError("Could not add same module twice");
                return false;
            }

            _pluginmodules.Add(typeof(T));

            return true;
        }
    }
}
