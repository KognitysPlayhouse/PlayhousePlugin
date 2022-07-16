using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class ClearBroadcast : ICommand
	{
		public string Command { get; } = "clearbroadcast";
		public string[] Aliases { get; } = { "clearbc", "clearbroadcasts", "clearbcs" };
		public string Description { get; } = "Clears your broadcasts";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			p.ClearBroadcasts();

			response = "Cleared Broadcasts!";
			return true;
		}
	}
}