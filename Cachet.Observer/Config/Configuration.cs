using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CachetObserver.Config
{
    public class Configuration
    {
        [JsonProperty]
        public ConfigVersion Version { get; private set; }
        [JsonProperty]
        public string CachetAddress { get; private set; }
        [JsonProperty]
        public string API_key { get; private set; }
        [JsonProperty]
        public List<ObservedService> ObservedServices { get; private set; }

        internal static Configuration GetDefaultConfig()
        {
            Configuration config = new Configuration
            {
                Version = ConfigVersion.v1,
                CachetAddress = "",
                API_key = "",
                ObservedServices = new List<ObservedService>
                {
                    new ObservedService()
                    {
                        CachetComponentID = 0,
                        Checks = new List<Check>
                        {
                            new Check
                            {
                                Interval = 5,
                                ModuleName = "",
                                Priority = 0
                            }
                        }
                    }
                }
                
            };

            return config;
        }
    }
}
