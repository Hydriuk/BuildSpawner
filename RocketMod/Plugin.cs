using BuildSpawner.Services;
using BuildSpawner.Models;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSpawner.RocketMod.Services;
using BuildSpawner.API;

namespace BuildSpawner.RocketMod
{
    public class Plugin : RocketPlugin
    {
        public static Plugin Instance { get; set; }

        public IBuildStore BuildStore { get; set; }
        public IThreadAdapter ThreadAdapter { get; set; }
        public IBuildManager BuildManager { get; set; }

        protected override void Load()
        {
            Instance = this;

            BuildStore = new BuildStore();
            ThreadAdapter = new ThreadAdapter();
            BuildManager = new BuildManager(BuildStore, ThreadAdapter);
        }
    }
}
