using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using RemoteAdmin;
using UnityEngine;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Tutorial : ICommand
    {
        public string Command { get; } = "tutorial";
        public string[] Aliases { get; } = {"tut"};
        public string Description { get; } = "Sets a player as a tutorial";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.tut"))
            {
                response = "You do not have permission to use this command";
                return false;
            }
            Player Ply;
            switch (arguments.Count)
            {
                case 0:
                case 1:
                    if (arguments.Count == 0)
                    {
                        if (!(sender is PlayerCommandSender plysend))
                        {
                            response = "You must be in-game to run this command if you specify yourself!";
                            return false;
                        }

                        Ply = Player.Get(plysend.ReferenceHub);
                    }
                    else
                    {
                        if (String.IsNullOrWhiteSpace(arguments.At(0)))
                        {
                            response = "Please do not try to put a space as tutorial";
                            return false;
                        }

                        Ply = Player.Get(arguments.At(0));
                        if (Ply == null)
                        {
                            response = $"Player not found: {arguments.At(0)}";
                            return false;
                        }
                    }

                    DoTutorialFunction(Ply, out response);
                    return true;
                default:
                    response = "Usage: tutorial (optional: id / name)";
                    return false;
            }
        }

        private IEnumerator<float> SetClassAsTutorial(Player Ply)
        {
            Vector3 OldPos;
            if (Ply.Role.Type == RoleType.Spectator)
            {
                OldPos = new Vector3(48.2f, 991, -59);
            }
            else
                OldPos = Ply.Position;
            
            Ply.Role.Type = RoleType.Tutorial;
            yield return Timing.WaitForSeconds(0.5f);
            Ply.Position = OldPos;
            Ply.NoClipEnabled = true;
        }

        private void DoTutorialFunction(Player Ply, out string response)
        {
            if (Ply.Role.Type != RoleType.Tutorial)
            {
                Timing.RunCoroutine(SetClassAsTutorial(Ply));
                response = $"Player {Ply.Nickname} is now set to tutorial";
            }
            else
            {
                Ply.Role.Type = RoleType.Spectator;
                Ply.NoClipEnabled = false;
                response = $"Player {Ply.Nickname} is now set to spectator";
            }
        }
    }
}