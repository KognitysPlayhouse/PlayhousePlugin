using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Discord : ICommand
	{
		public string Command { get; } = "discord";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Gives you the link to the discord server.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			response = "\nJoin the Discord for some Server Perks!\nhttps://discord.gg/kognity";
			return true;
		}
	}
}