using System;
using CommandSystem;
using Exiled.Permissions.Extensions;

// This was taken from CreativeToolbox, that plugin is now out of date so I just took the bit that matters for my server.
namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class InfAmmo : ParentCommand
    {
        public InfAmmo() => LoadGeneratedCommands();

        public override string Command { get; } = "infammo";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Gives infinite ammo to players, clears infinite ammo from players, and shows who has infinite ammo";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new All());
            RegisterCommand(new Clear());
            RegisterCommand(new Give());
            RegisterCommand(new List());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender as CommandSender).CheckPermission("ct.infammo"))
            {
                response = "You do not have permission to run this command! Missing permission: \"ct.infammo\"";
                return false;
            }

            response = "Please enter a valid subcommand! Available ones: all, clear, give, list";
            return false;
        }
    }
}
