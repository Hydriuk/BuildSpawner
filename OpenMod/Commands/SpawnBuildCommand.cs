using BuildSpawner.API;
using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;

namespace BuildSpawner.OpenMod.Commands
{
    [Command("spawnbuild")]
    [CommandAlias("sbuild")]
    [CommandSyntax("<name> [<shiftX> <shiftY> <shiftZ>] [-o] [-p <player>] [-pg <player>]")]
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

            int i = 0;

            string buildName = string.Empty;
            if (Context.Parameters.Length > i)
            {
                buildName = Context.Parameters[0];
                i++;
            }

            float shiftX = 0;
            float shiftY = 0;
            float shiftZ = 0;
            if (Context.Parameters.Length > i + 3 &&
                float.TryParse(Context.Parameters[1], out shiftX) &&
                float.TryParse(Context.Parameters[2], out shiftY) &&
                float.TryParse(Context.Parameters[3], out shiftZ))
            {
                i += 3;
            }

            bool origin = false;
            CSteamID playerId = CSteamID.Nil;
            CSteamID groupId = CSteamID.Nil;
            while (i < Context.Parameters.Length)
            {
                // Parse origin
                if (Context.Parameters[i] == "-origin" || Context.Parameters[i] == "-o")
                {
                    origin = true;
                    i++;
                }

                // Parse player
                else if (Context.Parameters[i] == "-player" || Context.Parameters[i] == "-p")
                {
                    if(i + 1 < Context.Parameters.Length && !Context.Parameters[i + 1].StartsWith("-"))
                    {
                        playerId = ParsePlayer(Context.Parameters[i + 1]);
                        i += 2;
                    }
                    else
                    {
                        playerId = user.SteamId;
                        i++;
                    }
                }

                // Parse playergroup
                else if (Context.Parameters[i] == "-playergroup" || Context.Parameters[i] == "-pg")
                {
                    if (i + 1 < Context.Parameters.Length && !Context.Parameters[i + 1].StartsWith("-"))
                    {
                        playerId = ParsePlayer(Context.Parameters[i + 1]);
                        groupId = ParseGroup(Context.Parameters[i + 1]);
                        i += 2;
                    }
                    else
                    {
                        playerId = user.SteamId;
                        groupId = user.Player.Player.quests.groupID;
                        i++;
                    }
                }
                else
                {
                    throw new CommandWrongUsageException(Context);
                }
            }

            if (Context.Parameters.Length > i ||
                buildName == string.Empty)
                throw new CommandWrongUsageException(Context);

            if (origin)
            {
                if (!_buildManager.PlaceBuild(buildName, playerId.m_SteamID, groupId.m_SteamID))
                    throw new UserFriendlyException($"{buildName} does not exist");

                return UniTask.CompletedTask;
            }

            Vector3 shift;
            if (shiftX != 0 || shiftY != 0 || shiftZ != 0)
                shift = new Vector3(shiftX, shiftY, shiftZ);
            else
                shift = Vector3.zero;

            bool buildFound = _buildManager.PlaceBuild(
                buildName,
                user.Player.Player.transform.position,
                user.Player.Player.transform.rotation,
                shift,
                playerId.m_SteamID,
                groupId.m_SteamID
            );

            if (!buildFound)
                throw new UserFriendlyException($"{buildName} does not exist");

            return UniTask.CompletedTask;
        }

        private CSteamID ParsePlayer(string name)
        {
            Player player = PlayerTool.getPlayer(name);

            if (player == null)
                throw new CommandWrongUsageException($"Player \"{name}\" not found");

            return player.channel.owner.playerID.steamID;
        }

        private CSteamID ParseGroup(string name)
        {
            Player player = PlayerTool.getPlayer(name);

            if (player == null)
                throw new CommandWrongUsageException($"Player \"{player}\" not found");

            return player.quests.groupID;
        }
    }
}