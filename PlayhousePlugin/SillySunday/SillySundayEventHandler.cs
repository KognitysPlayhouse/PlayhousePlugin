using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using MEC;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InventorySystem.Items.Pickups;
using Exiled.API.Extensions;

namespace PlayhousePlugin
{
	public static class SillySundayEventHandler
	{
		public static bool NerfWarMode = false;
		public static bool instantRevive = false;
		public static bool ohfiverescuemode = false;
		public static bool randomrevive = false;
		public static bool slaughterhouse = false;
		public static bool sugarrush = false;
		public static bool deathswap = false;

		public static bool ohfivedied = false;
		public static Player OhFivePlayer;
		
		public static void ResetToDefaults()
		{
			NerfWarMode = false;
			instantRevive = false;
			ohfiverescuemode = false;
			randomrevive = false;
			slaughterhouse = false;
			sugarrush = false;
			deathswap = false;

			ohfivedied = false;
			OhFivePlayer = null;
		}

		public static IEnumerator<float> NerfWar1()
		{
			Map.Broadcast(5, "<color=orange>Starting gamemode: <b>Nerf War!</b></color>");
			Round.IsLocked = true;
			SillySundayEventHandler.NerfWarMode = true;
			yield return Timing.WaitForSeconds(0.3f);

			foreach (Player Ply in Player.List)
			{
				Ply.Kill("too much cringe");
			}

			// Cleans all the items
			foreach (ItemPickupBase item in UnityEngine.Object.FindObjectsOfType<ItemPickupBase>())
				item.DestroySelf();

			// Cleans all the ragdolls
			foreach (Ragdoll doll in UnityEngine.Object.FindObjectsOfType<Ragdoll>())
				NetworkServer.Destroy(doll.gameObject);

			List<Player> classds = new List<Player> { };
			List<Player> scientists = new List<Player> { };
			bool toggle = false;

			Room gr18 = Room.List.FirstOrDefault(room => room.Name.Contains("372"));
			Room scp914 = Room.List.FirstOrDefault(room => room.Name.Contains("914"));
			Room armoury = Room.List.FirstOrDefault(room => room.Name.Contains("LCZ_Armory"));

			foreach (var door in armoury.Doors)
			{
				door.Base.ServerChangeLock(DoorLockReason.AdminCommand, true);
			}
			foreach (var door in scp914.Doors)
			{
				door.Base.ServerChangeLock(DoorLockReason.AdminCommand, true);
			}

			IEnumerable<Room> Checkpoints = Room.List.ToList().Where(room => room.Name.Contains("Chkp"));
			foreach (Room checkPoint in Checkpoints)
			{
				foreach (var door in checkPoint.Doors)
				{
					door.Base.ServerChangeLock(DoorLockReason.AdminCommand, true);
				}
			}

			foreach (Player Ply in Player.List)
			{
				if (toggle)
				{
					toggle = false;
					classds.Add(Ply);
				}
				else
				{
					toggle = true;
					scientists.Add(Ply);
				}
			}

			foreach (Player scientist in scientists)
			{
				scientist.Role.Type = RoleType.Scientist;
				Timing.CallDelayed(0.8f, () =>
				{
					scientist.ClearInventory();
					scientist.ClearBroadcasts();
					scientist.AddItem(ItemType.GunCOM15);
					scientist.Position = new Vector3(gr18.Position.x, gr18.Position.y + 2f, gr18.Position.z);
					scientist.Ammo[ItemType.Ammo9x19] = 10;
				});
			}

			foreach (Player classd in classds)
			{
				classd.Role.Type = RoleType.ClassD;
				Timing.CallDelayed(0.8f, () =>
				{
					classd.ClearInventory();
					classd.ClearBroadcasts();
					classd.AddItem(ItemType.GunCOM15);
					classd.Position = RoleExtensions.GetRandomSpawnProperties(RoleType.ClassD).Item1;
					classd.Ammo[ItemType.Ammo9x19] = 10;
				});
			}

			yield return Timing.WaitForSeconds(1f);
			Map.Broadcast(10, "<color=orange><b><size=70>Shoot your Nerf® Pistol and eliminate the other team!</size></b></color>");
		}

