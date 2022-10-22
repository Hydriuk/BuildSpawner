#if OPENMOD
using OpenMod.API.Ioc;
#endif
namespace BuildSpawner.API
{
#if OPENMOD
    [Service]
#endif
    public interface IEnvironmentProvider
    {
        string PluginDirectory { get; }
    }
}
