using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;

namespace BuildSpawner.RocketMod.Commands
{
    public class SpawnBuildCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "spawnbuild";

        public string Help => "Spawn a build.";

        public string Syntax => " <name> [<shiftX> <shiftY> <shiftZ>] [-origin | -o]";

        public List<string> Aliases => new List<string>() { "sbuild" };

        public List<string> Permissions => new List<string>() { "buildspawner.admin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length != 1 && command.Length != 2 && command.Length != 4)
            {
                ChatManager.serverSendMessage($"Wrong syntax: {Syntax}", Color.red, toPlayer: player.SteamPlayer());
                return;
            }

            string buildName = command[0];

            if (command.Length == 2)
            {
                if (!Plugin.Instance.BuildManager.PlaceBuild(buildName, player.CSteamID.m_SteamID, player.SteamGroupID.m_SteamID))
                    ChatManager.serverSendMessage($"{buildName} does not exist", Color.yellow, toPlayer: player.SteamPlayer());

                return;
            }

            Vector3 shift;
            if (command.Length == 4)
            {
                if (!float.TryParse(command[1], out float shiftX) || !float.TryParse(command[2], out float shiftY) || !float.TryParse(command[3], out float shiftZ))
                {
                    ChatManager.serverSendMessage("Coordinates must be decimal numbers", Color.red, toPlayer: player.SteamPlayer());
                    return;
                }

                shift = new Vector3(shiftX, shiftY, shiftZ);
            }
            else
            {
                shift = Vector3.zero;
            }

            bool buildFound = Plugin.Instance.BuildManager.PlaceBuild(
                buildName,
                player.Player.transform.position,
                player.Player.transform.rotation,
                shift,
                player.CSteamID.m_SteamID,
                player.SteamGroupID.m_SteamID
            );

            if (!buildFound)
                ChatManager.serverSendMessage($"{buildName} does not exist", Color.yellow, toPlayer: player.SteamPlayer());
        }
    }
}