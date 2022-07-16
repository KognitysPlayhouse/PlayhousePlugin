using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using RemoteAdmin;
using UnityEngine;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Shit : ICommand
    {
        public string Command { get; } = "shit";
        public string[] Aliases { get; } = null;
        public string Description { get; } = "Makes you shit yourself if you're in the LCZ bathroom.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender))
            {
                response = "This command can only be ran by a player!";
                return false;
            }

            if (!EventHandler.IsDevServer)
            {
                response = "You may not take a shit because it would lag everyone.";
                return false;
            }

            var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
            var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

            if (p.CurrentRoom.Type == RoomType.LczToilets)
            {
                var gameObject = UnityEngine.Object.Instantiate<GameObject>(
                    LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x => x.name == "TantrumObj"));
                gameObject.transform.position = p.ReferenceHub.playerMovementSync.RealModelPosition;
                NetworkServer.Spawn(gameObject);
                response = "Shitting time.";
                return true;
            }
            else
            {
                response = "You're not in the right place to shit yourself.";
                return false;    
            }
        }
    }
}