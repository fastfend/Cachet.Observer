using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace CachetObserver.SDK
{
    public abstract class PluginModule<T> : IPluginModule where T : class
    {
        public abstract string ModuleName { get; }

        public readonly T Configuarion;
        public readonly ILogger Logger;

        public PluginModule(ModuleConfiguration configuration, ILogger logger)
        {
            Configuarion = configuration.GetConfiguration<T>();
        }

        public abstract ModuleJobResult Run();
    }
}
