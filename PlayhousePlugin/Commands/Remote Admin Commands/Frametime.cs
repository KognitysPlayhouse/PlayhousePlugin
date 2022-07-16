using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using UnityEngine;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Frametime : ICommand
    {
        public string Command { get; } = "frametime";
        public string[] Aliases { get; } = {"ft"};
        public string Description { get; } = "Command to get the frametime of the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"frametime:{Time.deltaTime}s ~ {1/Time.deltaTime}FPS";
            return true;
        }
    }
}