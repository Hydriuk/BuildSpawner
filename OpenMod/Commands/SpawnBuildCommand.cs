using BuildSpawner.API;
using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using UnityEngine;

namespace BuildSpawner.OpenMod.Commands
{
    [Command("spawnbuild")]
    [CommandAlias("sbuild")]
    [CommandSyntax("<name> [<shiftX> <shiftY> <shiftZ>] [-origin | -o]")]
    [CommandDescription("Spawn a build.")]
    [CommandActor(typeof(UnturnedUser))]
    public class SpawnBuildCommand : UnturnedCommand
    {
        private readonly IBuildManager _buildManager;

        public SpawnBuildCommand(IServiceProvider serviceProvider, IBuildManager buildManager) : base(serviceProvider)
        {
            _buildManager = buildManager;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            if (Context.Parameters.Length != 1 && Context.Parameters.Length != 2 && Context.Parameters.Length != 4)
                throw new CommandWrongUsageException(Context);

            string buildName = Context.Parameters[0];

            if (Context.Parameters.Length == 2)
            {
                if (!_buildManager.PlaceBuild(buildName, user.SteamId.m_SteamID, user.Player.Player.channel.owner.playerID.group.m_SteamID))
                    throw new UserFriendlyException($"{buildName} does not exist");

                return UniTask.CompletedTask;
            }

            Vector3 shift;
            if (Context.Parameters.Length == 4)
            {
                if (!float.TryParse(Context.Parameters[1], out float shiftX) ||
                    !float.TryParse(Context.Parameters[2], out float shiftY) ||
                    !float.TryParse(Context.Parameters[3], out float shiftZ))
                {
                    throw new CommandWrongUsageException("Shifts must be numbers");
                }

                shift = new Vector3(shiftX, shiftY, shiftZ);
            }
            else
            {
                shift = Vector3.zero;
            }

            bool buildFound = _buildManager.PlaceBuild(
                buildName,
                user.Player.Player.transform.position,
                user.Player.Player.transform.rotation,
                shift,
                user.SteamId.m_SteamID,
                user.Player.Player.channel.owner.playerID.group.m_SteamID
            );

            if (!buildFound)
                throw new UserFriendlyException($"{buildName} does not exist");

            return UniTask.CompletedTask;
        }
    }
}