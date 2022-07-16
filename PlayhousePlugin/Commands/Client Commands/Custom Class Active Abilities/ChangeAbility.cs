using System;
using CommandSystem;
using Exiled.API.Features;
using PlayhousePlugin.CustomClass;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class ChangeAbility : ICommand
	{
		public string Command { get; } = "changeability";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Changes your ability selection";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return false;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			CustomClassManager classManager = p.CustomClassManager();

			if (classManager.CustomClass != null && classManager.CustomClass.AbilitiesNum != 0)
			{
				// If somehow your selection was beyond what's allowed
				if (classManager.AbilityIndex > classManager.CustomClass.AbilitiesNum-1)
				{
					classManager.AbilityIndex = 0;
					response = "Changed your ability selection!";
					return true;
				}

				if (classManager.AbilityIndex + 1 > classManager.CustomClass.AbilitiesNum-1) // If adding 1 more is more than your ability
				{
					classManager.AbilityIndex = 0;
					response = "Changed your ability selection!";
					return true;
				}

				classManager.AbilityIndex += 1;
				response = "Changed your ability selection!";
				return true;
			}
			else
			{
				response = "You are not a custom class/You do not have any active abilities";
				return false;
			}
		}
	}
}