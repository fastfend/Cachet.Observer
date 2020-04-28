using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CachetObserver.SDK
{
    public interface IPlugin
    {
        IPluginInformation PluginInformation { get; }
        ReadOnlyCollection<Type> PluginModules { get; }
    }
}
