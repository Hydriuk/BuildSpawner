using BuildSpawner.API;
using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using UnityEngine;

namespace BuildSpawner.OpenMod.Commands
{
    [Command("registerbuild")]
    [CommandAlias("rbuild")]
    [CommandSyntax("<name> <sizeX> <sizeY> <sizeZ> [shiftX] [shiftY] [shiftZ]")]
    [CommandDescription("Registers a build.")]
    [CommandActor(typeof(UnturnedUser))]
    public class RegisterBuildCommand : UnturnedCommand
    {
        private readonly IBuildManager _buildManager;

        public RegisterBuildCommand(IServiceProvider serviceProvider, IBuildManager buildManager) : base(serviceProvider)
        {
            _buildManager = buildManager;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            if (Context.Parameters.Length != 4 && Context.Parameters.Length != 7)
                throw new CommandWrongUsageException(Context);

            string buildName = Context.Parameters[0];

            if (buildName.StartsWith("-"))
                throw new CommandWrongUsageException("The build's name can't start with \"-\"");

            if (!float.TryParse(Context.Parameters[1], out float sizeX) ||
                !float.TryParse(Context.Parameters[2], out float sizeY) ||
                !float.TryParse(Context.Parameters[3], out float sizeZ))
            {
                throw new CommandWrongUsageException("Sizes must be numbers");
            }
            Vector3 size = new Vector3(sizeX, sizeY, sizeZ);

            Vector3 shift;
            if (Context.Parameters.Length == 7)
            {
                if (!float.TryParse(Context.Parameters[4], out float shiftX) ||
                    !float.TryParse(Context.Parameters[5], out float shiftY) ||
                    !float.TryParse(Context.Parameters[6], out float shiftZ))
                {
                    throw new CommandWrongUsageException("Shifts must be numbers");
                }

                shift = new Vector3(shiftX, shiftY, shiftZ);
            }
            else
            {
                shift = Vector3.zero;
            }

            _buildManager.SaveBuild(
                buildName,
                user.Player.Player.transform.position,
                user.Player.Player.transform.rotation,
                size,
                shift
            );

            return UniTask.CompletedTask;
        }
    }
}