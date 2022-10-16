using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSpawner.RocketMod
{
    public class Plugin : RocketPlugin
    {
        public static Plugin Instance { get; set; }

        protected override void Load()
        {
            Instance = this;
        }
    }
}
