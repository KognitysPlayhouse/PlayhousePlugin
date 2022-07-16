using System;
using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.API.Features;

namespace PlayhousePlugin.Commands
{
    class Give : ICommand
    {
        public string Command { get; } = "give";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Gives the player infinite ammo";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender as CommandSender).CheckPermission("ct.infammo"))
            {
                response = "You do not have permission to run this command! Missing permission: \"ct.infammo\"";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: infammo give (player id / name)";
                return false;
            }

            Player Ply = Player.Get(arguments.At(0));
            if (Ply == null)
            {
                response = $"Player \"{arguments.At(0)}\" not found";
                return false;
            }

            if (!Ply.ReferenceHub.TryGetComponent(out InfiniteAmmoComponent InfAmmo))
            {
                EventHandler.PlayersWithInfiniteAmmo.Add(Ply);
                Ply.GameObject.AddComponent<InfiniteAmmoComponent>();
                Ply.Broadcast(5, "Infinite ammo is enabled for you!");
                response = $"Infinite ammo enabled for Player \"{Ply.Nickname}\"";
                return true;
            }
            else
            {
				EventHandler.PlayersWithInfiniteAmmo.Remove(Ply);
                UnityEngine.Object.Destroy(InfAmmo);
                Ply.Broadcast(5, "Infinite ammo is disabled for you!");
                response = $"Infinite ammo disabled for Player \"{Ply.Nickname}\"";
                return true;
            }
        }
    }
}
