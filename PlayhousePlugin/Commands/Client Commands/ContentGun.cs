using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class ContentGun : ICommand
	{
		public string Command { get; } = "contentgun";
		public string[] Aliases { get; } = new string[] { "cgun", "contentg", "cttg" };
		public string Description { get; } = "A donator perk that allows you fire ragdolls!";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (EventHandler.ContentGun.ContainsKey(p))
			{
				response = "You have already consumed all your ragdolls this round!";
				return false;
			}

			if (Donator.GetDonator(p, out Donator donator) != null)
			{
				if (donator.DonatorNum >= 1)
				{
					EventHandler.ContentGun.Add(p, new Pair(ContentGunAllowance[donator.DonatorNum], 0));
					p.Broadcast(4, $"<i>Activated! It will automatically deactivate once you've shot {ContentGunAllowance[donator.DonatorNum]} ragdolls!</i>");
					response = $"Activated! It will automatically deactivate once you've shot {ContentGunAllowance[donator.DonatorNum]} ragdolls!";
					return true;
				}
				else
				{
					response = "This is a donator tier 1 restricted command!";
					return true;
				}
			}
			else
			{
				response = "This is a donator tier 1 restricted command!";
				return true;
			}
		}

		public static Dictionary<int, int> ContentGunAllowance = new Dictionary<int, int>()
		{
			{ 1, 5 },
			{ 2, 10 },
			{ 3, 25 },
			{ 4, 50 },
			{ 5, 100 },
			{ 6, 200 },
		};
	}
}