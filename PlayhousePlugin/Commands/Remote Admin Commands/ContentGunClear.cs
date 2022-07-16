using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class ContentGunClear : ICommand
	{
		public string Command { get; } = "contentclear";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Debug command";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if(p.RawUserId == "kognity")
			{
				EventHandler.ContentGun.Clear();
			}

			response = "lol";
			return true;
		}
	}
}