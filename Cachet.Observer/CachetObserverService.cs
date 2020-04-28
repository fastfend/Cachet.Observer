using System;
using System.Collections.Generic;
using System.Text;
using Cachet.NET;
using Cachet.NET.Responses;
using Cachet.NET.Responses.Objects;
using CachetObserver.Checker;
using CachetObserver.Config;
using Microsoft.Extensions.Logging;

namespace CachetObserver
{
    class CachetObserverService : ICachetObserverService
    {
        private readonly CachetServer CachetServerInstance;
        private readonly ILogger Logger;
        private readonly IConfigManager ConfigManager;

        private bool keyVerifed = false;
        private bool online = false;

        public bool KeyVerifed { get => keyVerifed; }
        public bool Online { get => online; }

        public CachetObserverService(ILoggerFactory loggerfactory, IConfigManager configManager)
        {
            Logger = loggerfactory.CreateLogger("CachetObserverInstance");
            Logger.LogInformation("Initializing CachetObserver");
            ConfigManager = configManager;

            CachetServerInstance = new CachetServer(ConfigManager.Configuration.CachetAddress + "/api/v1/", ConfigManager.Configuration.API_key);

            keyVerifed = VerifedAPIKey();
            online = GetCachetAPIStatus();
            CheckComponents();
        }

        private bool VerifedAPIKey()
        {
            try
            {
                var x = CachetServerInstance.NewComponent("APIverification", "", ComponentStatus.Operational, "", 0, 0, false);
                if(x != null)
                {
                    CachetServerInstance.DeleteComponent(x.Id);
                }
                else
                {
                    return false;
                }
                
            }
            catch (UnauthorizedException)
            {
                return false;
            }
            
            return true;
        }

        private void CheckComponents()
        {
            foreach (var item in ConfigManager.Configuration.ObservedServices)
            {
                try
                {
                    var x = CachetServerInstance.GetComponent(item.CachetComponentID);
                    Logger.LogInformation("Successfully found component with ID {0} directing to {1}", item.CachetComponentID, x.Name);
                }
                catch
                {
                    Logger.LogError("There is no component in Cachet with ID {0}", item.CachetComponentID);
                }
            }
        }

        private bool GetCachetAPIStatus()
        {
            Logger.LogDebug("Pinging Cachet API");
            var response = CachetServerInstance.Ping();
            Logger.LogDebug("Ping status: " + response.ToString());

            online = response;

            return response;
        }

        public void RunChecks()
        {
            //throw new NotImplementedException();
        }

        public void SetMetrcs()
        {
            //throw new NotImplementedException();
        }

        public void ReportIncidents()
        {
            if(Online)
            {
                Logger.LogInformation("Incidents");
                var x = CachetServerInstance.GetIncidents().Data;
                foreach(var item in x)
                {
                    Logger.LogInformation("ID: " + item.Id);
                }
            }
            //throw new NotImplementedException();
        }
    }
}