		public static IEnumerator<float> NerfWar2()
		{
			foreach (Player Ply in Player.List)
			{
				Ply.Kill("too much cringe");
			}
			yield return Timing.WaitForSeconds(0.1f);

			UtilityMethods.CleanupRagdollsAndItems();

			List<Player> classds = new List<Player> { };
			List<Player> scientists = new List<Player> { };
			bool toggle = false;

			foreach (Player Ply in Player.List)
			{
				if (toggle)
				{
					toggle = false;
					classds.Add(Ply);
				}
				else
				{
					toggle = true;
					scientists.Add(Ply);
				}
			}

			foreach (Player scientist in scientists)
			{
				scientist.Role.Type = RoleType.Scientist;
				Timing.CallDelayed(0.8f, () =>
				{
					scientist.ClearInventory();
					scientist.ClearBroadcasts();
					scientist.AddItem(ItemType.GunFSP9);
					scientist.Position = new Vector3(
						RoleExtensions.GetRandomSpawnProperties(RoleType.Scp096).Item1.x,
						RoleExtensions.GetRandomSpawnProperties(RoleType.Scp096).Item1.y + 2f,
						RoleExtensions.GetRandomSpawnProperties(RoleType.Scp096).Item1.z);
					scientist.Ammo[ItemType.Ammo9x19] = 10;
				});
			}

			foreach (Player classd in classds)
			{
				classd.Role.Type = RoleType.ClassD;
				Timing.CallDelayed(0.8f, () =>
				{
					classd.ClearInventory();
					classd.ClearBroadcasts();
					classd.AddItem(ItemType.GunFSP9);
					classd.Position = new Vector3(
						RoleExtensions.GetRandomSpawnProperties(RoleType.Scp93953).Item1.x,
						RoleExtensions.GetRandomSpawnProperties(RoleType.Scp93953).Item1.y + 2f,
						RoleExtensions.GetRandomSpawnProperties(RoleType.Scp93953).Item1.z);
					classd.Ammo[ItemType.Ammo9x19] = 10;
				});
			}

			yield return Timing.WaitForSeconds(1f);
			Map.Broadcast(10, "<color=orange><b><size=70>Shoot your Nerf® Sub-Machine Gun and eliminate the other team!</size></b></color>");
		}

		public static IEnumerator<float> NerfWar3()
		{
			foreach (Player Ply in Player.List)
			{
				Ply.Kill("too much cringe");
			}
			yield return Timing.WaitForSeconds(0.1f);

			UtilityMethods.CleanupRagdollsAndItems();

			List<Player> classds = new List<Player> { };
			List<Player> guards = new List<Player> { };
			bool toggle = false;

			Vector3 pt1 = RoleExtensions.GetRandomSpawnProperties(RoleType.FacilityGuard).Item1;
			Vector3 pt2 = RoleExtensions.GetRandomSpawnProperties(RoleType.FacilityGuard).Item1;

			while(pt1 == pt2)
			{
				pt2 = RoleExtensions.GetRandomSpawnProperties(RoleType.FacilityGuard).Item1;
			}

			foreach (Player Ply in Player.List)
			{
				if (toggle)
				{
					toggle = false;
					classds.Add(Ply);
				}
				else
				{
					toggle = true;
					guards.Add(Ply);
				}
			}

			foreach (Player guard in guards)
			{
				guard.Role.Type = RoleType.FacilityGuard;
				Timing.CallDelayed(0.8f, () =>
				{
					guard.ClearInventory();
					guard.ClearBroadcasts();
					guard.AddItem(ItemType.GunCrossvec);
					guard.Position = new Vector3(
						pt2.x,
						pt2.y + 2f,
						pt2.z);
					guard.Ammo[ItemType.Ammo9x19] = 10;
				});
			}

			foreach (Player classd in classds)
			{
				classd.Role.Type = RoleType.ClassD;
				Timing.CallDelayed(0.8f, () =>
				{
					classd.ClearInventory();
					classd.ClearBroadcasts();
					classd.AddItem(ItemType.GunCrossvec);
					classd.Position = new Vector3(
						pt1.x,
						pt1.y + 2f,
						pt1.z);
					classd.Ammo[ItemType.Ammo9x19] = 10;
				});
			}

			yield return Timing.WaitForSeconds(1f);
			Map.Broadcast(10, "<color=orange><b><size=70>Shoot your Nerf® Sub-Machine Gun and eliminate the other team!</size></b></color>");
		}

