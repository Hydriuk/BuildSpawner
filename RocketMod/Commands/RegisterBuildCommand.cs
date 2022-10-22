using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;

namespace BuildSpawner.RocketMod.Commands
{
    public class RegisterBuildCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "registerbuild";

        public string Help => "Registers a build";

        public string Syntax => "<id> <sizeX> <sizeY> <sizeZ> [shiftX] [shiftY] [shiftZ]";

        public List<string> Aliases => new List<string>() { "rbuild" };

        public List<string> Permissions => new List<string>() { "buildspawner.admin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length != 4 && command.Length != 7)
            {
                ChatManager.serverSendMessage($"Wrong syntax: {Syntax}", Color.red, toPlayer: player.SteamPlayer());
                return;
            }

            string buildingId = command[0];

            if (buildingId.StartsWith("-"))
            {
                ChatManager.serverSendMessage("The build's name can't start with \"-\"", Color.red, toPlayer: player.SteamPlayer());
                return;
            }

            if (!float.TryParse(command[1], out float sizeX) ||
                !float.TryParse(command[2], out float sizeY) ||
                !float.TryParse(command[3], out float sizeZ))
            {
                ChatManager.serverSendMessage("Sizes must be numbers", Color.red, toPlayer: player.SteamPlayer());
                return;
            }
            Vector3 size = new Vector3(sizeX, sizeY, sizeZ);

            Vector3 shift;
            if (command.Length == 7)
            {
                if (!float.TryParse(command[4], out float shiftX) ||
                    !float.TryParse(command[5], out float shiftY) ||
                    !float.TryParse(command[6], out float shiftZ))
                {
                    ChatManager.serverSendMessage("Shifts must be numbers", Color.red, toPlayer: player.SteamPlayer());
                    return;
                }

                shift = new Vector3(shiftX, shiftY, shiftZ);
            }
            else
            {
                shift = Vector3.zero;
            }

            Plugin.Instance.BuildManager.SaveBuild(
                buildingId,
                player.Player.transform.position,
                player.Player.transform.rotation,
                size,
                shift
            );
        }
    }
}