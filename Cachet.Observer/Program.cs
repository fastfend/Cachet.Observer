using System;
using System.Threading;
using CachetObserver.Checker;
using CachetObserver.Config;
using CachetObserver.Extensions;
using CachetObserver.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CachetObserver
{
    class Program
    {
        static void Main(string[] args)
        {
            //Dependency Injection Setup
            var serviceProvider = new ServiceCollection()
                .AddLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .AddSingleton<IConfigManager, ConfigManager>()
                .AddSingleton<ICachetObserverService, CachetObserverService>()
                .AddSingleton<IPluginManager, PluginManager>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            //Plugin Manager
            var pluginManager = serviceProvider.GetService<IPluginManager>();

            //Configuaration
            var configManager = serviceProvider.GetService<IConfigManager>();

            if(configManager.Configuration == null)
            {
                logger.LogCritical("Your config file is invalid");
                Thread.Sleep(1000);
                return;
            }

            if(string.IsNullOrEmpty(Convert.ToString(configManager.Configuration.CachetAddress)) || string.IsNullOrEmpty(configManager.Configuration.API_key))
            {
                logger.LogWarning("You must provide your Cachet site address and API key in config file");
                Thread.Sleep(1000);
                return;
            }

            var checkerManager = new CheckerManager(pluginManager, serviceProvider.GetService<ILoggerFactory>());
            checkerManager.RegisterChecker(configManager.Configuration.ObservedServices[0].Checks[0]);

            //Observer Service
            var observer = serviceProvider.GetService<ICachetObserverService>();

            //Program
            if (observer.Online)
            {
                logger.LogInformation("Successfully connected to provided Cachet API");

                if(observer.KeyVerifed)
                {
                    logger.LogInformation("Cachet API Key verified");

                    while (true)
                    {
                        if (observer.Online)
                        {
                            //observer.ReportIncidents();
                        }
                        else
                        {
                            logger.LogError("Couldn't connect to provided Cachet API. Trying next time...");
                        }

                        Thread.Sleep(new TimeSpan(0, 0, 5));
                    }
                }
                else
                {
                    logger.LogCritical("Provided Cachet API key is invalid. Shutting down...");
                }

            }
            else
            {
                logger.LogCritical("Couldn't connect to provided Cachet API. Shutting down...");
            }
            Thread.Sleep(1000);
        }
    }
}
