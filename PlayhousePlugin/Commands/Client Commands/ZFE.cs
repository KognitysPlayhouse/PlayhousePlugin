using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class ZFE : ICommand
	{
		public string Command { get; } = "zfe";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Active only on sundays, it explodes you and damages anything nearby.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (p.Role.Team == Team.SCP && EventHandler.SillySunday)
			{
				if(p.Role.Type == RoleType.Scp0492)
				{
					UtilityMethods.Explode(p);
					/*
					//p.Kill(DamageTypes.Grenade);
					foreach (Player ply in Player.List)
					{
						if (ply.Role.Type == RoleType.Spectator || ply.Role.Type == RoleType.Scp079 || ply.IsGodModeEnabled || ply.ReferenceHub.characterClassManager.SpawnProtected || p == ply || ply.Role.Team == Team.SCP) continue;

						if (Vector3.Distance(p.Position, ply.Position) <= 10)
						{
							if (Physics.Linecast(p.Position, ply.Position, 9)) continue;
							ply.Hurt((250 * Mathf.Clamp(1 / (0.8f * Vector3.Distance(p.Position, ply.Position)), 0, 1)), p, damageType: DamageTypes.Grenade);
						}
					}*/
				}
				else if (p.Health <= 200)
				{
					UtilityMethods.Explode(p);
				}

				response = "KABOOOM!";
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