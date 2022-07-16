using System.Collections.Generic;
using System.Linq;
using MEC;
using Exiled.API.Features;
using UnityEngine;
using Mirror;
using Exiled.API.Enums;
using CustomPlayerEffects;
using System.Text;
using System.IO;
using InventorySystem.Items.Pickups;
using System;
using AdminToys;
using Exiled.API.Features.Items;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs;
using Footprinting;
using RemoteAdmin;
using Mirror.LiteNetLib4Mirror;
using InventorySystem;
using InventorySystem.Items.Firearms.Ammo;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using PlayerStatsSystem;

namespace PlayhousePlugin
{
	// Contains commonly used methods
	class UtilityMethods
	{
		public static void FindAndSetCustomBadge(Player ply)
		{
			if (ply.GlobalBadge != null) return;
			try
			{
				var t = File.ReadAllText($@"/home/ubuntu/.config/EXILED/Configs/CustomBadgeTexts/badges.txt").Split('\n').ToList().Where(x => x.Contains(ply.RawUserId)).FirstOrDefault();

				if (t != null)
				{
					Timing.CallDelayed(5f, () =>
					{
						ply.RankName = ply.RankName=="" ? $"{t.Substring(ply.RawUserId.Length + 1)}" : $"{t.Substring(ply.RawUserId.Length+1)} ({ply.RankName})";
					});
				}

			}
			catch
			{
				return;
			}
		}

		public static IEnumerator<float> FadeAway(PrimitiveObjectToy toy)
		{
			var time = 0f;
			while (time < 3f)
			{
				time += Time.deltaTime;
				toy.NetworkMaterialColor = new Color(1, 0, 0, 0.7f - time/4.3f);
				yield return Timing.WaitForOneFrame;
			}
			NetworkServer.Destroy(toy.gameObject);
		}
		
		public static SchematicObject SpawnSchematic(string schematicName, Vector3 position)
		{
			var schematicData = MapUtils.GetSchematicDataByName(schematicName);
			return ObjectSpawner.SpawnSchematic(schematicName,
				position, Quaternion.identity, Vector3.one, schematicData);
		}
		
		public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, Quaternion rotation)
		{
			var schematicData = MapUtils.GetSchematicDataByName(schematicName);
			return ObjectSpawner.SpawnSchematic(schematicName,
				position, rotation, Vector3.one, schematicData);
		}
		