		public static IEnumerator<float> NerfWar4()
		{
			foreach (Player Ply in Player.List)
			{
				Ply.Kill("too much cringe");
			}
			yield return Timing.WaitForSeconds(0.1f);

			UtilityMethods.CleanupRagdollsAndItems();

			List<Player> classds = new List<Player> { };
			List<Player> mtfs = new List<Player> { };
			bool toggle = false;


			foreach (Player Ply in Player.List)
			{
				if (toggle)
				{
					toggle = false;
					classds.Add(Ply);
				}
				else
				{
					toggle = true;
					mtfs.Add(Ply);
				}
			}

			foreach (Player mtf in mtfs)
			{
				mtf.Role.Type = RoleType.NtfPrivate;
				Timing.CallDelayed(0.8f, () =>
				{
					mtf.ClearInventory();
					mtf.ClearBroadcasts();
					mtf.AddItem(ItemType.GunLogicer);
					mtf.Position = new Vector3(
						RoleExtensions.GetRandomSpawnProperties(RoleType.NtfPrivate).Item1.x,
						RoleExtensions.GetRandomSpawnProperties(RoleType.NtfPrivate).Item1.y + 2f,
						RoleExtensions.GetRandomSpawnProperties(RoleType.NtfPrivate).Item1.z);
					mtf.Ammo[ItemType.Ammo762x39] = 10;
				});
			}

			foreach (Player classd in classds)
			{
				classd.Role.Type = RoleType.ClassD;
				Timing.CallDelayed(0.8f, () =>
				{
					classd.ClearInventory();
					classd.ClearBroadcasts();
					classd.AddItem(ItemType.GunLogicer);
					classd.Position = new Vector3(
						RoleExtensions.GetRandomSpawnProperties(RoleType.ChaosConscript).Item1.x,
						RoleExtensions.GetRandomSpawnProperties(RoleType.ChaosConscript).Item1.y + 2f,
						RoleExtensions.GetRandomSpawnProperties(RoleType.ChaosConscript).Item1.z);
					classd.Ammo[ItemType.Ammo762x39] = 10;
				});
			}

			yield return Timing.WaitForSeconds(1f);
			Round.IsLocked = false;
			Map.Broadcast(10, "<color=orange><b><size=70>Shoot your Nerf® Machine Gun and eliminate the other team!</size></b></color>");

			EventHandler.coroutines.Add(Timing.RunCoroutine(Spawn939NerfWar()));
		}

		public static IEnumerator<float> Spawn939NerfWar()
		{
			yield return Timing.WaitForSeconds(120f);
			foreach(Player Ply in Player.List)
			{
				if(Ply.Role.Type == RoleType.Spectator)
				{
					Ply.Role.Type = RoleType.Scp93953;
					Timing.CallDelayed(0.8f, () =>
					{
						Ply.Position = RoleExtensions.GetRandomSpawnProperties(RoleType.ChaosConscript).Item1;
					});
				}
			}
		}

