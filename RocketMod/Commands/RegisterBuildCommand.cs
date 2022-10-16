using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length != 4 && command.Length != 7)
            {
                ChatManager.serverSendMessage($"Wrong syntax: {Syntax}", Color.red, toPlayer: player.SteamPlayer());
                return;
            }

            string buildingId = command[0];
            Vector3 size;
            Vector3 shift;

            if (buildingId.StartsWith("-"))
            {
                ChatManager.serverSendMessage($"The building's name can't start with \"-\"", Color.red, toPlayer: player.SteamPlayer());
                return;
            }

            try
            {
                size = new Vector3(
                    float.Parse(command[1]),
                    float.Parse(command[2]),
                    float.Parse(command[3])
                );

                if (command.Length == 7)
                {
                    shift = new Vector3(
                        float.Parse(command[4]),
                        float.Parse(command[5]),
                        float.Parse(command[6])
                    );
                }
                else
                {
                    shift = Vector3.zero;
                }
            }
            catch (Exception)
            {
                ChatManager.serverSendMessage($"Coordinates must be decimal numbers", Color.red, toPlayer: player.SteamPlayer());
                return;
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
