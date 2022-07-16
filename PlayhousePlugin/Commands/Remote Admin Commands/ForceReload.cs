using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class ForceReload : ICommand
	{
		public string Command { get; } = "forcereload";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Forces everyone to reload their weapons";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			foreach(Player ply in Player.List)
			{
				if(ply.CurrentItem is Firearm firearm)
					ply.ReloadWeapon();
				else
				{
					try
					{
						ply.Inventory.ServerSelectItem(ply.Inventory.UserInventory.Items.Where(x => x.Value.ItemTypeId.IsWeapon()).FirstOrDefault().Value.ItemSerial);
						Timing.CallDelayed(2.5f, () => { ply.ReloadWeapon(); });
					}
					catch { }
				}
			}

			response = "lol";
			return true;
		}
	}
}