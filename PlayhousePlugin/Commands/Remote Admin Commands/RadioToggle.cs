using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using UnityEngine;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RadioToggle : ICommand
    {
        public string Command { get; } = "radiotoggle";
        public string[] Aliases { get; } = {"radiot", "toggleradio"};
        public string Description { get; } = "Command to toggle removing radios when players spawn";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender))
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
            var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

            if(EventHandler.WipeRadio)
                EventHandler.WipeRadio = false;
            else
                EventHandler.WipeRadio = true;

            response = "Done!";
            return true;
        }

        public Vector3 Position = new Vector3(-17, 994, -58);
        
        
    }
}