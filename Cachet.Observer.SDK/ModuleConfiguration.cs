using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CachetObserver.SDK
{
    public class ModuleConfiguration
    {
        private readonly JObject _configuration;

        public ModuleConfiguration(JObject configuration)
        {
            _configuration = configuration;
        }

        public T GetConfiguration<T>() where T : class
        {
            return _configuration.ToObject<T>();
        }
    }
}