		public static IEnumerator<float> OhFiveRescue()
		{
			yield return Timing.WaitForSeconds(3f);
			Round.IsLocked = true;
			yield return Timing.WaitForSeconds(1f);
			foreach (Player Ply in Player.List)
			{
				Ply.Kill("too much cringe");
			}
			yield return Timing.WaitForSeconds(1f);
			List<Player> playerList = Player.List.ToList();
			List<Player> ntfUnits = new List<Player> { };

			Room gr18 = Room.List.Where(room => room.Name.Contains("372")).FirstOrDefault();

			Player scientist = playerList[EventHandler.random.Next(playerList.Count)];
			playerList.Remove(scientist);

			scientist.ClearBroadcasts();

			scientist.Role.Type = RoleType.Scientist;
			scientist.MaxHealth = 200;
			scientist.Health = 200;
			GiveInventory(scientist);
			scientist.Scale = new Vector3(0.9f, 0.9f, 0.9f);
			scientist.Broadcast(10, "<color=yellow><i>Welcome to 05 Rescue</i>\n<b>Your objective is to escape the facility</b></color>");
			OhFivePlayer = scientist;

			for (int x = 0; x < 9; x++)
			{
				ntfUnits.Add(playerList[EventHandler.random.Next(playerList.Count)]);
				playerList.Remove(ntfUnits[ntfUnits.Count - 1]);
			}

			foreach (Player ntf in ntfUnits)
			{
				ntf.ClearBroadcasts();
				ntf.Role.Type = RoleType.NtfCaptain;
				GiveInventory(ntf);
				ntf.Scale = new Vector3(1.1f, 1.1f, 1.1f);
				ntf.Broadcast(10, "<color=yellow><i>Welcome to 05 Rescue</i>\n<b>Your objective is to escort 05-13 out of the facility. Eliminate all hostiles.</b></color>");
			}

			foreach (Player chaos in playerList)
			{
				chaos.ClearBroadcasts();

				chaos.Role.Type = RoleType.ChaosConscript;
				chaos.Position = RoleExtensions.GetRandomSpawnProperties(chaos.Role.Type).Item1;
				chaos.ClearInventory();
				chaos.AddItem(ItemType.GunLogicer);
				chaos.AddItem(ItemType.Medkit);
				chaos.AddItem(ItemType.Medkit);
				chaos.AddItem(ItemType.KeycardChaosInsurgency);
				chaos.Ammo[ItemType.Ammo762x39] = 150;
				chaos.Broadcast(10, "<color=yellow><i>Welcome to 05 Rescue</i>\n<b>Your objective is to kill all Foundation members</b></color>");
			}
			yield return Timing.WaitForSeconds(1.5f);
			foreach (Player chaos in playerList)
			{
				chaos.MaxHealth = 100;
				chaos.Health = 100;
			}

			foreach (Player ntf in ntfUnits)
			{
				ntf.MaxHealth = 450;
				ntf.Health = 450;
				ntf.Position = new Vector3(gr18.Position.x, gr18.Position.y + 2f, gr18.Position.z);
				yield return Timing.WaitForSeconds(0.25f);
			}
			yield return Timing.WaitForSeconds(0.5f);
			scientist.Scale = new Vector3(0.9f, 0.9f, 0.9f);
			yield return Timing.WaitForSeconds(0.5f);
			scientist.Position = new Vector3(gr18.Position.x, gr18.Position.y + 2f, gr18.Position.z);
			Round.IsLocked = false;
		}

		public static IEnumerator<float> SlaughterHouse()
		{
			yield return Timing.WaitForSeconds(1f);
			Round.IsLocked = true;
			yield return Timing.WaitForSeconds(1f);
			List<Player> playerList = Player.List.ToList();
			List<Player> scientists = new List<Player> { };

			List<Room> Checkpoints = Room.List.ToList().Where(room => room.Name.Contains("Chkp")).ToList();
			Room gr18 = Room.List.Where(room => room.Name.Contains("372")).FirstOrDefault();

			foreach (Room checkPoint in Checkpoints)
			{
				foreach (var door in checkPoint.Doors)
				{
					door.Base.ActiveLocks = 1;
					door.Base.NetworkActiveLocks = 1;
				}
			}

			foreach (Player Ply in Player.List)
			{
				Log.Info(Ply.Nickname);
			}

			for (int x = 0; x < 5; x++)
			{
				scientists.Add(playerList[EventHandler.random.Next(playerList.Count)]);
				playerList.Remove(scientists[scientists.Count - 1]);
			}

			foreach (Player scientist in scientists)
			{
				scientist.ClearBroadcasts();

				scientist.Role.Type = RoleType.Scientist;
				GiveInventory(scientist);
				Timing.CallDelayed(1, () =>
				{
					scientist.Scale = new Vector3(1.1f, 1.1f, 1.1f);
				});
				scientist.Broadcast(10, "<color=yellow><i>Welcome to Slaughter House</i>\n<b>Your objective is to kill as many Chaos as possible.</b></color>");
			}

			foreach (Player chaos in playerList)
			{
				chaos.ClearBroadcasts();

				chaos.Role.Type = RoleType.ChaosConscript;
				Timing.CallDelayed(1, () =>
				{
					chaos.Position = RoleExtensions.GetRandomSpawnProperties(RoleType.Scp173).Item1;
				});
				chaos.ClearInventory();
				chaos.AddItem(ItemType.GunLogicer);
				chaos.AddItem(ItemType.Medkit);
				chaos.AddItem(ItemType.Medkit);
				chaos.AddItem(ItemType.KeycardChaosInsurgency);
				chaos.Ammo[ItemType.Ammo762x39] = 450;
				chaos.Broadcast(10, "<color=yellow><i>Welcome to Slaughter House</i>\n<b>Your objective is to try and kill the Scientists.</b></color>");
			}


			yield return Timing.WaitForSeconds(1.5f);

			foreach (Player scientist in scientists)
			{
				scientist.MaxHealth = 5000;
				scientist.Health = 5000;
				scientist.Position = new Vector3(gr18.Position.x, gr18.Position.y + 1.7f, gr18.Position.z);
				yield return Timing.WaitForSeconds(0.25f);
			}
			yield return Timing.WaitForSeconds(0.25f);
			Round.IsLocked = false;

		}

