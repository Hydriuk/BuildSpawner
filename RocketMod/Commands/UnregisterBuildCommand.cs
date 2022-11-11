using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BuildSpawner.RocketMod.Commands
{
    public class UnregisterBuildCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "unregisterbuild";

        public string Help => "Removes a build. Cannot be undone.";

        public string Syntax => "<name>";

        public List<string> Aliases => new List<string>() { "unrbuild" };

        public List<string> Permissions => new List<string>() { "buildspawner.admin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length != 1)
            {
                ChatManager.serverSendMessage($"Wrong syntax: {Syntax}", Color.red, toPlayer: player.SteamPlayer());
                return;
            }

            if (!Plugin.Instance.BuildStore.RemoveBuild(command[0]))
            {
                ChatManager.serverSendMessage($"{command[0]} does not exist", Color.red, toPlayer: player.SteamPlayer());
                return;
            }
        }
    }
}
