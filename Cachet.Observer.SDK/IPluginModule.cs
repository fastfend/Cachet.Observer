using System.Threading.Tasks;

namespace CachetObserver.SDK
{
    public interface IPluginModule
    {
        string ModuleName { get; }

        ModuleJobResult Run(); 
    }
}