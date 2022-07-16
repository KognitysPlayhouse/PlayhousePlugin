using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DisableBulletHoles : ICommand
	{
		public string Command { get; } = "disablebh";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Disables bulletholes";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (DisableBulletHolesBool)
			{
				DisableBulletHolesBool = false;
			}
			else
			{
				DisableBulletHolesBool = true;
			}

			response = $"Bulletholes are now {DisableBulletHolesBool}!";
			return true;
		}
		public static bool DisableBulletHolesBool = false;
	}
}