using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SillySunday : ICommand
	{
		public string Command { get; } = "sillysunday";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Activates Silly Sunday (Mostly for debug)";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (EventHandler.SillySunday)
			{
				EventHandler.SillySunday = false;
				response = "Deactivated!";
				return true;
			}
			else
			{
				EventHandler.SillySunday = true;
				response = "Activated!";
				return true;
			}
		}
	}
}