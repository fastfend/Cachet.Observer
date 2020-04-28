using CachetObserver.Config;
using CachetObserver.Plugins;
using CachetObserver.SDK;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CachetObserver.Checker
{
    public class CheckerManager
    {
        private readonly IPluginManager _pluginManager;
        private readonly ILoggerFactory _loggerfactory;
        private readonly ILogger _logger;

        private readonly List<Checker> Checkers;

        public CheckerManager(IPluginManager pluginManager, ILoggerFactory loggerfactory)
        {
            _pluginManager = pluginManager;
            _loggerfactory = loggerfactory;
            _logger = loggerfactory.CreateLogger<CheckerManager>();

            Checkers = new List<Checker>();
        }

        public void RegisterChecker(Check check)
        {
            foreach(Type type in _pluginManager.ModulesLoaded)
            {
                if(check.ModuleName == type.Name)
                {
                    var x = type.GetConstructors();
                    IPluginModule pluginModule = (IPluginModule)Activator.CreateInstance(type, new ModuleConfiguration(check.ModuleConfiguration), _loggerfactory.CreateLogger(check.ModuleName));
                    Checker checker = new Checker(pluginModule);
                    _logger.LogInformation(pluginModule.Run().ToString());
                }
                
            }
        }
    }
}
