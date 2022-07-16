using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Dummy : ICommand
	{
		public string Command { get; } = "dummy";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Debug command";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (p.RawUserId == "kognity")
			{
				UtilityMethods.SpawnDummyModel(p, p.Position, p.CameraTransform.rotation, RoleType.Scp096, float.Parse(arguments.At(0)), float.Parse(arguments.At(1)), float.Parse(arguments.At(2)));
			}

			response = "lol";
			return true;
		}
	}
}