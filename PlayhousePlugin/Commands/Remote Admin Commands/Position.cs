using CommandSystem;
using Exiled.API.Features;
using System;
using UnityEngine;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Position : ICommand
    {
        public string Command { get; } = "position";
        public string[] Aliases { get; } = {"pos"};
        public string Description { get; } = "Modifies or retrieves the position of a user or all users";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "\nUsage:\nposition (player id / name) (x position) (y position) (z position)";
                return false;
            }

            var Ply = Player.Get(arguments.At(0));

            if (Ply == null)
            {
                response = $"Couldn't find player {arguments.At(0)}";
                return false;
            }

            if (float.TryParse(arguments.At(1), out float X) && float.TryParse(arguments.At(2), out float Y) && float.TryParse(arguments.At(3), out float Z))
            {
                Ply.Position = new Vector3(X, Y, Z);
                response = $"Done!";
                return true;
            }
            else
            {
                response = $"Invalid position";
                return false;
            }
        }
    }
}