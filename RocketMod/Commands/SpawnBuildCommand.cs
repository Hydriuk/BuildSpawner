using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using UnityEngine;

namespace BuildSpawner.RocketMod.Commands
{
    public class SpawnBuildCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "spawnbuild";

        public string Help => "Spawn a build.";

        public string Syntax => "<name> [<shiftX> <shiftY> <shiftZ>] [-origin | -o] [-pg <player>] [-r]";

        public List<string> Aliases => new List<string>() { "sbuild" };

        public List<string> Permissions => new List<string>() { "buildspawner.admin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;

            int i = 0;

            string buildName = string.Empty;
            if (command.Length > i)
            {
                buildName = command[0];
                i++;
            }

            float shiftX = 0;
            float shiftY = 0;
            float shiftZ = 0;
            if (command.Length > i + 3 &&
                float.TryParse(command[1], out shiftX) &&
                float.TryParse(command[2], out shiftY) &&
                float.TryParse(command[3], out shiftZ))
            {
                i += 3;
            }

            bool origin = false;
            bool replace = false;
            CSteamID playerId = CSteamID.Nil;
            CSteamID groupId = CSteamID.Nil;
            while (i < command.Length)
            {
                // Parse origin
                if (command[i] == "-origin" || command[i] == "-o")
                {
                    origin = true;
                    i++;
                }

                // Parse replace
                else if (command[i] == "-replace" || command[i] == "-r")
                {
                    replace = true;
                    i++;
                }

                // Parse player
                else if (command[i] == "-player" || command[i] == "-p")
                {
                    if (i + 1 < command.Length && !command[i + 1].StartsWith("-"))
                    {
                        playerId = ParsePlayer(command[i + 1]);

                        if (playerId == CSteamID.Nil)
                        {
                            ChatManager.serverSendMessage($"Player \"{command[i + 1]}\" not found", Color.red, toPlayer: uPlayer.SteamPlayer());
                            return;
                        }

                        i += 2;
                    }
                    else
                    {
                        playerId = uPlayer.CSteamID;
                        i++;
                    }
                }

                // Parse playergroup
                else if (command[i] == "-playergroup" || command[i] == "-pg")
                {
                    if (i + 1 < command.Length && !command[i + 1].StartsWith("-"))
                    {
                        playerId = ParsePlayer(command[i + 1]);

                        if(playerId == CSteamID.Nil)
                        {
                            ChatManager.serverSendMessage($"Player \"{command[i + 1]}\" not found", Color.red, toPlayer: uPlayer.SteamPlayer());
                            return;
                        }

                        groupId = ParseGroup(command[i + 1]);
                        i += 2;
                    }
                    else
                    {
                        playerId = uPlayer.CSteamID;
                        groupId = uPlayer.Player.quests.groupID;
                        i++;
                    }
                }

                // Wrong Syntax
                else
                {
                    ChatManager.serverSendMessage($"Wrong syntax: {Syntax}", Color.red, toPlayer: uPlayer.SteamPlayer());
                    return;
                }
            }

            if (command.Length > i ||
                buildName == string.Empty)
            {
                ChatManager.serverSendMessage($"Wrong syntax: {Syntax}", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            if (origin)
            {
                if (!Plugin.Instance.BuildManager.PlaceBuild(buildName, playerId.m_SteamID, groupId.m_SteamID, replace))
                    ChatManager.serverSendMessage($"{buildName} does not exist", Color.red, toPlayer: uPlayer.SteamPlayer());

                return;
            }

            Vector3 shift;
            if (shiftX != 0 || shiftY != 0 || shiftZ != 0)
                shift = new Vector3(shiftX, shiftY, shiftZ);
            else
                shift = Vector3.zero;

            bool buildFound = Plugin.Instance.BuildManager.PlaceBuild(
                buildName,
                uPlayer.Player.transform.position,
                uPlayer.Player.transform.rotation,
                shift,
                playerId.m_SteamID,
                groupId.m_SteamID,
                replace
            );

            if (!buildFound)
            {
                ChatManager.serverSendMessage($"{buildName} does not exist", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }
        }

        private CSteamID ParsePlayer(string name)
        {
            Player player = PlayerTool.getPlayer(name);

            if (player == null)
                return CSteamID.Nil;

            return player.channel.owner.playerID.steamID;
        }

        private CSteamID ParseGroup(string name)
        {
            Player player = PlayerTool.getPlayer(name);

            if (player == null)
                return CSteamID.Nil;

            return player.quests.groupID;
        }
    }
}