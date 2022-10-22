using BuildSpawner.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSpawner.RocketMod.Services
{
    public class EnvironmentProvider : IEnvironmentProvider
    {
        public string PluginDirectory { get; }

        public EnvironmentProvider(Plugin plugin)
        {
            PluginDirectory = plugin.Directory;
        }
    }
}
