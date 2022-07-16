using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace PlayhousePlugin.Commands
{
    public class Clear : ICommand
    {
        public string Command { get; } = "clear";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Clears infinite ammo from everyone";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender as CommandSender).CheckPermission("ct.infammo"))
            {
                response = "You do not have permission to run this command! Missing permission: \"ct.infammo\"";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: infammo clear";
                return false;
            }

            foreach (Player Ply in Player.List)
                if (Ply.ReferenceHub.TryGetComponent(out InfiniteAmmoComponent InfAmmo))
                    UnityEngine.Object.Destroy(InfAmmo);

			EventHandler.PlayersWithInfiniteAmmo.Clear();
            Map.Broadcast(5, "Infinite ammo is taken away from everyone now!");
            response = "Infinite ammo has been taken away from everyone";
            return true;
        }
    }
}
