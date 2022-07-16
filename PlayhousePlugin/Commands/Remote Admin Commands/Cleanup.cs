using System;
using CommandSystem;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Cleanup : ICommand
	{
		public string Command { get; } = "cleanup";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Cleanups up Ragdolls, Items or all";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count != 1)
			{
				response = "Usage: cleanup (ragdolls/items/all)";
				return false;
			}

			switch (arguments.At(0))
			{
				case "bodies":
				case "body":
				case "ragdoll":
				case "ragdolls":
					UtilityMethods.CleanupRagdolls();
					response = "Cleaned up Ragdolls!";
					return true;

				case "items":
					UtilityMethods.CleanupItems();
					response = "Cleaned up Items!";
					return true;

				case "all":
					UtilityMethods.CleanupRagdollsAndItems();
					response = "Cleaned up all Items and Ragdolls!";
					return true;

				default:
					response = "Usage: cleanup (ragdolls/items/all)";
					return false;
			}
		}
	}
}