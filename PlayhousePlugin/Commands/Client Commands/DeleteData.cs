using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class DeleteData : ICommand
	{
		public string Command { get; } = "deletedata";
		public string[] Aliases { get; } = null ;
		public string Description { get; } = "Deletes your \"klp playerstats\" data";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (!Handler.deletes.Contains(p))
			{
				response = "Your data is marked for deletion, it will take affect next round";
				return true;
			}
			else
			{
				response = "Your data is already marked for deletion, it will take affect next round";
				return true;
			}
		}
	}
}