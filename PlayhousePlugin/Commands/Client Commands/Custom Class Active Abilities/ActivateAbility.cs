using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class ActivateAbility : ICommand
	{
		public string Command { get; } = "activateability";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Actives your ability selection";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return false;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var abilityIndex = p.CustomClassManager().AbilityIndex;

			if ( p.CustomClassManager().CustomClass == null || p.CustomClassManager().CustomClass.AbilitiesNum == 0)
			{
				response = "Your class doesn't have any active abilities";
				return false;
			}

			p.CustomClassManager().CustomClass.ActiveAbilities[abilityIndex].Use(); 
			response = "Used ability";
			return true;
		}
	}
}