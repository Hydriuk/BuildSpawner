using BuildSpawner.API;
using BuildSpawner.RocketMod.Services;
using BuildSpawner.Services;
using Rocket.Core.Plugins;

namespace BuildSpawner.RocketMod
{
    public class Plugin : RocketPlugin
    {
        public static Plugin Instance { get; set; }

        public IBuildStore BuildStore { get; set; }
        public IThreadAdapter ThreadAdapter { get; set; }
        public IBuildManager BuildManager { get; set; }
        public IEnvironmentProvider EnvironmentProvider { get; set; }

        protected override void Load()
        {
            Instance = this;

            EnvironmentProvider = new EnvironmentProvider(this);
            BuildStore = new BuildStore(EnvironmentProvider);
            ThreadAdapter = new ThreadAdapter();
            BuildManager = new BuildManager(BuildStore, ThreadAdapter);
        }
    }
}