using System;
using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.API.Features;

namespace PlayhousePlugin.Commands
{
    public class List : ICommand
    {
        public string Command { get; } = "list";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Lists every player who has infinite ammo";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender as CommandSender).CheckPermission("ct.infammo"))
            {
                response = "You do not have permission to run this command! Missing permission: \"ct.infammo\"";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: infammo list";
                return false;
            }

            if (EventHandler.PlayersWithInfiniteAmmo.Count > 0)
            {
				EventHandler.PlayerLister.Append("Players with infinite ammo: ");
                foreach (Player Ply in EventHandler.PlayersWithInfiniteAmmo)
					EventHandler.PlayerLister.Append(Ply.Nickname + ", ");

                int length = EventHandler.PlayerLister.ToString().Length;
                response = EventHandler.PlayerLister.ToString().Substring(0, length - 2);
				EventHandler.PlayerLister.Clear();
                return true;
            }
            else
            {
                response = "There are no players currently online with infinite ammo";
				EventHandler.PlayerLister.Clear();
                return true;
            }
        }
    }
}