		public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, Vector3 scale)
		{
			var schematicData = MapUtils.GetSchematicDataByName(schematicName);
			return ObjectSpawner.SpawnSchematic(schematicName,
				position, Quaternion.identity, scale, schematicData);
		}

		public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			var schematicData = MapUtils.GetSchematicDataByName(schematicName);
			return ObjectSpawner.SpawnSchematic(schematicName,
				position, rotation, scale, schematicData);
		}
		
		// Spawns an actual grenade that gets spit from the players view
		public static Pickup SpawnGrenadeOnPlayer(Player player, ItemType grenadeType, float timer, float velocity = 1f)
		{
			var nade = (ExplosiveGrenade) ExplosiveGrenade.Create(grenadeType, player);
			nade.FuseTime = 99999;
			var pickup = nade.Spawn(player.Position);
			nade.Base.ServerThrow(10, 1, Vector3.one, player.CameraTransform.forward*2);
			//nade.SpawnActive(player.Position, player);
			return pickup;
		}

		public static void ApplyAmmoRegen(Player p, ushort ammoCount, bool displayHint, Player Ammo)
		{
			List<ItemType> typesToGive = new List<ItemType>();
			foreach(var gun in p.Items.Where(x => x.Type.IsWeapon()))
			{
				Firearm firearm = (Firearm)gun;
				if (!typesToGive.Contains(firearm.AmmoType.GetItemType()))
					typesToGive.Add(firearm.AmmoType.GetItemType());
			}

			ArmourAmmo limits;
			var armour = p.Items.FirstOrDefault(x => x.Type.IsArmor());

			if (armour == null)
				limits = Utils.ArmourAmmoLimits[ItemType.None];
			else
				limits = Utils.ArmourAmmoLimits[armour.Type];

			bool ammoGiven = false;
			
			foreach (var type in typesToGive)
			{
				try
				{
					if (p.Ammo[type] < limits.LimitDictionary[type])
					{
						if (p.Ammo[type] + ammoCount > limits.LimitDictionary[type])
							p.Inventory.ServerSetAmmo(type, limits.LimitDictionary[type]);
						else
							p.Inventory.ServerAddAmmo(type, ammoCount);
						
						ammoGiven = true;
					}
				}
				catch
				{
					p.Inventory.ServerAddAmmo(type, 1);
				}
			}

			if (displayHint && ammoGiven)
			{
				Ammo.ShowCenterDownHint("<color=red>You are providing ammo!</color>", 1);
				p.ShowCenterDownHint($"<color=red>Ammo Replenished</color>", 1);

			}
		}

		public static void ApplyPoison(Player p, Player Exterminator)
		{
			if (p.Health - 4f <= 0)
			{
				p.Kill("Military Grade Bio-Weapon");
			}
			else
			{
				if (p.Role.Type != RoleType.Scp079 && p.IsAlive)
				{
					p.Hurt(4f, "Military Grade Bio-Weapon");
					//p.Hurt(7.5f, Exterminator, damageType: DamageTypes.Poison);
					p.ShowCenterDownHint("<color=yellow>You are being poisoned by a military grade Bio-Weapon</color>");
				}
			}
		}

		// Healing method designed to be used for the Medic which heals HP and overheals AHP
		public static void ApplyMedicHeal(Player p, float h, bool displayHint, Player Medic)
		{
			float HpGiven = 0;
			float AHPGiven = 0;
			if (p.Health + h > p.MaxHealth)
			{
				HpGiven = p.MaxHealth - p.Health;
				p.Health = p.MaxHealth;
			}
			else
			{
				HpGiven = h;
				p.Health += h;
			}
			
			if (p.Health == p.MaxHealth && p != Medic)
			{
				if (p.ArtificialHealth < 20)
				{
					//Give player 5 AHP if their current AHP is less than 20 if we add 5 AHP
					if (p.ArtificialHealth + 5 > 20)
					{
						AHPGiven = 20 - p.ArtificialHealth;
						p.ArtificialHealth = 20;
					}
					else
					{
						AHPGiven = 5;
						p.ArtificialHealth += 5;
					}
				}
			}
			if (displayHint && HpGiven != 0)
			{
				Medic.ShowCenterDownHint("<color=red>You are healing!</color>", 1);
				p.ShowCenterDownHint($"<color=red>+HP</color>");
			}
			if (displayHint && AHPGiven > 1)
			{
				p.ShowCenterDownHint($"<color=red>+AHP</color>");
			}
		}

		// Heals HP of target
		public static void ApplyHeal(Player p, float h, bool displayHint, Player Healer)
		{
			float HpGiven = 0;
			if (p.Health + h > p.MaxHealth)
			{
				HpGiven = p.MaxHealth - p.Health;
				p.Health = p.MaxHealth;
			}
			else
			{
				HpGiven = h;
				p.Health += h;
			}
			
			if (displayHint && HpGiven > 1)
			{
				Healer.ShowCenterDownHint("<color=red>You are healing!</color>", 1);
				p.ShowCenterDownHint($"<color=red>+HP</color>");
			}
		}

		// Overheals AHP of target
		public static void ApplyOverheal(Player p, float h, bool displayHint, Player Healer)
		{
			if (p.MaxArtificialHealth == 0 && (p.Role.Type == RoleType.Scp049 || p.Role.Type == RoleType.Scp0492))
			{
				p.MaxArtificialHealth = 100;
			}
			
			float HpGiven = 0;
			if (p.ArtificialHealth + h > p.MaxArtificialHealth)
			{
				HpGiven = p.MaxArtificialHealth - p.ArtificialHealth;
				p.ArtificialHealth = p.MaxArtificialHealth;
			}
			else
			{
				HpGiven = h;
				p.ArtificialHealth += h;
			}
			
			if (displayHint && HpGiven > 1)
			{
				Healer.ShowCenterDownHint("<color=red>You are overhealing!</color>", 1);
				p.ShowCenterDownHint($"<color=red>+AHP</color>");
			}
		}

		public static void InfectPlayer(Player Ply)
		{
			if (!PlayhousePlugin.PlayhousePluginRef.Handler.InfectedPlayers.Contains(Ply))
			{
				PlayhousePlugin.PlayhousePluginRef.Handler.InfectedPlayers.Add(Ply);
				Ply.ReferenceHub.playerEffectsController.EnableEffect<Poisoned>();
				Ply.ReferenceHub.playerEffectsController.EnableEffect<Hemorrhage>();
			}
		}

		public static IEnumerator<float> LobbyTimer()
		{
			StringBuilder messageUp = new StringBuilder();
			StringBuilder messageDown = new StringBuilder();
			var text = "<b>discord.gg/kognity</b>\n<color=%rainbow%><b>Welcome To Kognity's Playhouse\nGo stand near the team you want to play as!</b></color>";
			int x = 0;
			string[] colors = { "#f54242", "#f56042", "#f57e42", "#f59c42", "#f5b942", "#f5d742", "#f5f542", "#d7f542", "#b9f542", "#9cf542", "#7ef542", "#60f542", "#42f542", "#42f560", "#42f57b", "#42f599", "#42f5b6", "#42f5d4", "#42f5f2", "#42ddf5", "#42bcf5", "#429ef5", "#4281f5", "#4263f5", "#4245f5", "#5a42f5", "#7842f5", "#9642f5", "#b342f5", "#d142f5", "#ef42f5", "#f542dd", "#f542c2", "#f542aa", "#f5428d", "#f5426f", "#f54251" };
			while (!Round.IsStarted)
			{
				messageUp.Clear();
				messageDown.Clear();

				messageUp.Append("<size=40><color=yellow><b>The game will be starting soon, %seconds</b></color></size>");

				short NetworkTimer = GameCore.RoundStart.singleton.NetworkTimer;

				switch (NetworkTimer)
				{
					case -2: messageUp.Replace("%seconds", "Server is paused"); break;

					case -1: messageUp.Replace("%seconds", "Round is being started"); break;

					case 1: messageUp.Replace("%seconds", $"{NetworkTimer} second remain"); break;

					case 0: messageUp.Replace("%seconds", "Round is being started"); break;

					default: messageUp.Replace("%seconds", $"{NetworkTimer} seconds remains"); break;
				}

				messageUp.Append($"\n<size=30><i>%players</i></size>");

				if (Player.List.Count() == 1) messageUp.Replace("%players", $"{Player.List.Count()} player has connected");
				else messageUp.Replace("%players", $"{Player.List.Count()} players have connected");

				messageDown.Append(text.Replace("%rainbow%", colors[x++ % colors.Length]));

				foreach (Player ply in Player.List)
				{
					ply.ShowCenterUpHint(messageUp.ToString());
					ply.ShowCenterDownHint(messageDown.ToString());
					//ply.ShowCenterHint(message.ToString(), 1);
					//ply.Broadcast(1, message.ToString());
				}
				x++;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public static void SpawnRagdoll(Vector3 pos, Quaternion rot, RoleType roleType, string deathCause, Player owner = null)
		{
			ReferenceHub target = owner?.ReferenceHub ?? ReferenceHub.HostHub;
			Exiled.API.Features.Ragdoll.Spawn(new RagdollInfo(target, new CustomReasonDamageHandler(deathCause), pos, rot));
		}

		/// <summary>
		/// Cleans up both all the Ragdolls and all the items in the Map.
		/// </summary>
		public static void CleanupRagdollsAndItems()
		{
			// Cleans all the items
			foreach (ItemPickupBase item in UnityEngine.Object.FindObjectsOfType<ItemPickupBase>())
				item.DestroySelf();

			// Cleans all the ragdolls
			foreach (Ragdoll doll in UnityEngine.Object.FindObjectsOfType<Ragdoll>())
				NetworkServer.Destroy(doll.gameObject);
		}

		/// <summary>
		/// Cleans up all the items in the Map.
		/// </summary>
		public static void CleanupItems()
		{
			// Cleans all the items
			foreach (ItemPickupBase item in UnityEngine.Object.FindObjectsOfType<ItemPickupBase>())
				item.DestroySelf();
		}

		/// <summary>
		/// Cleans all the Ragdolls in the Map.
		/// </summary>
		public static void CleanupRagdolls()
		{
			// Cleans all the ragdolls
			foreach (Ragdoll doll in UnityEngine.Object.FindObjectsOfType<Ragdoll>())
				NetworkServer.Destroy(doll.gameObject);
		}

		/// <summary>
		/// Cleans %50 of common items
		/// </summary>
		public static IEnumerator<float> SoftCleanItems()
		{
			List<Pickup> Radios = new List<Pickup>();
			List<Pickup> Keycards = new List<Pickup>();
			List<Pickup> Armor = new List<Pickup>();

			foreach (var p in Map.Pickups)
			{
				switch (p.Type)
				{
					case ItemType.ArmorCombat:
						Armor.Add(p);
						break;
					case ItemType.Radio:
						Radios.Add(p);
						break;
					case ItemType.KeycardNTFLieutenant:
						Keycards.Add(p);
						break;
				}
			}

			if(Radios.Count > 10)
			{
				for (int i = 0; i < 3 + Radios.Count/2; i++)
				{
					var item = Radios.PickRandom();
					Radios.Remove(item);
					item.Destroy();
					Log.Info(i);
					yield return Timing.WaitForSeconds(0.1f);
				}
			}
			
			if(Keycards.Count > 10)
			{
				for (int i = 0; i < 3 + Keycards.Count/2; i++)
				{
					var item = Keycards.PickRandom();
					Keycards.Remove(item);
					item.Destroy();
					yield return Timing.WaitForSeconds(0.1f);
				}
			}
			
			if(Armor.Count > 10)
			{
				for (int i = 0; i < 3 + Armor.Count/2; i++)
				{
					var item = Armor.PickRandom();
					Armor.Remove(item);
					item.Destroy();
					yield return Timing.WaitForSeconds(0.1f);
				}
			}

			yield return Timing.WaitForSeconds(1);
		}

		public static IEnumerator<float> DensifyAmmoBoxes(SpawningRagdollEventArgs ev)
		{
			yield return Timing.WaitForSeconds(0.2f);
			
			bool AmmoBoxes = true;
			List<Pickup> Ammo9 = new List<Pickup>();
			List<Pickup> Ammo556 = new List<Pickup>();
			List<Pickup> Ammo762 = new List<Pickup>();
			List<Pickup> Ammo44 = new List<Pickup>();
			List<Pickup> Ammo12 = new List<Pickup>();


			foreach (var pickup in Map.Pickups)
			{
				if (Vector3.Distance(ev.Position, pickup.Position) < 4)
				{
					switch (pickup.Type)
					{
						case ItemType.Ammo9x19:
							Ammo9.Add(pickup);
							AmmoBoxes = true;
							break;
							
						case ItemType.Ammo12gauge:
							Ammo12.Add(pickup);
							AmmoBoxes = true;
							break;
							
						case ItemType.Ammo44cal:
							Ammo44.Add(pickup);
							AmmoBoxes = true;
							break;
							
						case ItemType.Ammo556x45:
							Ammo556.Add(pickup);
							AmmoBoxes = true;
							break;
							
						case ItemType.Ammo762x39:
							Ammo762.Add(pickup);
							AmmoBoxes = true;
							break;
					}
				}
			}

			if (!AmmoBoxes) yield break;
			// 9mm
			if (Ammo9.Count > 1)
			{
				Vector3 MeanVector = Vector3.zero;

				foreach (var box in Ammo9)
					MeanVector += box.Position;

				MeanVector = MeanVector / Ammo9.Count;


				Pickup AuthorityBox = Ammo9[0];
				foreach (var box in Ammo9)
					if (Vector3.Distance(box.Position, MeanVector) < Vector3.Distance(AuthorityBox.Position, MeanVector))
						AuthorityBox = box;	

				Ammo9.Remove(AuthorityBox);

				AmmoPickup AuthorityAmmoPickup = (AmmoPickup) AuthorityBox.Base;

				ushort Amount = 0;
				foreach (var box in Ammo9)
				{
					var a = (AmmoPickup) box.Base;
					Amount += a.NetworkSavedAmmo;
				}

				AuthorityAmmoPickup.NetworkSavedAmmo += Amount;

				foreach (var box in Ammo9)
				{
					box.Destroy();
				}
			}
			
			// 5.56mm
			if (Ammo556.Count > 1)
			{
				Vector3 MeanVector = Vector3.zero;

				foreach (var box in Ammo556)
					MeanVector += box.Position;

				MeanVector = MeanVector / Ammo9.Count;


				Pickup AuthorityBox = Ammo556[0];
				foreach (var box in Ammo556)
					if (Vector3.Distance(box.Position, MeanVector) < Vector3.Distance(AuthorityBox.Position, MeanVector))
						AuthorityBox = box;	

				Ammo556.Remove(AuthorityBox);

				AmmoPickup AuthorityAmmoPickup = (AmmoPickup) AuthorityBox.Base;

				ushort Amount = 0;
				foreach (var box in Ammo556)
				{
					var a = (AmmoPickup) box.Base;
					Amount += a.NetworkSavedAmmo;
				}

				AuthorityAmmoPickup.NetworkSavedAmmo += Amount;

				foreach (var box in Ammo556)
				{
					box.Destroy();
				}
			}
			
			// 7.62mm
			if (Ammo762.Count > 1)
			{
				Vector3 MeanVector = Vector3.zero;

				foreach (var box in Ammo762)
					MeanVector += box.Position;

				MeanVector = MeanVector / Ammo762.Count;


				Pickup AuthorityBox = Ammo762[0];
				foreach (var box in Ammo762)
					if (Vector3.Distance(box.Position, MeanVector) < Vector3.Distance(AuthorityBox.Position, MeanVector))
						AuthorityBox = box;	

				Ammo762.Remove(AuthorityBox);

				AmmoPickup AuthorityAmmoPickup = (AmmoPickup) AuthorityBox.Base;

				ushort Amount = 0;
				foreach (var box in Ammo762)
				{
					var a = (AmmoPickup) box.Base;
					Amount += a.NetworkSavedAmmo;
				}

				AuthorityAmmoPickup.NetworkSavedAmmo += Amount;

				foreach (var box in Ammo762)
				{
					box.Destroy();
				}
			}
			
			// .44 cal
			if (Ammo44.Count > 1)
			{
				Vector3 MeanVector = Vector3.zero;

				foreach (var box in Ammo44)
					MeanVector += box.Position;

				MeanVector = MeanVector / Ammo44.Count;


				Pickup AuthorityBox = Ammo44[0];
				foreach (var box in Ammo44)
					if (Vector3.Distance(box.Position, MeanVector) < Vector3.Distance(AuthorityBox.Position, MeanVector))
						AuthorityBox = box;	

				Ammo44.Remove(AuthorityBox);

				AmmoPickup AuthorityAmmoPickup = (AmmoPickup) AuthorityBox.Base;

				ushort Amount = 0;
				foreach (var box in Ammo44)
				{
					var a = (AmmoPickup) box.Base;
					Amount += a.NetworkSavedAmmo;
				}

				AuthorityAmmoPickup.NetworkSavedAmmo += Amount;

				foreach (var box in Ammo44)
				{
					box.Destroy();
				}
			}
			
			// 12 Guage
			if (Ammo12.Count > 1)
			{
				Vector3 MeanVector = Vector3.zero;

				foreach (var box in Ammo12)
					MeanVector += box.Position;

				MeanVector = MeanVector / Ammo12.Count;


				Pickup AuthorityBox = Ammo12[0];
				foreach (var box in Ammo12)
					if (Vector3.Distance(box.Position, MeanVector) < Vector3.Distance(AuthorityBox.Position, MeanVector))
						AuthorityBox = box;	

				Ammo12.Remove(AuthorityBox);

				AmmoPickup AuthorityAmmoPickup = (AmmoPickup) AuthorityBox.Base;

				ushort Amount = 0;
				foreach (var box in Ammo12)
				{
					var a = (AmmoPickup) box.Base;
					Amount += a.NetworkSavedAmmo;
				}

				AuthorityAmmoPickup.NetworkSavedAmmo += Amount;

				foreach (var box in Ammo12)
				{
					box.Destroy();
				}
			}
		}
		
		public static IEnumerator<float> DensifyAmmoBoxes(DroppingAmmoEventArgs ev)
		{
			yield return Timing.WaitForSeconds(0.2f);
			
			bool AmmoBoxes = false;
			List<Pickup> Ammo9 = new List<Pickup>();
			List<Pickup> Ammo556 = new List<Pickup>();
			List<Pickup> Ammo762 = new List<Pickup>();
			List<Pickup> Ammo44 = new List<Pickup>();
			List<Pickup> Ammo12 = new List<Pickup>();


			foreach (var pickup in Map.Pickups)
			{
				if (Vector3.Distance(ev.Player.Position, pickup.Position) < 4)
				{
					switch (pickup.Type)
					{
						case ItemType.Ammo9x19:
							Ammo9.Add(pickup);
							AmmoBoxes = true;
							Log.Info("9 mm");
							break;

						case ItemType.Ammo12gauge:
							Ammo12.Add(pickup);
							AmmoBoxes = true;
							Log.Info("12 gauge");
							break;

						case ItemType.Ammo44cal:
							Ammo44.Add(pickup);
							AmmoBoxes = true;
							Log.Info("44 cal");
							break;

						case ItemType.Ammo556x45:
							Ammo556.Add(pickup);
							AmmoBoxes = true;
							Log.Info("556 ammo");
							break;

						case ItemType.Ammo762x39:
							Ammo762.Add(pickup);
							AmmoBoxes = true;
							Log.Info("762 ammo");
							break;
					}
				}
			}

			if (!AmmoBoxes) yield break;
			// 9mm
			if (Ammo9.Count > 1)
			{
				Vector3 MeanVector = Vector3.zero;

				foreach (var box in Ammo9)
					MeanVector += box.Position;

				MeanVector = MeanVector / Ammo9.Count;


				Pickup AuthorityBox = Ammo9[0];
				foreach (var box in Ammo9)
					if (Vector3.Distance(box.Position, MeanVector) <
					    Vector3.Distance(AuthorityBox.Position, MeanVector))
						AuthorityBox = box;

				Ammo9.Remove(AuthorityBox);

				AmmoPickup AuthorityAmmoPickup = (AmmoPickup) AuthorityBox.Base;

				ushort Amount = 0;
				foreach (var box in Ammo9)
				{
					var a = (AmmoPickup) box.Base;
					Amount += a.NetworkSavedAmmo;
				}

				AuthorityAmmoPickup.NetworkSavedAmmo += Amount;

				foreach (var box in Ammo9)
				{
					box.Destroy();
				}
			}

			// 5.56mm
			if (Ammo556.Count > 1)
			{
				Vector3 MeanVector = Vector3.zero;

				foreach (var box in Ammo556)
					MeanVector += box.Position;

				MeanVector /= Ammo9.Count;


				Pickup AuthorityBox = Ammo556[0];
				foreach (var box in Ammo556)
					if (Vector3.Distance(box.Position, MeanVector) <
					    Vector3.Distance(AuthorityBox.Position, MeanVector))
						AuthorityBox = box;

				Ammo556.Remove(AuthorityBox);

				AmmoPickup AuthorityAmmoPickup = (AmmoPickup) AuthorityBox.Base;

				ushort Amount = 0;
				foreach (var box in Ammo556)
				{
					var a = (AmmoPickup) box.Base;
					Amount += a.NetworkSavedAmmo;
				}

				AuthorityAmmoPickup.NetworkSavedAmmo += Amount;

				foreach (var box in Ammo556)
				{
					box.Destroy();
				}
			}

			// 7.62mm
			if (Ammo762.Count > 1)
			{
				Vector3 MeanVector = Vector3.zero;

				foreach (var box in Ammo762)
					MeanVector += box.Position;

				MeanVector = MeanVector / Ammo762.Count;


				Pickup AuthorityBox = Ammo762[0];
				foreach (var box in Ammo762)
					if (Vector3.Distance(box.Position, MeanVector) <
					    Vector3.Distance(AuthorityBox.Position, MeanVector))
						AuthorityBox = box;

				Ammo762.Remove(AuthorityBox);

				AmmoPickup AuthorityAmmoPickup = (AmmoPickup) AuthorityBox.Base;

				ushort Amount = 0;
				foreach (var box in Ammo762)
				{
					var a = (AmmoPickup) box.Base;
					Amount += a.NetworkSavedAmmo;
				}

				AuthorityAmmoPickup.NetworkSavedAmmo += Amount;

				foreach (var box in Ammo762)
				{
					box.Destroy();
				}
			}

			// .44 cal
			if (Ammo44.Count > 1)
			{
				Vector3 MeanVector = Vector3.zero;

				foreach (var box in Ammo44)
					MeanVector += box.Position;

				MeanVector = MeanVector / Ammo44.Count;


				Pickup AuthorityBox = Ammo44[0];
				foreach (var box in Ammo44)
					if (Vector3.Distance(box.Position, MeanVector) <
					    Vector3.Distance(AuthorityBox.Position, MeanVector))
						AuthorityBox = box;

				Ammo44.Remove(AuthorityBox);

				AmmoPickup AuthorityAmmoPickup = (AmmoPickup) AuthorityBox.Base;

				ushort Amount = 0;
				foreach (var box in Ammo44)
				{
					var a = (AmmoPickup) box.Base;
					Amount += a.NetworkSavedAmmo;
				}

				AuthorityAmmoPickup.NetworkSavedAmmo += Amount;

				foreach (var box in Ammo44)
				{
					box.Destroy();
				}
			}

			// 12 Guage
			if (Ammo12.Count > 1)
			{
				Vector3 MeanVector = Vector3.zero;

				foreach (var box in Ammo12)
					MeanVector += box.Position;

				MeanVector = MeanVector / Ammo12.Count;


				Pickup AuthorityBox = Ammo12[0];
				foreach (var box in Ammo12)
					if (Vector3.Distance(box.Position, MeanVector) <
					    Vector3.Distance(AuthorityBox.Position, MeanVector))
						AuthorityBox = box;

				Ammo12.Remove(AuthorityBox);

				AmmoPickup AuthorityAmmoPickup = (AmmoPickup) AuthorityBox.Base;

				ushort Amount = 0;
				foreach (var box in Ammo12)
				{
					var a = (AmmoPickup) box.Base;
					Amount += a.NetworkSavedAmmo;
				}

				AuthorityAmmoPickup.NetworkSavedAmmo += Amount;

				foreach (var box in Ammo12)
				{
					box.Destroy();
				}
			}
		}
		
		/// <summary>
		/// Changes their PlayerInfo based on params
		/// </summary>
		/// <param name="Ply">Player object</param>
		/// <param name="text">Text to show</param>
		/// <param name="size">Size of badge default 25</param>
		/// <param name="colour">Color used</param>
		public static void GiveCustomPlayerInfo(Player Ply, string text, int size, string colour)
		{
			switch (colour)
			{
				case "mint":
					colour = "98FB98";
					break;

				case "army_green":
					colour = "4B5320";
					break;

				case "yellow":
					colour = "FAFF86";
					break;
			}
			switch (size)
			{
				case -1:
					size = 25;
					break;
			}
			Ply.ReferenceHub.nicknameSync.Network_customPlayerInfoString = $"<size=\"{size}\"><color=#{colour}>{text}</color></size>";
		}

		/// <summary>
		/// Jails a player
		/// </summary>
		/// <param name="player"></param>
		/// <param name="skipadd"></param>
		/// <returns></returns>
		public static IEnumerator<float> DoJail(Player player, bool skipadd = false)
		{
			List<Item> items = new List<Item>();
			Dictionary<AmmoType, ushort> ammo = new Dictionary<AmmoType, ushort>();
			foreach (KeyValuePair<ItemType, ushort> kvp in player.Ammo)

				ammo.Add(kvp.Key.GetAmmoType(), kvp.Value);
			foreach (Item item in player.Items)
				items.Add(item);
			if (!skipadd)
			{
				EventHandler.JailedPlayers.Add(new Jailed
				{
					Health = player.Health,
					Position = player.Position,
					Items = items,
					Name = player.Nickname,
					Role = player.Role.Type,
					Userid = player.UserId,
					CurrentRound = true,
					Ammo = ammo,
					SCP207Intensity = player.ReferenceHub.playerEffectsController.GetEffect<Scp207>()._intensity
				});
			}

			if (player.IsOverwatchEnabled)
				player.IsOverwatchEnabled = false;

			yield return Timing.WaitForSeconds(1f);
			player.ClearInventory(false);
			player.Role.Type = RoleType.Tutorial;
			player.Position = new Vector3(53f, 1020f, -44f);
		}


		/// <summary>
		/// Unjails a player
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static IEnumerator<float> DoUnJail(Player player)
		{
			Jailed jail = EventHandler.JailedPlayers.Find(j => j.Userid == player.UserId);
			if (jail.CurrentRound)
			{
				player.SetRole(jail.Role, SpawnReason.ForceClass, true);
				yield return Timing.WaitForSeconds(0.5f);
				player.ResetInventory(jail.Items);
				player.Health = jail.Health;
				player.Position = jail.Position;
				foreach (KeyValuePair<AmmoType, ushort> kvp in jail.Ammo)
					player.Ammo[kvp.Key.GetItemType()] = kvp.Value;

				player.ReferenceHub.playerEffectsController.ChangeEffectIntensity<Scp207>(jail.SCP207Intensity);
			}
			else
			{
				player.Role.Type = RoleType.Spectator;
			}
			EventHandler.JailedPlayers.Remove(jail);
		}

		/// <summary>
		/// Adds the rainbow controller for a given Player object
		/// </summary>
		/// <param name="Ply"></param>
		public static void AddRainbowController(Player Ply)
		{
			if (Ply.ReferenceHub.TryGetComponent(out PlayhousePlugin RainbowTagCtrl))
				return;

			Ply.GameObject.AddComponent<RainbowTagController>();
		}


		/// <summary>
		/// Finds the preference of a certain player
		/// </summary>
		/// <param name="Ply"></param>
		/// <returns></returns>
		public static ItemType FindPreference(Player Ply)
		{
			try
			{
				string text = System.IO.File.ReadAllText($@"/home/ubuntu/.config/EXILED/Configs/Pets/PetPreference/{Ply.UserId}");
				return Commands.Pets.Items[text];
			}
			catch
			{
				return ItemType.None;
			}

		}

		/// <summary>
		/// Finds the preference of a certain player
		/// </summary>
		/// <param name="Ply"></param>
		/// <returns></returns>
		public static string FindPreferenceRaw(Player Ply)
		{
			try
			{
				return File.ReadAllText($@"/home/ubuntu/.config/EXILED/Configs/Pets/PetPreference/{Ply.UserId}");
			}
			catch
			{
				return "";
			}

		}

		public static int GetDonatorNum(string itemNum)
		{
			return int.Parse($"{itemNum[0]}");
		}

		/// <summary>
		/// Updates the Pet Preference for a certain player
		/// </summary>
		/// <param name="Ply"></param>
		/// <param name="itemNum"></param>
		public static void UpdatePreference(Player Ply, string itemNum)
		{
			System.IO.File.WriteAllText($@"/home/ubuntu/.config/EXILED/Configs/Pets/PetPreference/{Ply.UserId}", itemNum);
		}

		/// <summary>
		/// Quite simply it checks if they already have a hat that is following them and kill the coroutine responsible for it.
		/// </summary>
		/// <param name="ev"></param>
		public static void CheckExistingSpawnedHatAndKill(string UserId)
		{
			Hat.KillHat(Player.Get(UserId));
		}


		/// <summary>
		/// Checks if a given UserId has a pet, if so kill the coroutine responsible for it.
		/// </summary>
		/// <param name="UserId"></param>
		public static void CheckExistingPetAndKill(string UserId)
		{
			if (PetFollow.Coroutines.ContainsKey(UserId))
			{
				PetFollow.KillPet(Player.Get(UserId));
			}
		}

		public static void SpawnDummyModel(Player Ply, Vector3 position, Quaternion rotation, RoleType role, float x, float y, float z)
		{
			GameObject obj = UnityEngine.Object.Instantiate(
										LiteNetLib4MirrorNetworkManager.singleton.playerPrefab);
			CharacterClassManager ccm = obj.GetComponent<CharacterClassManager>();
			if (ccm == null)
				Log.Error("CCM is null, this can cause problems!");
			ccm.CurClass = role;
			ccm.GodMode = true;
			//ccm.OldRefreshPlyModel(PlayerManager.localPlayer);
			obj.GetComponent<NicknameSync>().Network_myNickSync = "Dummy";
			obj.GetComponent<QueryProcessor>().PlayerId = 9999;
			obj.GetComponent<QueryProcessor>().NetworkPlayerId = 9999;
			obj.transform.localScale = new Vector3(x, y, z);

			obj.transform.position = position;
			obj.transform.rotation = rotation;

			NetworkServer.Spawn(obj);
			/*
			if (Plugin.DumHubs.TryGetValue(Ply, out List<GameObject> objs))
			{
				objs.Add(obj);
			}
			else
			{
				Plugin.DumHubs.Add(Ply, new List<GameObject>());
				Plugin.DumHubs[Ply].Add(obj);
				DummyIndex = Plugin.DumHubs[Ply].Count();
			}
			if (DummyIndex != 1)
				DummyIndex = objs.Count();*/
		}
		
		public static void SpawnTempDummy(Player Ply, Vector3 position, Quaternion rotation, RoleType role, float x, float y, float z)
		{
			GameObject obj = UnityEngine.Object.Instantiate(
				LiteNetLib4MirrorNetworkManager.singleton.playerPrefab);
			CharacterClassManager ccm = obj.GetComponent<CharacterClassManager>();
			if (ccm == null)
				Log.Error("CCM is null, this can cause problems!");
			ccm.CurClass = role;
			ccm.GodMode = true;
			//ccm.OldRefreshPlyModel(PlayerManager.localPlayer);
			obj.GetComponent<NicknameSync>().Network_myNickSync = "Dummy";
			obj.GetComponent<QueryProcessor>().PlayerId = 9999;
			obj.GetComponent<QueryProcessor>().NetworkPlayerId = 9999;
			obj.transform.localScale = new Vector3(x, y, z);

			obj.transform.position = position;
			obj.transform.rotation = rotation;

			NetworkServer.Spawn(obj);
			Timing.CallDelayed(3, () => { NetworkServer.Destroy(obj); });
		}

		/// <summary>
		/// Spawns a grenade with a very short fuse to explode, kill and damage enviroment
		/// </summary>
		/// <param name="player"></param>
		public static void Explode(Player player)
		{
			var grenade = (ExplosiveGrenade) ExplosiveGrenade.Create(ItemType.GrenadeHE, player);
			grenade.PinPullTime = 0.1f;
			grenade.FuseTime = 0.2f;
			grenade.SpawnActive(player.Position, player);
			player.Hurt(new ExplosionDamageHandler(new Footprint(player.ReferenceHub), Vector3.one*10, float.MaxValue, 100));
		}
		
		/// <summary>
		/// RNG generates a random number with the one inputted being the max. 1 = 100%, 2 = 50%, 3 = 33%, etc.
		/// </summary>
		/// <param name="chance"></param>
		/// <returns>True if it generates the same, otherwise false.</returns>
		public static bool RandomChance(int chance)
		{
			return chance == EventHandler.random.Next(chance)+1;
		}
		
		public static void SpawnDebugPrimitive(Vector3 pos)
		{
			var circle = UnityEngine.Object.Instantiate(Utils.PrimitiveBaseObject);
			circle.NetworkPrimitiveType = PrimitiveType.Cylinder;
			circle.NetworkMaterialColor = new Color(1, 0, 0 );
			circle.NetworkMovementSmoothing = 60;
			circle.transform.position = pos;
			NetworkServer.Spawn(circle.gameObject);
			circle.UpdatePositionServer();
		}

		/// <summary>
		/// Spawns a fake grenade for cosmetic effects
		/// </summary>
		/// <param name="player"></param>
		public static void FakeExplode(Player player)
		{
			var grenade = (ExplosiveGrenade) ExplosiveGrenade.Create(ItemType.GrenadeHE, player);

			grenade.PinPullTime = 0.1f;
			grenade.FuseTime = 0.2f;
			grenade.MaxRadius = 0f;
			
			EventHandler.GrenadesToFake.Add(grenade.Base.Projectile);
			grenade.SpawnActive(player.Position);
		}

		/// <summary>
		/// Spawns a fake grenade or flashbang for cosmetic effects
		/// </summary>
		/// <param name="player"></param>
		public static void FakeExplodeAtPosition(Vector3 pos, bool flash = false)
		{
			if (flash)
			{
				var flashGren = (FlashGrenade)FlashGrenade.Create(ItemType.GrenadeFlash);
				flashGren.PinPullTime = 0.1f;
				flashGren.FuseTime = 0.2f;
				flashGren.BlindCurve = AnimationCurve.Constant(0f, float.MaxValue, 0f);
				EventHandler.GrenadesToFake.Add(flashGren.Base.Projectile);
				flashGren.SpawnActive(pos);

			}
			else
			{
				var grenade = (ExplosiveGrenade) ExplosiveGrenade.Create(ItemType.GrenadeHE);
				grenade.PinPullTime = 0.1f;
				grenade.FuseTime = 0.2f;
				grenade.MaxRadius = 0f;
				EventHandler.GrenadesToFake.Add(grenade.Base.Projectile);
				grenade.SpawnActive(pos);
			}
		}
		
		public static IEnumerator<float> DeathSequence(Player boss)
		{
			boss.NoClipEnabled = true;
			
			yield return Timing.WaitForSeconds(0.5f);

			FakeExplode(boss);

			yield return Timing.WaitForSeconds(1f);

			FakeExplode(boss);

			Timing.RunCoroutine(SinkInGround(boss));

			while (boss.IsAlive)
			{
				yield return Timing.WaitForSeconds(Convert.ToSingle(EventHandler.random.NextDouble()) + 0.4f);
				FakeExplode(boss);
			}
		}

		public static IEnumerator<float> SinkInGround(Player boss)
		{
			while (boss.IsAlive)
			{
				boss.Position += Vector3.down*0.2f;
				yield return Timing.WaitForSeconds(0.1f);
			}
		}
		
		public static bool IsAlphaWarheadCountdown()
		{
			return AlphaWarheadController.Host.timeToDetonation < 
			       AlphaWarheadController.Host.RealDetonationTime() - 
			       ((AlphaWarheadController._resumeScenario >= 0) 
				       ? AlphaWarheadController.Host.scenarios_resume[AlphaWarheadController._resumeScenario].additionalTime 
				       : AlphaWarheadController.Host.scenarios_start[AlphaWarheadController._startScenario].additionalTime);
		}

		public static void RewardPlayers()
		{
			if (EventHandler.RewardPlayers)
			{
				foreach (Player Ply in Player.List)
				{
					string myfile = $"/home/ubuntu/klpBot/RoundClaimsIDs/{Ply.UserId}";

					// Appending the given texts 
					using (StreamWriter sw = File.AppendText(myfile))
					{
						sw.WriteLine("roundPlayed");
					}
				}
			}
		}
	}
}
