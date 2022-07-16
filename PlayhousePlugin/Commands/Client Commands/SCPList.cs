using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class SCPList : ICommand
	{
		public string Command { get; } = "scplist";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Lists all the SCPs in the game. (Only works if you are SCP)";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (p.Role.Team == Team.SCP)
			{
				IEnumerable<Player> SCPs = Player.List.Where(r => r.Role.Team == Team.SCP);

				string responseMessage = "----------";

				foreach (Player scp in SCPs)
				{
					responseMessage += $"\n{scp.Nickname} - {scp.ReferenceHub.characterClassManager.Classes.SafeGet(scp.Role.Type).fullName} - {scp.Health} HP\nCurrent Zone: {scp.Zone}\n----------";
				}

				response = responseMessage;
				return true;
			}
			else
			{
				response = "You cannot use this command!";
				return true;
			}
		}
	}
}