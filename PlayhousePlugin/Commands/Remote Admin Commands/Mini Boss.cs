using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using RemoteAdmin;
using UnityEngine;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class MiniBoss : ICommand
    {
        public string Command { get; } = "miniboss";
        public string[] Aliases { get; } = {"mb", "mini", "boss"};
        public string Description { get; } = "Command to create minibosses";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender))
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
            var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

            foreach (var player in arguments.ToList())
            {
                Player ply = Player.Get(player);

                ply.Role.Type = RoleType.ChaosRepressor;
                ply.ClearInventory();
                ply.AddItem(ItemType.GunLogicer);
                ply.Health = 15000;
                    
                ply.EnableEffect(EffectType.Scp207);
                ply.EnableEffect(EffectType.Scp207);
                ply.EnableEffect(EffectType.Scp207);
                ply.EnableEffect(EffectType.Scp207);
                
                Timing.CallDelayed(0.5f, () =>
                {
                    ply.Position = Position;
                });
                
                Timing.CallDelayed(1f, () =>
                {
                    ply.Scale = new Vector3(2, 2, 2);
                });
                    
            }

            response = "Done!";
            return true;
        }

        public Vector3 Position = new Vector3(-17, 994, -58);
        
        
    }
}