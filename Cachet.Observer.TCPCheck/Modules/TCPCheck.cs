using CachetObserver.BasicChecks.Modules.TCPCheck;
using CachetObserver.SDK;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CachetObserver.Plugin.BasicChecks.Modules
{
    public class TCPCheck : PluginModule<TCPCheckConfiguration>
    {

        public TCPCheck(ModuleConfiguration configuration, ILogger logger) : base(configuration, logger)
        {

        }

        public override string ModuleName => "TCPCheck";

        public override ModuleJobResult Run()
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(Configuarion.Address, Configuarion.Port);
                    return ModuleJobResult.Pass;
                }
                catch (Exception)
                {
                    return ModuleJobResult.Fail;
                }
            }
        }
    }
}
