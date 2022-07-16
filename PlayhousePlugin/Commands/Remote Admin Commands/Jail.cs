using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class Jail : ICommand
	{
		public string Command { get; } = "jail";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Jails or unjails a user.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!((CommandSender)sender).CheckPermission("at.jail"))
			{
				response = "You do not have permission to use this command";
				return false;
			}

			if (arguments.Count != 1)
			{
				response = "Usage: jail (player id / name)";
				return false;
			}

			Player Ply = Player.Get(arguments.At(0));
			if (Ply == null)
			{
				response = $"Player not found: {arguments.At(0)}";
				return false;
			}

			if (EventHandler.JailedPlayers.Any(j => j.Userid == Ply.UserId))
			{
				Timing.RunCoroutine(UtilityMethods.DoUnJail(Ply));
				response = $"Player {Ply.Nickname} has been unjailed now";
			}
			else
			{
				Timing.RunCoroutine(UtilityMethods.DoJail(Ply));
				response = $"Player {Ply.Nickname} has been jailed now";
			}
			return true;
		}
	}
}