using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class Ghost : ICommand
	{
		public string Command { get; } = "ghost";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Sets everyone or a user to be invisible.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!((CommandSender)sender).CheckPermission("at.ghost"))
			{
				response = "You do not have permission to use this command";
				return false;
			}

			if (arguments.Count != 1)
			{
				response = "Usage:\nghost ((player id / name) or (all / *))" +
				           "\nghost clear";
				return false;
			}

			switch (arguments.At(0))
			{
				case "clear":
					foreach (Player Pl in Player.List)
						Pl.IsInvisible = false;

					response = "Everyone is no longer invisible";
					return true;
				case "*":
				case "all":
					foreach (Player Pl in Player.List)
						Pl.IsInvisible = true;

					response = "Everyone is now invisible";
					return true;
				default:
					Player Ply = Player.Get(arguments.At(0));
					if (Ply == null)
					{
						response = $"Player not found: {arguments.At(0)}";
						return false;
					}

					if (!Ply.IsInvisible)
					{
						Ply.IsInvisible = true;
						Timing.RunCoroutine(Notification(Ply));
						response = $"Player {Ply.Nickname} is now invisible";
					}
					else
					{
						Ply.IsInvisible = false;
						response = $"Player {Ply.Nickname} is no longer invisible";
					}
					return true;
			}
		}

		private static IEnumerator<float> Notification(Player ply)
		{
			while (ply.IsInvisible)
			{
				ply.ShowCenterDownHint("You are invisible!", 1);
				yield return Timing.WaitForSeconds(1f);
			}
		}
	}
}