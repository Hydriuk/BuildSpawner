using BuildSpawner.API;
using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using System.Drawing;

namespace BuildSpawner.OpenMod.Commands
{
    [Command("listbuilds")]
    [CommandAlias("lbuild")]
    [CommandDescription("List all available build")]
    [CommandActor(typeof(UnturnedUser))]
    public class ListBuildsCommand : UnturnedCommand
    {
        private readonly IBuildManager _buildManager;

        public ListBuildsCommand(IServiceProvider serviceProvider, IBuildManager buildManager) : base(serviceProvider)
        {
            _buildManager = buildManager;
        }

        protected override UniTask OnExecuteAsync()
        {
            string buildingList = _buildManager.ListBuilds();

            Context.Actor.PrintMessageAsync($"Available buildings: {buildingList}", Color.Green);

            return UniTask.CompletedTask;
        }
    }
}