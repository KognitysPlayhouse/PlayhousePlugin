using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class LocationCommand : ICommand
	{
		public string Command { get; } = "location";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Command to give your exact position and roation.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			response = $"Position:{p.Position.x}, {p.Position.y}, {p.Position.z}\nRotation: {p.ReferenceHub.transform.rotation.eulerAngles.x}, {p.ReferenceHub.transform.rotation.eulerAngles.y}, {p.ReferenceHub.transform.rotation.eulerAngles.z}";
			return true;
		}
	}
}