		public static IEnumerator<float> SugarRush()
		{
			yield return Timing.WaitForSeconds(3f);

			foreach (Player Ply in Player.List)
			{
				Ply.EnableEffect<MovementBoost>(1);
				Ply.ChangeEffectIntensity(EffectType.MovementBoost,  30, 1800);
			}
			Map.Broadcast(10, "<color=red><size=55><b>Welcome to <i><color=yellow>Sugar Rush</color></i> Event.\nWas it sugar or was it another white powder?</b></size></color>");
		}

		public static IEnumerator<float> InstantRevive()
		{
			yield return Timing.WaitForSeconds(3f);
			List<Player> players = new List<Player> { };
			foreach (Player SCP in Player.Get(Team.SCP))
			{
				SCP.Kill("too much cringe");;
				players.Add(SCP);
			}
			yield return Timing.WaitForSeconds(1f);
			foreach (Player SCP in players)
			{
				SCP.Role.Type = RoleType.Scp049;
				yield return Timing.WaitForSeconds(0.2f);
				SCP.Position = RoleExtensions.GetRandomSpawnProperties(RoleType.Scp049).Item1;
			}
			Map.Broadcast(10, "<color=red><size=55><b>Welcome to <i><color=yellow>Instant Revive</color></i> Event. When SCP-049 kills a person, that person will instantly turn into a zombie!</b></size></color>");
		}

		public static IEnumerator<float> RandomRevive()
		{
			yield return Timing.WaitForSeconds(3f);
			List<Player> players = new List<Player> { };
			foreach (Player SCP in Player.Get(Team.SCP))
			{
				SCP.Kill("too much cringe");;
				players.Add(SCP);
			}
			yield return Timing.WaitForSeconds(1f);
			foreach (Player SCP in players)
			{
				SCP.Role.Type = RoleType.Scp049;
				yield return Timing.WaitForSeconds(0.2f);
				SCP.Position = RoleExtensions.GetRandomSpawnProperties(RoleType.Scp049).Item1;
			}
			Map.Broadcast(10, "<color=red><size=55><b>Welcome to <i><color=yellow>Plague Doctor's Beckoning</color></i> Event. When SCP-049 revives a dead body, that person will turn into a Random SCP</b></size></color>");
		}

		public static IEnumerator<float> DeathSwap()
		{
			yield return Timing.WaitForSeconds(3f);
		}

		/// <summary>
		/// Gives player Logicer, E11, CrossVec, 3 Medkits, Radio, and O5 keycard with full ammo.
		/// </summary>
		/// <param name="ply"></param>
		public static void GiveInventory(Player ply)
		{
			ply.ClearInventory();
			ply.AddItem(ItemType.Medkit);
			ply.AddItem(ItemType.Medkit);
			ply.AddItem(ItemType.Medkit);

			ply.AddItem(ItemType.GunLogicer);
			ply.AddItem(ItemType.GunE11SR);
			ply.AddItem(ItemType.GunCrossvec);

			ply.AddItem(ItemType.Radio);
			ply.AddItem(ItemType.KeycardO5);

			ply.Ammo[ItemType.Ammo9x19] = 500;
			ply.Ammo[ItemType.Ammo556x45] = 400;
			ply.Ammo[ItemType.Ammo762x39] = 750;
		}
	}
}
