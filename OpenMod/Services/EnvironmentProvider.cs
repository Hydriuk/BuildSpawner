using BuildSpawner.API;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSpawner.OpenMod.Services
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class EnvironmentProvider : IEnvironmentProvider
    {
        public string PluginDirectory { get; }

        public EnvironmentProvider(Plugin plugin)
        {
            PluginDirectory = plugin.WorkingDirectory;
        }
    }
}
