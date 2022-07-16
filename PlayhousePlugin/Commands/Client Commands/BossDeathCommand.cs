using System;
using CommandSystem;
using Exiled.API.Features;
using MEC;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class BossDeath : ICommand
	{
		public string Command { get; } = "die";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Kognity only command";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (p.RawUserId != "kognity")
			{
				response = "You cannot use this command!";
				return false;
			}

			if (!EventHandler.SillySunday)
			{
				response = "Not Sunday";
				return false;
			}
			
			Timing.RunCoroutine(UtilityMethods.DeathSequence(p));
			Map.Broadcast(10, "<b><color=red><i>Boss has been defeated!!</i></color></b>");
			//GameCore.Console.singleton.TypeCommand("/audio 1 /home/ubuntu/tf2Audio/bossDefeated.mp3");
			p.IsGodModeEnabled = true;
			Timing.CallDelayed(12, () =>
			{
				p.Kill("too much cringe");
				p.Role.Type = RoleType.Spectator;
				Round.IsLocked = false;
			});

			response = "death time";
			return true;
		}
	}
}