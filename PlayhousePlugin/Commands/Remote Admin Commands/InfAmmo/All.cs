using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace PlayhousePlugin.Commands
{
    public class All : ICommand
    {
        public string Command { get; } = "all";

        public string[] Aliases { get; } = new string[] { "*" };

        public string Description { get; } = "Gives everyone infinite ammo";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender as CommandSender).CheckPermission("ct.infammo"))
            {
                response = "You do not have permission to run this command! Missing permission: \"ct.infammo\"";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: infammo all / *";
                return false;
            }

            foreach (Player Ply in Player.List)
            {
                if (!Ply.ReferenceHub.TryGetComponent(out InfiniteAmmoComponent InfAmmo))
                {
                    Ply.ReferenceHub.gameObject.AddComponent<InfiniteAmmoComponent>();
					EventHandler.PlayersWithInfiniteAmmo.Add(Ply);
                }
            }

            Map.Broadcast(5, "Everyone has been given infinite ammo now!");
            response = "Everyone has been given infinite ammo";
            return true;
        }
    }
}
