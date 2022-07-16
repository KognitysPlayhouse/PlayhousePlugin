using System;
using System.Collections.Generic;
using System.IO;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using RemoteAdmin;
using UnityEngine;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Pets : ICommand
	{
		public string Command { get; } = "pets";
		public string[] Aliases { get; } = new string[] { "pet" };
		public string Description { get; } = "A donator perk that allows you to have a cute item pet that follows you around!";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (Donator.GetDonator(p, out Donator donator) != null)
			{
				if (arguments.Count == 0)
				{
					response = "Here are the all the pets!";
					p.SendConsoleMessage(File.ReadAllText("/home/ubuntu/.config/EXILED/Configs/Pets/PetsText.txt"), "yellow");
					return true;
				}
				var command = arguments.At(0);
				switch (command)
				{
					case "unequip":
					case "remove":
					case "0":
						UtilityMethods.CheckExistingPetAndKill(p.UserId);
						UtilityMethods.UpdatePreference(p, "0");
						response = "Pet uneqipped!";
						return true;				

					default:
						if (!Items.ContainsKey(command))
						{
							response = "That is not a valid pet number! USAGE: \".pets NUMBER\"";
							return true;
						}

						if(Items[command] == ItemType.MicroHID)
						{
							if (donator.IsBooster)
							{
								UtilityMethods.CheckExistingPetAndKill(p.UserId);

								PetFollow.Coroutines.Add(p.UserId,
									Timing.RunCoroutine(PetFollow.FollowPlayer(p, Item.Create(Items[command]).Spawn(p.Position + Vector3.up * 2))));

								UtilityMethods.UpdatePreference(p, command);
								response = "Pet Equipped!";
								return true;
							}
							else
							{
								response = "You must be a Discord Server booster to equip this pet!";
								return true;
							}

						}

						if(UtilityMethods.GetDonatorNum(command) <= donator.DonatorNum)
						{
							UtilityMethods.CheckExistingPetAndKill(p.UserId);

							PetFollow.Coroutines.Add(p.UserId,
									Timing.RunCoroutine(PetFollow.FollowPlayer(p, Item.Create(Items[command]).Spawn(p.Position + Vector3.up * 2))));

							UtilityMethods.UpdatePreference(p, command);
							response = "Pet Equipped!";
							return true;
						}
						else
						{
							response = $"You must be at least a Tier {UtilityMethods.GetDonatorNum(command)} Donator to equip this pet!";
							return true;
						}
				}
			}
			else
			{
				response = "This is a donator restricted command!";
				return true;
			}
		}

		public static Dictionary<string, ItemType> Items = new Dictionary<string, ItemType>()
		{
			// Discord Donators
			{ "d1", ItemType.MicroHID },
			{ "D1", ItemType.MicroHID },

			// Tier 1 Donators
			{ "11", ItemType.KeycardNTFLieutenant },
			{ "12", ItemType.Coin },
			{ "13", ItemType.Painkillers },
			{ "14", ItemType.SCP018 },

			// Tier 2 Donators
			{ "21", ItemType.KeycardNTFCommander },
			{ "22", ItemType.GrenadeFlash },
			{ "23", ItemType.Medkit },
			{ "24", ItemType.SCP207},

			// Tier 3 Donators
			{ "31", ItemType.KeycardContainmentEngineer},
			{ "32", ItemType.Adrenaline },
			{ "33", ItemType.Radio },
			{ "34", ItemType.SCP268 },

			// Tier 4 Donators
			{ "41", ItemType.KeycardFacilityManager },

			// Tier 5 Donators
			{ "51", ItemType.KeycardO5},
		};
	}
}