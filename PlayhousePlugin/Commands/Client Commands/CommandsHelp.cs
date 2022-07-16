using System;
using CommandSystem;
using Exiled.API.Features;
using MEC;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class CommandsHelp : ICommand
	{
		public string Command { get; } = "commands";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Gives you a list of commands you can use.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			//response = "<color=yellow>List of valid commands and default binds:\n\".commands\" - Gives this list\n\".commands\" - Gives this list\n\".kill\" - Kills yourself\n\".suicide\" - Kills yourself\n\".scplist\" - Gives a list of all currently alive SCPs (only works if you're SCP)\n\".pets\" - Gives the menu for donator pets\n\".deletedata\" - Wipes your Player Stats (WARNING: CHANGES ARE PERMANENT)\n\".discord\" - Gives you discord link\n\".clearbroadcast\" - Clears any broadcasts you have</color>";
			response = null;
			Timing.CallDelayed(0.1f, () =>
			{
				p.SendConsoleMessage("List of valid commands and default binds:\n\".commands\" - Gives this list\n\".kill\" - Kills yourself\n\".suicide\" - Kills yourself\n\".scplist\" - Gives a list of all currently alive SCPs (only works if you're SCP)\n\".pets\" - Gives the menu for donator pets\n\".deletedata\" - Wipes your Player Stats (WARNING: CHANGES ARE PERMANENT)\n\".discord\" - Gives you discord link\n\".clearbroadcast\" - Clears any broadcasts you have", "yellow");
				p.SendConsoleMessage("Default Binds to type:\n\"cmdbind f .zfe\" - Binds F to Explode as Zombie (only on Sundays)\n\"cmdbind 51 .activateability\" - Binds your '3' key to Activate Ability\n\"cmdbind 52 .changeability\" - Binds your '4' key to Change Ability", "cyan");
			});
			return true;
		}
	}
}