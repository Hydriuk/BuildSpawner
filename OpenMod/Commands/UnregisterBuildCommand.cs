using BuildSpawner.API;
using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSpawner.OpenMod.Commands
{
    [Command("unregisterbuild")]
    [CommandAlias("unrbuild")]
    [CommandSyntax("<name>")]
    [CommandDescription("Removes a build. Cannot be undone.")]
    [CommandActor(typeof(UnturnedUser))]
    public class UnregisterBuildCommand : UnturnedCommand
    {
        private readonly IBuildStore _buildStore;

        public UnregisterBuildCommand(IServiceProvider serviceProvider, IBuildStore buildStore) : base(serviceProvider)
        {
            _buildStore = buildStore;
        }

        protected override UniTask OnExecuteAsync()
        {
            if (Context.Parameters.Length != 1)
                throw new CommandWrongUsageException(Context);

            if (!_buildStore.RemoveBuild(Context.Parameters[0]))
                throw new CommandWrongUsageException($"{Context.Parameters[0]} does not exist");

            return UniTask.CompletedTask;
        }
    }
}
