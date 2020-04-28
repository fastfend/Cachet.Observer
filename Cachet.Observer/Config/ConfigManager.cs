using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CachetObserver.Extensions;
using System.Linq;
using CachetObserver.Plugins;

namespace CachetObserver.Config
{
    public class ConfigManager : IConfigManager
    {
        private const string cfgFilePath = "config.json";
        private const ConfigVersion cfgVersion = ConfigVersion.v1;

        private readonly ILogger _logger;
        private readonly IPluginManager _pluginManager;

        public Configuration Configuration { get; }

        public ConfigManager(ILoggerFactory loggerFactory, IPluginManager pluginManager)
        {
            _logger = loggerFactory.CreateLogger<ConfigManager>();
            _pluginManager = pluginManager;

            if (File.Exists(cfgFilePath))
            {
                Configuration = Load();
            }
            else
            {
                Create();
                Configuration = Load();
            }
        }

        private void Create()
        {
            _logger.LogDebug("Creating config file");
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };

            using (StreamWriter sw = new StreamWriter(cfgFilePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                _logger.LogDebug("Creating default config object");
                Configuration defaultConfig = Configuration.GetDefaultConfig();
                _logger.LogDebug("Serializing config file");
                serializer.Serialize(writer, defaultConfig);
            }
        }

        private ConfigVersion GetConfigVersion()
        {
            try
            {
                _logger.LogDebug("Reading JSON file");
                string json = File.ReadAllText(cfgFilePath);

                _logger.LogDebug("Parsing JSON file");
                JObject jsonobject = JObject.Parse(json);

                _logger.LogDebug("Getting version from JObject");
                string version = jsonobject.Value<string>("Version");

                _logger.LogDebug("Parsing version from: {0}", version.NullableToString());

                ConfigVersion versionenum = (ConfigVersion)Enum.Parse(typeof(ConfigVersion), version);
                if (!Enum.IsDefined(typeof(ConfigVersion), versionenum) && !versionenum.ToString().Contains(","))
                    throw new InvalidOperationException($"{version} is not an underlying value of the ConfigVersion enumeration.");

                _logger.LogInformation("Version found: {0}", versionenum.ToString());
                return versionenum;
            }
            catch (Exception e)
            {
                _logger.LogError("Couldn't read version of config");
                _logger.LogTrace(e.Message);
                return ConfigVersion.Unknown;
            }

        }

        private Configuration Load()
        {
            //TODO: Verify JSON Schema
            switch(GetConfigVersion())
            {
                case ConfigVersion.v1:
                    return LoadNewestVersion();
                case ConfigVersion.Unknown:
                    _logger.LogError("Unknown version of config file");
                    return null;
            }
            _logger.LogCritical("Unknown error when loading config file");
            return null;
        }

        private bool ContainsDuplicateComponents(Configuration configuration)
        {
            List<int> temp_list = new List<int>();
            foreach (var item in configuration.ObservedServices)
            {
                temp_list.Add(item.CachetComponentID);
            }

            if (temp_list.Count != temp_list.Distinct().Count())
            {
                return true;
            }
            return false;
        }

        private Configuration LoadNewestVersion()
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader(cfgFilePath))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                _logger.LogDebug("Deserializing config file");
                Configuration configuration = serializer.Deserialize<Configuration>(reader);
                if(ContainsDuplicateComponents(configuration))
                {
                    _logger.LogCritical("Config file contains duplicates of component IDs");
                    return null;
                }

                LogConfig(configuration);
                return configuration;
            }
        }

        private void LogConfig(Configuration configuration)
        {
            string firstConfigText = "Configuration:"
                                + Environment.NewLine
                                + " Version: " + configuration.Version.ToString()
                                + Environment.NewLine
                                + " Cachet Address: " + configuration.CachetAddress.NullableToString()
                                + Environment.NewLine
                                + " API Key (first 3 letters): " + configuration.API_key.TrySubstring(0, 3).NullableToString();

            foreach (var item in configuration.ObservedServices)
            {
                firstConfigText = firstConfigText
                    + Environment.NewLine
                    + " Observed Services:"
                    + Environment.NewLine
                    + "  [" + item.CachetComponentID.ToString() + "]:";
                foreach(var check in item.Checks)
                {
                    firstConfigText = firstConfigText
                        + Environment.NewLine
                        + "   Module Name: " + check.ModuleName
                        + Environment.NewLine
                        + "   Priority: " + check.Priority
                        + Environment.NewLine
                        + "   Interval: " + check.Interval;
                }
            }

            _logger.LogDebug(firstConfigText);
        }
    }
}
