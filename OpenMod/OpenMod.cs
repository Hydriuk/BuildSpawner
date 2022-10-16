using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("BuildSpawner", DisplayName = "BuildSpawner", Author = "Hydriuk")]

namespace BuildSpawner.OpenMod
{
    public class Plugin : OpenModUnturnedPlugin
    {
        public Plugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
