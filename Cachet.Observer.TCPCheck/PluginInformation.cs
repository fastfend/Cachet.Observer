using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CachetObserver.SDK;
namespace CachetObserver.Plugin.BasicChecks
{
    public class PluginInformation : IPluginInformation
    {
        public string Name => "BasicChecks";

        public string Description => "Basic checks for CachetObserver";

        public string Author => "Piotr Stadnicki";

        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public SupportedSDKVersion SupportedSDK => SupportedSDKVersion.v1;
    }
}
