using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Footprinting;
using InventorySystem.Items.Firearms;
using PlayerStatsSystem;
using RemoteAdmin;
using UnityEngine;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Vaporize : ICommand
	{
		public string Command { get; } = "vaporize";
		public string[] Aliases { get; } = { "vapourize", "vape" };
		public string Description { get; } = "Vaporizes the player.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;
			
			if (p.IsAlive && !EventHandler.IsDeathMatchServer && !p.IsGodModeEnabled)
			{
				var e = new DisruptorDamageHandler(new Footprint(p.ReferenceHub), int.MaxValue)
				{
					ForceFullFriendlyFire = true
				};
				
				p.ReferenceHub.playerStats.DealDamage(e);
				
				if (!p.DoNotTrack)
				{
					if (Handler.killBindsUsed.ContainsKey(p))
						Handler.killBindsUsed[p] += 1;
					else
						Handler.killBindsUsed.Add(p, 1);
				}
				
				response = $"{p.Nickname} bid farewell cruel world";
				return true;
			}
			else
			{
				response = "You cannot use this command";
				return false;
			}
		}
	}
}