using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;

namespace BuildSpawner.RocketMod.Commands
{
    public class ListBuildsCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "listbuilds";

        public string Help => "List all available builds.";

        public string Syntax => string.Empty;

        public List<string> Aliases => new List<string>() { "lbuilds" };

        public List<string> Permissions => new List<string>() { "buildspawner.admin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            string buildList = Plugin.Instance.BuildManager.ListBuilds();

            ChatManager.serverSendMessage($"Available builds: {buildList}", Color.green, toPlayer: player.SteamPlayer());
        }
    }
}