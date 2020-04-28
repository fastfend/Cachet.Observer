using CachetObserver.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CachetObserver.Plugins
{
    public interface IPluginManager
    {
        ReadOnlyCollection<IPlugin> PluginsLoaded { get; }
        ReadOnlyCollection<Type> ModulesLoaded { get; }
    }
}
