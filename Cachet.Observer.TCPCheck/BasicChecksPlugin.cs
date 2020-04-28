using System;
using System.Collections.Generic;
using CachetObserver.Plugin.BasicChecks.Modules;
using CachetObserver.SDK;
using Microsoft.Extensions.Logging;

namespace CachetObserver.Plugin.BasicChecks
{
    public class BasicChecksPlugin : PluginBase
    {
        private readonly PluginInformation pluginInformation = new PluginInformation();
        public override IPluginInformation PluginInformation => pluginInformation;

        public BasicChecksPlugin(ILogger logger) : base(logger)
        {
            AddModule<TCPCheck>();
        }
    }
}
