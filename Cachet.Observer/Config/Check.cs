using Newtonsoft.Json.Linq;

namespace CachetObserver.Config
{
    public class Check
    {
        public string ModuleName;
        public int Interval;
        public int Priority;
        public JObject ModuleConfiguration;
    }
}
