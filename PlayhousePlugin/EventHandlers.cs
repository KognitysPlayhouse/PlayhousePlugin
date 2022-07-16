using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MEC;
using System.Net.Http;
using CustomPlayerEffects;
using Mirror;
using Interactables.Interobjects.DoorUtils;
using System.Text;
using System.Threading.Tasks;
using AdminToys;
using Respawning;
using Exiled.Loader;
using Exiled.API.Features.Items;
using GameCore;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror.LiteNetLib4Mirror;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Usables.Scp330;
using Kognity.DB.Account;
using Kognity.DB.Account.Components;
using Kognity.DB.Common.Cache;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using PlayerStatsSystem;
using PlayhousePlugin.Commands;
using Respawning.NamingRules;
using PlayhousePlugin.Components;
using PlayhousePlugin.Controllers;
using PlayhousePlugin.CustomClass;
using PlayhousePlugin.CustomClass.Abilities;
using PlayhousePlugin.CustomClass.SCP;
using PlayhousePlugin.CustomClass.SCP_Abilities;
using PlayhousePlugin.CustomGameMode;
using Unity.Mathematics;
using WebSocketSharp;
using Log = Exiled.API.Features.Log;

namespace PlayhousePlugin
{
	public class EventHandler
	{
		public PlayhousePlugin plugin;
		public EventHandler(PlayhousePlugin plugin) => this.plugin = plugin;

		public static Stream stream;
		public static Pickup Radio;

		public static Vector3 SpawnPoint = new Vector3(240.1f,977,95.8f);

		public static Vector3 ClassDPoint = new Vector3(245, 980, 81.6f);
		public static Vector3 GuardPoint = new Vector3(237, 980, 81.7f);
		public static Vector3 Tutorial = new Vector3(228, 980, 87.6f);
		public static Vector3 SCPPoint = new Vector3(223, 980, 99);
		public static Vector3 ScientistPoint = new Vector3(226, 980, 107);

		static StringBuilder text = new StringBuilder();
		public static System.Random random = new System.Random();

		public Dictionary<Player, Player> OngoingReqs = new Dictionary<Player, Player>();
		public List<CoroutineHandle> SCPSwapCoroutines = new List<CoroutineHandle>();
		public Dictionary<Player, CoroutineHandle> ReqCoroutines = new Dictionary<Player, CoroutineHandle>();

		public static bool MTFSign = false;
		public static bool CISign = false;
		public static bool allowSwaps = false;

		public SchematicObject lobby;

		public static int deathmatchport = 9999;
		public static bool IsDeathMatchServer => Server.Port == deathmatchport;
		public static bool IsDevServer => Server.Port == 7778;
		public static bool IsEventServer => Server.Port == 8899;
		public static bool RewardPlayers = false;
		public static bool WipeRadio = false;
		public static Stopwatch Stopwatch = new Stopwatch();
		
		public List<Pickup> Letters = new List<Pickup> { };

		public List<Pickup> LetterM = new List<Pickup> { };
		public List<Pickup> LetterT = new List<Pickup> { };
		public List<Pickup> LetterF = new List<Pickup> { };

		public List<Pickup> LetterC = new List<Pickup> { };
		public List<Pickup> LetterI = new List<Pickup> { };

		public List<Pickup> ArtItems = new List<Pickup> { };
		public List<Pickup> ArtItems2 = new List<Pickup> { };
		public List<Pickup> ArtItems3 = new List<Pickup> { };
		public List<Pickup> ArtItems4 = new List<Pickup> { };

		public List<Ragdoll> ArtRagdolls = new List<Ragdoll> { };
		public List<Ragdoll> ArtRagdolls2 = new List<Ragdoll> { };

		public static List<Player> Stunned = new List<Player>();
		
		public static HashSet<Player> PlayersWithInfiniteAmmo = new HashSet<Player>();
		public static StringBuilder PlayerLister = new StringBuilder();

		public List<int> RecentlyUncuffed = new List<int> { };
		public Dictionary<int, int> RecentlyUncuffer = new Dictionary<int, int> { };

		public static Dictionary<Player, Pair> ContentGun = new Dictionary<Player, Pair>();
		public static Dictionary<Player, int> PlayerSprays = new Dictionary<Player, int>();
		public static Dictionary<Player, int> PlayerSpraysFree = new Dictionary<Player, int>();
		public static List<ThrownProjectile> GrenadesToFake = new List<ThrownProjectile>();

		public List<Player> InfectedPlayers = new List<Player> { };

		public static List<Player> DeletePlayerRagdoll = new List<Player>();

		public List<Player> RagdollGun = new List<Player> { };

		//Stickies stuff
		public Dictionary<Player, Pickup[]> StickyPositions = new Dictionary<Player, Pickup[]>();
		public Dictionary<Pickup, int> TempStickies = new Dictionary<Pickup, int>();

		public static List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

		public List<string> JoinedPlayers = new List<string> { };
		public List<string> DoNotSpawn = new List<string> { };

		public static List<Jailed> JailedPlayers = new List<Jailed>();

		public static bool RoundActive = false;

		private static readonly HttpClient client = new HttpClient();

		public List<Player> PlayersWithInfiniteDrop = new List<Player>();
		
		public static bool SillySunday = false;

		// Escape zone area
		private static Vector3 escapeArea = new Vector3(177.5f, 985.0f, 29.0f);

		public Dictionary<Player, int> kills = new Dictionary<Player, int> { };
		public Dictionary<Player, int> deaths = new Dictionary<Player, int> { };
		public Dictionary<Player, int> damageDealt = new Dictionary<Player, int> { };
		public Dictionary<Player, int> escapes = new Dictionary<Player, int> { };
		public Dictionary<Player, int> dmWins = new Dictionary<Player, int> { };
		public Dictionary<Player, int> dmLosses = new Dictionary<Player, int> { };
		public Dictionary<Player, int> medItemsUsed = new Dictionary<Player, int> { };
		public Dictionary<Player, int> scpItemsUsed = new Dictionary<Player, int> { };
		public Dictionary<Player, int> classDRescued = new Dictionary<Player, int> { };
		public Dictionary<Player, int> scientistsRescued = new Dictionary<Player, int> { };
		public Dictionary<Player, int> mtfAndChaosConverted = new Dictionary<Player, int> { };
		public Dictionary<Player, int> lastResortConverted = new Dictionary<Player, int> { };
		public Dictionary<Player, int> killBindsUsed = new Dictionary<Player, int> { };
		public List<Player> joins = new List<Player> { };
		public List<Player> deletes = new List<Player> { };

		public Dictionary<Player, int> GeneralKills = new Dictionary<Player, int> { };
		public Dictionary<Player, int> SCPKills = new Dictionary<Player, int> { };
		public Dictionary<Player, int> HumanKills = new Dictionary<Player, int> { };
		public Dictionary<string, int> MTFSquadKills = new Dictionary<string, int> { };

		public WebSocket ws = new WebSocket("ws://127.0.0.1:8765");

		public void ConnectToServer()
		{
			ws.Connect();
		}

		public void SetUpWebsockets()
		{
			ws.OnMessage += Ws_OnMessage;
			ws.OnError += Ws_OnError;
			ws.OnClose += Ws_OnClose;

		}

		private void Ws_OnClose(object sender, CloseEventArgs e)
		{
			ws.Connect();
		}

		private void Ws_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
		{
			Log.Error(e.Message);
		}

		private void Ws_OnMessage(object sender, MessageEventArgs e)
		{
			Log.Info("EEEEEEEEEEEEEEEEEEEE");
			Log.Info(e.Data);
		}

		public void MapDonatorToObjects()
		{
			using (StreamReader reader = new StreamReader(@"/home/ubuntu/.config/EXILED/Configs/Donators/donators.csv"))
			{
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					string[] values = line.Split(',');

					Donator tempDonator = new Donator
					{
						UserId = values[0],
						DonatorNum = int.Parse(values[1]),
						IsBooster = values[2] == "1"
					};
					Donator.Donators.Add(tempDonator);
				}
			}

		}

		public void OnReloading(ReloadingWeaponEventArgs ev)
		{
			if(SillySundayEventHandler.NerfWarMode || SillySundayEventHandler.slaughterhouse || SillySundayEventHandler.ohfiverescuemode)
				ev.Firearm.Ammo = Byte.MaxValue;
		}

		public void RoundRestart()
		{
			ClearLists();
			Log.Info("restarting shits");
			ConfigManager.ReloadRemoteAdmin();
			ConfigManager.Reload();
			GameCore.ConfigFile.ReloadGameConfigs(false);
			ReservedSlot.Reload();

			foreach (var thing in coroutines)
			{
				if (thing.IsRunning)
				{
					Timing.KillCoroutines(thing);
				}
			}

			foreach(var player in Player.List)
				if(player.GameObject.TryGetComponent<PlayhousePluginComponent>(out var comp))
					UnityEngine.Object.Destroy(comp);
		}

		public void RoundEnding(EndingRoundEventArgs ev)
		{
			if (ev.IsRoundEnded)
			{
				if(IsEventServer)
					Stopwatch.Stop();
				
				// I am aware you can do it with  Server.FriendlyFire but I don't want the server to show up as a friendly fire server in the server list.
				Server.FriendlyFire = true;
				foreach (Player Ply in Player.List)
					Ply.IsFriendlyFireEnabled = true;

				UtilityMethods.CleanupRagdollsAndItems();
				UtilityMethods.RewardPlayers();
				
				RoundActive = false;

				Timing.KillCoroutines(ElevatorController.CoroutineHandle);
				
				foreach (var door in Door.List)
					door.Unlock();

				foreach (var room in Room.List)
					room.ResetColor();

				if (!IsEventServer)
				{
					if (GeneralKills.Count == 0)
					{
						Map.Broadcast(10, "<size=100><b><color=#FF69B4>No one got any kills!</color></b></size>");
					}
					else
					{
						string mvpMessage = "";

						var t = GeneralKills.OrderByDescending(x => x.Value).FirstOrDefault();
						mvpMessage +=
							$"<size=30><b>{t.Key.Nickname} with <color=red>{t.Value} Kills</color> in Total</b></size>";

						if (SCPKills.Count != 0)
						{
							var e = SCPKills.OrderByDescending(x => x.Value).FirstOrDefault();
							mvpMessage +=
								$"<size=1>\n</size><size=30><b>{e.Key.Nickname} with <color=red>{e.Value} Kills</color> as SCP</b></size>";
						}

						if (HumanKills.Count != 0)
						{
							var e = HumanKills.OrderByDescending(x => x.Value).FirstOrDefault();
							mvpMessage +=
								$"<size=1>\n</size><size=30><b>{e.Key.Nickname} with <color=red>{e.Value} Kills</color> as a Human</b></size>";
						}

						if (MTFSquadKills.Count != 0)
						{
							var e = MTFSquadKills.OrderByDescending(x => x.Value).FirstOrDefault();
							mvpMessage +=
								$"<size=1>\n</size><size=30><b><color=blue>{e.Key}</color> with <color=red>{e.Value} Kills</color></b></size>";
						}

						Map.Broadcast(10, mvpMessage);
						Log.Info(mvpMessage);
					}
				}

				string messageToSend = "";

				// Excuse the use of %&@&, when I made this I was very new to the whole networking thing so this is the best I could come up with.
				// This sends information to a WebSockets server which processes the data and adds it to a SQLite3 Database
				// And this is fully DNT compliant.
				foreach (Player Ply in joins)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&join%&@&0\n";

				foreach (Player Ply in kills.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&kill%&@&{kills[Ply]}\n";

				foreach (Player Ply in deaths.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&death%&@&{deaths[Ply]}\n";

				foreach (Player Ply in escapes.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&escape%&@&{escapes[Ply]}\n";

				foreach (Player Ply in damageDealt.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&damage%&@&{damageDealt[Ply]}\n";

				foreach (Player Ply in medItemsUsed.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&meditems%&@&{medItemsUsed[Ply]}\n";

				foreach (Player Ply in scpItemsUsed.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&scpitems%&@&{scpItemsUsed[Ply]}\n";

				foreach (Player Ply in classDRescued.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&classdr%&@&{classDRescued[Ply]}\n";

				foreach (Player Ply in scientistsRescued.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&scientistr%&@&{scientistsRescued[Ply]}\n";

				foreach (Player Ply in killBindsUsed.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&killbinds%&@&{killBindsUsed[Ply]}\n";

				foreach (Player Ply in mtfAndChaosConverted.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&mtfciconverted%&@&{mtfAndChaosConverted[Ply]}\n";

				foreach (Player Ply in lastResortConverted.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&lastresortconverted%&@&{lastResortConverted[Ply]}\n";


				if (GeneralKills.Count != 0)
				{
					var t = GeneralKills.OrderByDescending(x => x.Value).ToList();
					foreach(var e in t)
					{
						if (!e.Key.DoNotTrack)
						{
							messageToSend += $"{e.Key.RawUserId}%&@&{e.Key.Nickname}%&@&mvp%&@&{e.Value}\n";
							break;
						}
					}
				}

				if (SCPKills.Count != 0)
				{
					var t = SCPKills.OrderByDescending(x => x.Value).ToList();
					foreach (var e in t)
					{
						if (!e.Key.DoNotTrack)
						{
							messageToSend += $"{e.Key.RawUserId}%&@&{e.Key.Nickname}%&@&mvp%&@&{e.Value}\n";
							break;
						}
					}
				}

				if (HumanKills.Count != 0)
				{
					var t = HumanKills.OrderByDescending(x => x.Value).ToList();
					foreach (var e in t)
					{
						if (!e.Key.DoNotTrack)
						{
							messageToSend += $"{e.Key.RawUserId}%&@&{e.Key.Nickname}%&@&mvp%&@&{e.Value}\n";
							break;
						}
					}
				}

				foreach (Player Ply in deletes)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&delete%&@&0";

				if (ws.IsAlive)
				{
					ws.Send(messageToSend);
				}
				else
				{
					ws.Connect();
					Timing.CallDelayed(0.4f, () =>
					{
						ws.Send(messageToSend);
					});
				}
			}
		}

		private void ClearLists()
		{
			Stunned.Clear();
			KingAndCompetitor.ClearQueues();
			Deathmatch.playerAndWins.Clear();
			Deathmatch.activeMatch.Clear();
			Letters.Clear();
			LetterM.Clear();
			LetterT.Clear();
			LetterF.Clear();
			LetterC.Clear();
			LetterI.Clear();
			ArtItems.Clear();
			ArtItems2.Clear();
			ArtItems3.Clear();
			ArtItems4.Clear();
			ArtRagdolls.Clear();
			ArtRagdolls2.Clear();
			PlayersWithInfiniteAmmo.Clear();
			PlayerLister.Clear();
			RecentlyUncuffed.Clear();
			RecentlyUncuffer.Clear();
			InfectedPlayers.Clear();
			RagdollGun.Clear();
			
			JailedPlayers.Clear();

			ContentGun.Clear();
			PlayerSprays.Clear();
			PlayerSpraysFree.Clear();
			GrenadesToFake.Clear();

			JoinedPlayers.Clear();
			DoNotSpawn.Clear();

			SillySundayEventHandler.ResetToDefaults();
			SillySundayInfectionController.ResetToDefaults();
			
			ObjectivePointController.DestroyObjectives();
			VendingMachineController.DestroyVendingMachines();
			Containment106ObjectiveController.DestroyObjectives();

			RoundActive = false;
			
			Donator.Donators.Clear();
			
			PlayersWithInfiniteDrop.Clear();

			PetFollow.Coroutines.Clear();
			PetFollow.IDsAndPickups.Clear();
			PetFollow.Pets.Clear();
			Hat.KillAllhats();

			SillySunday = false;

			kills.Clear();
			deaths.Clear();
			damageDealt.Clear();
			escapes.Clear();
			joins.Clear();
			deletes.Clear();
			dmWins.Clear();
			dmLosses.Clear();
			medItemsUsed.Clear();
			scpItemsUsed.Clear();
			classDRescued.Clear();
			scientistsRescued.Clear();
			mtfAndChaosConverted.Clear();
			lastResortConverted.Clear();
			killBindsUsed.Clear();

			CommonUtilsStuff.AutoNukeDetonated = false;
			SCP939Consts.killsBy939.Clear();

			MTFSign = false;
			CISign = false;
			allowSwaps = false;
			WipeRadio = false;

			StickyPositions.Clear();
			TempStickies.Clear();

			CustomItemSpawning.Clear();

			GeneralKills.Clear();
			HumanKills.Clear();
			SCPKills.Clear();
			MTFSquadKills.Clear();

			OngoingReqs.Clear();
			ReqCoroutines.Clear();
			SCPSwapCoroutines.Clear();

			DisableBulletHoles.DisableBulletHolesBool = false;
			WipeRadio = false;

			BreakoutBlitz.SCPKills = 0;
			BreakoutBlitz.ClassDEscapes = 0;
			BreakoutBlitz.ScientistEscapes = 0;
			BreakoutBlitz.PickupsToNotClear.Clear();
			
			CustomClassManager.Players.Clear();
		}

		public void WaitingForPlayers()
		{
			Round.IsLocked = true;
			Server.FriendlyFire = false;
			List<string> lines = new List<string> { "<color=orange>discord.gg/kognity</color>" };
			foreach(var line in lines)
			{
				SyncUnit mtfUnit = new SyncUnit
				{
					SpawnableTeam = (byte)SpawnableTeamType.NineTailedFox,
					UnitName = line
				};
				RespawnManager.Singleton.NamingManager.AllUnitNames.Add(mtfUnit);
			}

			if (!IsDeathMatchServer)
			{
				if(!IsEventServer)
				{
					ObjectivePointController.SpawnObjectives();
					Containment106ObjectiveController.SpawnObjectives();
				
					GateAReworkController.Spawn();
					GateBReworkController.Spawn();
				}
				
				RecyclingBinController.SpawnRecyclingBins();
				VendingMachineController.SpawnVendingMachines();
			}

			//if(Server.Port != 7778 && Server.Port != 9999)
				//Timing.RunCoroutine(UtilityMethods.RaimbowHint());

			if (DateTime.Now.DayOfWeek == 0 && (!IsDeathMatchServer))
			{
				SillySunday = true;
			}

			/*
			if(Server.Port == deathmatchport)
			{
				GameCore.Console.singleton.TypeCommand("/mapeditor load tournament");
				Timing.CallDelayed(0.5f, () =>
				{
					foreach(DoorVariant d in Door.List)
					{
						d.ServerChangeLock(DoorLockReason.AdminCommand, true);
					}
				});
			}*/
			
			try
			{
				MapDonatorToObjects();
			}
			catch
			{
				SendMessageAsync($"Error parsing donator info!! Server {Server.Port} <@216304765494099969> PING KOG **AAAAAAAAAAAAAAAA**\nhttps://tenor.com/view/turtwig-pokemon-turtwig-scream-scream-aaaa-gif-20526778", true);
			}

			if (IsDeathMatchServer)
			{
				Deathmatch.SetupArenas();
			}

			GameObject.Find("StartRound").transform.localScale = Vector3.zero;
			coroutines.Add(Timing.RunCoroutine(UtilityMethods.LobbyTimer()));

			lobby = ObjectSpawner.SpawnSchematic("CustomSpawnerLobby", SpawnPoint, Quaternion.identity);
		}		

		public void ChangingItem(ChangingItemEventArgs ev)
		{
			if(Stunned.Contains(ev.Player))
				ev.IsAllowed = false;
		}

		public void OnMedicalItemDequipped(UsedItemEventArgs ev)
		{
			if (!ev.Player.DoNotTrack)
			{
				if (ev.Item.Type == ItemType.SCP207 || ev.Item.Type == ItemType.SCP500)
				{
					if (scpItemsUsed.ContainsKey(ev.Player))
						scpItemsUsed[ev.Player] += 1;
					else
						scpItemsUsed.Add(ev.Player, 1);
				}
				else
				{
					if (medItemsUsed.ContainsKey(ev.Player))
						medItemsUsed[ev.Player] += 1;
					else
						medItemsUsed.Add(ev.Player, 1);
				}
			}

			if (ev.Item.Type == ItemType.Adrenaline && ConfigFile.ServerConfig.GetFloat("stamina_balance_use", 0.05f) == 0)
			{
				ev.Player.EnableEffect<MovementBoost>(1);
				ev.Player.ChangeEffectIntensity(EffectType.MovementBoost,  15, 8);
				ev.Player.ShowCenterDownHint("<color=yellow>+Movement Speed Boost</color>", 4);
			}

			if (ev.Item.Type == ItemType.Medkit || ev.Item.Type == ItemType.SCP500)
			{
				if (InfectedPlayers.Contains(ev.Player))
				{
					ev.Player.ReferenceHub.playerEffectsController.DisableEffect<Poisoned>();
					ev.Player.ReferenceHub.playerEffectsController.DisableEffect<Hemorrhage>();

					InfectedPlayers.Remove(ev.Player);
				}
			}

			if (ev.Item.Type == ItemType.SCP500)
			{
				ev.Player.Health = ev.Player.MaxHealth;
				if (ev.Player.CustomClassManager().CustomClass?.Name == "NTF Scout")
				{
					Timing.CallDelayed(0.2f, () =>
					{
						ev.Player.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();
					});
				}
				if (SillySundayEventHandler.sugarrush)
				{
					Timing.CallDelayed(0.2f, () =>
					{
						ev.Player.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();
						ev.Player.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();
						ev.Player.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();
						ev.Player.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();
					});
				}
			}

			if (ev.Item.Type == ItemType.Medkit && ev.Player.ReferenceHub.playerEffectsController.GetEffect<Hemorrhage>().IsEnabled)
			{
				ev.Player.ReferenceHub.playerEffectsController.DisableEffect<Hemorrhage>();
			}

			if (ev.Item.Type == ItemType.Adrenaline && ev.Item.Type == ItemType.Painkillers)
			{
				if (ev.Player.CustomClassManager().CustomClass?.Name == "Heretic")
				{
					Scp330Bag.AddSimpleRegeneration(ev.Player.ReferenceHub, 5, 10);
				}
			}
		}

		public void OnMute(ChangingMuteStatusEventArgs ev)
		{
			if (ev.Player == null)
				return;
			if (!ev.Player.IsVerified)
				return;
			if (ev.Player.Nickname.IsNullOrEmpty() && ev.Player.UserId.IsNullOrEmpty())
				return;
			Log.Info("A");
			List<Player> RAPermsPlayers = Player.List.Where(r => r.ReferenceHub.serverRoles.RemoteAdmin).ToList();
			Log.Info("B");
			string modsOnline = "";
			foreach (Player Ply in RAPermsPlayers)
			{
				modsOnline += $"{Ply.Nickname} ";
				Log.Info("C");
			}
			if (Server.Port == 7777)
			{
				SendMessageAsync($"Mute Status Change on **Server #1**:\n```Offender: {ev.Player.Nickname} ({ev.Player.UserId})\nMuted?: {ev.IsMuted}\nMods online: {modsOnline}```", true);
			}
			else if (Server.Port == 8888)
			{
				SendMessageAsync($"Mute Status Change on **Server #2**:\n```Offender: {ev.Player.Nickname} ({ev.Player.UserId})\nMuted?: {ev.IsMuted}\nMods online: {modsOnline}```", true);
			}
			else if (Server.Port == 9999)
			{
				SendMessageAsync($"Mute Status Change on **Server #3**:\n```Offender: {ev.Player.Nickname} ({ev.Player.UserId})\nMuted?: {ev.IsMuted}\nMods online: {modsOnline}```", true);
			}
			else if (Server.Port == 7778)
			{
				SendMessageAsync($"Mute Status Change on **Dev Server**:\n```Offender: {ev.Player.Nickname} ({ev.Player.UserId})\nMuted?: {ev.IsMuted}\nMods online: {modsOnline}```", true);
			}
			else
			{
				SendMessageAsync($"Mute Status Change on **Server with port: {Server.Port}**:\n```Offender: {ev.Player.Nickname} ({ev.Player.UserId})\nMuted?: {ev.IsMuted}\nMods online: {modsOnline}```", true);
			}
		}

		public void OnIntercomMute(ChangingIntercomMuteStatusEventArgs ev)
		{
			if (ev.Player == null)
				return;
			if (!ev.Player.IsVerified)
				return;
			if (ev.Player.Nickname.IsNullOrEmpty() && ev.Player.UserId.IsNullOrEmpty())
				return;
			List<Player> RAPermsPlayers = Player.List.Where(r => r.ReferenceHub.serverRoles.RemoteAdmin).ToList();
			string modsOnline = "";
			foreach (Player Ply in RAPermsPlayers)
			{
				modsOnline += $"{Ply.Nickname} ";
			}
			if (Server.Port == 7777)
			{
				SendMessageAsync($"Intercom Mute Status Change on **Server #1**:\n```Offender: {ev.Player.Nickname} ({ev.Player.UserId})\nMuted?: {ev.IsMuted}\nMods online: {modsOnline}```", true);
			}
			else if (Server.Port == 8888)
			{
				SendMessageAsync($"Intercom Mute Status Change on **Server #2**:\n```Offender: {ev.Player.Nickname} ({ev.Player.UserId})\nMuted?: {ev.IsMuted}\nMods online: {modsOnline}```", true);
			}
			else if (Server.Port == 9999)
			{
				SendMessageAsync($"Intercom Mute Status Change on **Server #3**:\n```Offender: {ev.Player.Nickname} ({ev.Player.UserId})\nMuted?: {ev.IsMuted}\nMods online: {modsOnline}```", true);
			}
			else if (Server.Port == 7778)
			{
				SendMessageAsync($"Intercom Mute Status Change on **Dev Server**:\n```Offender: {ev.Player.Nickname} ({ev.Player.UserId})\nMuted?: {ev.IsMuted}\nMods online: {modsOnline}```", true);
			}
			else
			{
				SendMessageAsync($"Intercom Mute Status Change on **Server with port: {Server.Port}**:\n```Offender: {ev.Player.Nickname} ({ev.Player.UserId})\nMods online: {modsOnline}```", true);
			}
		}

		public void OnRaging(EnragingEventArgs ev)
		{
			// Atmospheric thing (should be base game ngl)
			Map.TurnOffAllLights(0.5f);
		}

		public void Nevercalmdown(CalmingDownEventArgs ev)
		{
			if(Server.Port == 7778)
				ev.IsAllowed = false;
		}

		public void Add096Target(AddingTargetEventArgs ev)
		{
			if (ev.Target.ReferenceHub.playerEffectsController.GetEffect<Invisible>().IsEnabled)
			{
				ev.IsAllowed = false;
			}
			else
			{
				if (ev.Target.RawUserId == "76561198059742329")
				{
					ev.Target.ShowCenterDownHint(CustomNotificationMessages.Tony096Responses.PickRandom(), 5);
				}
				else
					ev.Target.ShowCenterDownHint(CustomNotificationMessages.responses096AddTarget.PickRandom(), 5);
			}
		}

		public void On106Contain(ContainingEventArgs ev)
		{
			if (Containment106ObjectiveController.Allow106Containment)
			{
				ev.IsAllowed = true;
			}
			else
			{
				ev.IsAllowed = false;
				ev.ButtonPresser.ShowCenterDownHint("<color=yellow>You must first capture the objective to your left!</color>", 5);
			}
		}

		public void FemurBreaker(EnteringFemurBreakerEventArgs ev)
		{
			IEnumerable<Player> players106 = Player.List.Where(ply => ply.Role.Type == RoleType.Scp106);
			foreach (Player scp in players106)
			{
				scp.ShowCenterDownHint("<color=yellow>You sense death near your containment chambers...</color>", 10);
			}
		}

		public void DoorInteraction(InteractingDoorEventArgs ev)
		{
			// Ensuring that any SCP can open checkpoints even if they have items in their hands
			if(!ev.IsAllowed && ev.Door.Base.IsCheckpoint() && ev.Player.IsScp)
				ev.IsAllowed = true;
			
			// Ensuring that people can't open any doors in heavy containment on deathmatch server
			if (IsDeathMatchServer)
			{
				if(ev.Player.CurrentRoom.Zone == ZoneType.HeavyContainment)
					ev.IsAllowed = false;
				return;
			}

			// Tony's dummy thing lmao
			if (ev.Player.RawUserId == "76561198059742329" && ev.Player.IsHuman && !ev.Door.IsOpen & ev.IsAllowed & UtilityMethods.RandomChance(5))
			{
				UtilityMethods.SpawnTempDummy(ev.Player,
					ev.Player.Position + ev.Player.CameraTransform.forward*3,
					ev.Player.GameObject.transform.rotation*Quaternion.Euler(0,180f,0),
					Utils.SCPRoles.PickRandom(),
					1, 1, 1
				);
			}

			// 106's door condition
			if (ev.Door.Nametag != null && ev.Door.Nametag.Contains("106") && ev.Player.Role.Type != RoleType.Scp079)
			{
				if (ev.IsAllowed && Generator.List.Count(x=>x.IsEngaged) >= 2)
				{
					ev.IsAllowed = true;
				}
				else
				{
					ev.IsAllowed = false;
					ev.Player.ShowCenterDownHint("<color=yellow>2 079 Generators are required to open this door!</color>", 4);
				}
			}

			// 939 Door notifications
			string roomName = Map.FindParentRoom(ev.Door.Base.gameObject).Name;
			ZoneType roomZone = Map.FindParentRoom(ev.Door.Base.gameObject).Zone;

			if (ev.IsAllowed)
			{
				if (roomName.Contains("Checkpoint") || roomName.Contains("Chkp"))
				{
					if (roomZone == ZoneType.LightContainment)
					{
						if (ev.Door.Base.TryGetComponent(out DoorNametagExtension doorNameExt))
						{
							if (doorNameExt.GetName == "CHECKPOINT_LCZ_A")
							{
								InformSCPs(ZoneType.LightContainment, "You sense activity at Checkpoint A");
							}

							if (doorNameExt.GetName == "CHECKPOINT_LCZ_B")
							{
								InformSCPs(ZoneType.LightContainment, "You sense activity at Checkpoint B");
							}
						}
						//Find 939s in light and notify them

					}
					else
					{
						InformSCPs(ZoneType.HeavyContainment, "You sense activity at Checkpoint");
						InformSCPs(ZoneType.Entrance, "You sense activity at Checkpoint");
						//This must be in entrance/heavy so notify any 939s in either of the zones
					}
				}
				else if (roomName.Contains("914"))
				{
					// Notify 939s in light
					InformSCPs(ZoneType.LightContainment, "You sense activity at SCP-914");
				}
				else if (roomName.Contains("012"))
				{
					// Notify 939s in light
					InformSCPs(ZoneType.LightContainment, "You sense activity at SCP-012");
				}
				else if (roomName.Contains("Hid"))
				{
					//notify 939 in heavy
					if (ev.Door.Base.TryGetComponent(out DoorNametagExtension doorNameExt))
					{
						if (doorNameExt.GetName == "HID")
						{
							InformSCPs(ZoneType.HeavyContainment, "You sense activity at MicroHID");
						}
					}

				}
				else if (roomName.Contains("106"))
				{
					//notify 939 in heavy
					InformSCPs(ZoneType.HeavyContainment, "You sense activity at SCP106's Chamber");
				}
				else if (roomName.Contains("Room3ar"))
				{
					InformSCPs(ZoneType.HeavyContainment, "You sense activity at Armoury");
				}
				else if (roomName.Contains("GateA"))
				{
					InformSCPs(ZoneType.Entrance, "You sense activity at Gate A");
				}
				else if (roomName.Contains("GateB"))
				{
					InformSCPs(ZoneType.Entrance, "You sense activity at Gate B");
				}
			}

			// Zombie Door Breaking
			ev.Door.Base.TryGetComponent(out DoorNametagExtension doorNameExt1);
			if (ev.IsAllowed)
				return;
			
			if (ev.Player.Role.Type == RoleType.Scp0492 && (!ev.Door.Base.IsCheckpoint()) && !(ev.Door.Base.NetworkActiveLocks == 1))
			{
				if ((!false && ev.Door.Base.IsConsideredOpen()) || (true && doorNameExt1.GetName.Contains("106")))
				{
					return;
				}
				int amount = 0;
				DoorNametagExtension d = doorNameExt1;
				foreach (Player Ply in Player.List.Where(r => r.Role.Type == RoleType.Scp0492).ToList())
				{
					if (d.GetName == "INTERCOM")
					{
						if (Vector3.Distance(ev.Player.Position, Ply.Position) <= 4)
						{
							amount++;
						}
					}
					else
					{
						if (Vector3.Distance(ev.Door.Base.transform.position, Ply.Position) <= 4)
						{
							amount++;
						}
					}
				}
				if (amount >= 4)
				{
					ev.IsAllowed = true;
					var door = d.TargetDoor as IDamageableDoor;
					door.ServerDamage(1000000, DoorDamageType.ServerCommand);
				}
				else
				{
					ev.Player.ShowCenterDownHint("<color=red> You need at least %amount more zombies to open this door.</color>".Replace("%amount", $"{4 - amount}"), 4);
				}
			}
		}

		public void ElevatorInteraction(InteractingElevatorEventArgs ev)
		{
			if (IsDeathMatchServer)
			{
				ev.IsAllowed = false;
				return;
			}

			if (SillySundayEventHandler.NerfWarMode)
			{
				ev.IsAllowed = false;
				return;
			}

			if (ObjectivePointController.DisableElevators)
			{
				ev.IsAllowed = false;
				ev.Player.ShowCenterDownHint("<color=yellow>Elevators have been disabled due to decontamination</color>");
				return;
			}
				
			switch (ev.Lift.Type)
			{
				case ElevatorType.GateA:
					InformSCPs(ZoneType.Entrance, "You hear the elevator at Gate A");
					InformSCPs(ZoneType.Surface, "You hear the elevator at Gate A");
					break;

				case ElevatorType.GateB:
					InformSCPs(ZoneType.Entrance, "You hear the elevator at Gate B");
					InformSCPs(ZoneType.Surface, "You hear the elevator at Gate B");
					break;

				case ElevatorType.LczA:
					InformSCPs(ZoneType.LightContainment, "You hear the elevator at Elevator A");
					InformSCPs(ZoneType.HeavyContainment, "You hear the elevator at Elevator A");
					break;

				case ElevatorType.LczB:
					InformSCPs(ZoneType.LightContainment, "You hear the elevator at Elevator B");
					InformSCPs(ZoneType.HeavyContainment, "You hear the elevator at Elevator B");
					break;

				case ElevatorType.Nuke:
					InformSCPs(ZoneType.HeavyContainment, "You hear the elevator at Nuke Silo");
					break;

				case ElevatorType.Scp049:
					InformSCPs(ZoneType.HeavyContainment, "You hear the elevator at SCP-049's Chamber");
					break;
			}
		}

		public void InformSCPs(ZoneType Zone, string message)
		{
			IEnumerable<Player> SCP939s = Player.List.Where(ply => ply.Role.Type.Is939());

			foreach (Player scp in SCP939s)
			{
				if (scp.CurrentRoom.Zone == Zone)
				{
					scp.ShowCenterDownHint($"<color=yellow>{message}</color>", 2);
				}
			}
		}

		public void OnDropItem(DroppingItemEventArgs ev)
		{
			if (PlayersWithInfiniteDrop.Contains(ev.Player))
			{
				if(ev.Player.Items.Count < 8)
					ev.Player.AddItem(ev.Item.Type);
			}
		}

		public void OnDroppingAmmo(DroppingAmmoEventArgs ev)
		{
			Timing.RunCoroutine(UtilityMethods.DensifyAmmoBoxes(ev));
		}

		public void OnHotkey(ProcessingHotkeyEventArgs ev)
		{
			if (ev.Player.IsScp && ev.Hotkey != HotkeyButton.PrimaryFirearm)
				ev.IsAllowed = false;
			
			/*
			if (Server.Port != 7778) return;
			if (ev.Player.GetEffectActive<Amnesia>())
			{
				var items = ev.Player.Items.ToList();
				items.Remove(ev.Player.CurrentItem);
				for (int i = 0; i < items.Count; i++)
				{
					Item item = items[i];
					if (item.Type.IsArmor())
					{
						items.Remove(item);
						continue;
					}
				}
			}*/
		}

		public void OnSpawning(SpawningEventArgs ev)
		{
			// Flashlight for juless lmao
			if (ev.Player.RawUserId == "76561198434926562" && ev.RoleType != RoleType.Scp079)
			{
				Timing.CallDelayed(5f, () =>
				{
					ev.Player.AddItem(ItemType.Flashlight);
				});
			}
			
			// Tony's coin
			if (ev.Player.RawUserId == "76561198059742329" && ev.RoleType != RoleType.Scp079)
			{
				Timing.CallDelayed(5f, () =>
				{
					ev.Player.ShowCenterUpHint("Tony you've been given coin whether you can use it or not.", 4);
					ev.Player.AddItem(ItemType.Coin);
				});
			}

			if (WipeRadio)
			{
				Timing.CallDelayed(2f, () =>
				{
					ev.Player.RemoveItem(ev.Player.Items.FirstOrDefault(x => x.Type == ItemType.Radio));
					Log.Info("eff");
				});
			}

			if (IsDeathMatchServer)
				return;
			
			// TODO complete all this shit
			/*
			if (ev.Player.IsHatOwner(out HatOwner hatOwner))
			{
				if (hatOwner.Preference != 0)
				{
					int.TryParse($"{hatOwner.Preference}"[0].ToString(), out int hatcategoryInt);
					int.TryParse($"{hatOwner.Preference}"[1].ToString(), out int hatInt);

					UtilityMethods.CheckExistingSpawnedHatAndKill(ev.Player.UserId);
					bool sucessful;

					if (ev.Role.TypeType == RoleType.Tutorial || Server.Port == 7778)
						sucessful = true;
					else
						sucessful = Hat.DecreaseToken(ev.Player.RawUserId, (Hat.HatCategory)hatcategoryInt, hatInt);

					if (sucessful)
					{
						switch (hatInt)
						{
							case 0:
								if (hatcategoryInt == 1)
									HatFollow.Coroutines.Add(ev.Player.UserId, Timing.RunCoroutine(HatFollow.SCP018Follow(ev.Player)));
								else
									HatFollow.Coroutines.Add(ev.Player.UserId, Timing.RunCoroutine(HatFollow.SCP018Halo(ev.Player)));
								break;
							case 1:
								if (hatcategoryInt == 1)
									HatFollow.Coroutines.Add(ev.Player.UserId, Timing.RunCoroutine(HatFollow.SCP268Follow(ev.Player)));
								else
									HatFollow.Coroutines.Add(ev.Player.UserId, Timing.RunCoroutine(HatFollow.OrbittingSCP(ev.Player)));
								break;
							case 2:
								if (hatcategoryInt == 1)
									HatFollow.Coroutines.Add(ev.Player.UserId, Timing.RunCoroutine(HatFollow.ButterFollow(ev.Player)));
								else
									HatFollow.Coroutines.Add(ev.Player.UserId, Timing.RunCoroutine(HatFollow.SpinningButter(ev.Player)));
								break;
						}
						ev.Player.SendConsoleMessage("Spawned Hat", "yellow");
					}
					else
					{
						UtilityMethods.CheckExistingSpawnedHatAndKill(ev.Player.UserId);
						Hat.SetPrefToNull(ev.Player);
						ev.Player.Broadcast(7, "<i>Automatic hat equip failed! Check console for more info!</i>");
						ev.Player.SendConsoleMessage("Error! You likely don't have a token try to purchase more!", "yellow");
					}
				}
			}

			ItemType userItem = UtilityMethods.FindPreference(ev.Player);
			if (userItem != ItemType.None)
			{
				Donator.GetDonator(ev.Player, out Donator donator);
				if (userItem == ItemType.MicroHID)
				{
					if (!donator.IsBooster)
					{
						ev.Player.Broadcast(7, "<i>Automatic pet equip failed! You are not a Server Booster! Check console for more info!</i>");
						ev.Player.SendConsoleMessage("Your boost has probably expired. If you think this is an error contact Kognity", "yellow");
						UtilityMethods.UpdatePreference(ev.Player, "0");
						return;
					}
					else
					{
						UtilityMethods.CheckExistingPetAndKill(ev.Player.UserId);

						PetFollow.Coroutines.Add(ev.Player.UserId,
							Timing.RunCoroutine(PetFollow.FollowPlayer(ev.Player, (Item)Item.Create(userItem).Spawn(ev.Player.Position + Vector3.up * 2))));

						return;
					}
				}

				if(UtilityMethods.GetDonatorNum(UtilityMethods.FindPreferenceRaw(ev.Player)) < donator.DonatorNum)
				{
					UtilityMethods.CheckExistingPetAndKill(ev.Player.UserId);

					PetFollow.Coroutines.Add(ev.Player.UserId,
						Timing.RunCoroutine(PetFollow.FollowPlayer(ev.Player, (Item)Item.Create(userItem).Spawn(ev.Player.Position + Vector3.up * 2))));
					return;
				}
				else
				{
					ev.Player.Broadcast(7, "<i>Automatic pet equip failed! Check console for more info!</i>");
					ev.Player.SendConsoleMessage("Your patreon has probably expired or you have decreased your Tier. If you think this is an error contact Kognity", "yellow");
					UtilityMethods.UpdatePreference(ev.Player, "0");
					return;
				}
			}*/
		}

		public void OnPreAuthenticating(PreAuthenticatingEventArgs ev)
		{
			if (ev.ServerFull && !ev.IsAllowed)
			{
				Donator.GetDonator(ev.UserId.Substring(0, ev.UserId.IndexOf("@")), out Donator donator);
				if(donator != null)
					if (donator.IsBooster)
						ev.IsAllowed = true;
			}
		}

		public void OnCoinFlip(FlippingCoinEventArgs ev)
		{
			if (!Round.IsStarted)
			{
				if (ev.IsTails)
					ev.Player.Health += 5;
				else
					ev.Player.Health -= 5;
			}
		}

		public void OnHurting(HurtingEventArgs ev)
		{
			if (!ev.Target.IsVerified)
				return;
			
			if (ev.Target.IsGodModeEnabled)
				return;
			
			ItemType type = ItemType.None;
			byte TranslationID = 0;
			
			if (ev.Handler.Base is ExplosionDamageHandler)
				type = ItemType.GrenadeHE;
			else if (ev.Handler.Base is MicroHidDamageHandler)
				type = ItemType.MicroHID;
			else if (ev.Handler.Base is Scp018DamageHandler)
				type = ItemType.SCP018;
			else if (ev.Handler.Base is FirearmDamageHandler firearmDamageHandler)
				type = firearmDamageHandler.WeaponType;
			else if (ev.Handler.Base is UniversalDamageHandler universalDamageHandler)
				TranslationID = universalDamageHandler.TranslationId;
			else if (ev.Handler.Base is ScpDamageHandler scpDamageHandler)
				TranslationID = scpDamageHandler._translationId;

			// Zombie Infection
			if (ev.Attacker != null && ev.Attacker.Role.Type == RoleType.Scp0492 && ev.Attacker != ev.Target && TranslationID == DeathTranslations.Zombie.Id)
			{
				if (!InfectedPlayers.Contains(ev.Target) && ev.Target.Role.Type != RoleType.Tutorial)
				{
					UtilityMethods.InfectPlayer(ev.Target);
				}
			}
			
			// Cuffed protection
			if (ev.Target.IsCuffed)
			{
				if (ev.Attacker != null && ev.Attacker != ev.Target.Cuffer)
				{
					if (Vector3.Distance(ev.Target.Position, ev.Target.Cuffer.Position) <= 8)
					{
						if (type != ItemType.None)
						{
							ev.Attacker.ShowCenterDownHint("<color=yellow>You cannot injure detained players</color>",3);
							ev.Amount = 0;
							ev.IsAllowed = false;
							return;
						}
					}
				}
			}

			// 096 Trigger for custom rage delay
			if (ev.Target.Role.Type == RoleType.Scp096 && ev.Handler.Type.IsWeapon() &&
			    ev.Target.CurrentScp is PlayableScps.Scp096 scp && scp.IsPreWindup && ev.Attacker?.Role.Type != RoleType.Tutorial)
			{
				scp.PreWindup(0.1f);
				scp.Enrage();
			}

			// 106 Rework stuff
			if (ev.Target.Role.Type == RoleType.Scp106 || ev.Attacker?.Role.Type == RoleType.Scp106)
			{
				// If 106 is taking damage
				if (ev.Target.Role.Type == RoleType.Scp106)
				{
					SCP106CustomClass customClass = (SCP106CustomClass)ev.Target.CustomClassManager().CustomClass;
					Vanish ability = (Vanish)customClass.ActiveAbilities[0];

					if (TranslationID == DeathTranslations.Scp207.Id && ability.IsVanish)
					{
						ev.Amount = 0;
						ev.IsAllowed = false;
					}
					else
					{
						if (ability.IsVanish && ev.Target != ev.Attacker)
						{
							// If they are NOT in the list they can DO damage
							if (ev.Attacker?.Id != null &&
							    !ev.Target.TargetGhostsHashSet.Contains((int) ev.Attacker?.Id))
							{
								// Full damage vulnerability
								if (ev.Handler.Type.IsWeapon(false))
									ev.Amount *= 5;
							}
							else
							{
								if (ev.Attacker != null)
								{
									ev.IsAllowed = false;
									ev.Amount = 0;
								}
							}
						}
						else
						{
							// 106's bullet resistance rebalance to 0%
							if (ev.Target.Role.Type == RoleType.Scp106 && ev.Handler.Type.IsWeapon(false))
								ev.Amount *= 5f;

							if (!customClass.HasVanished)
							{
								customClass.Shield.SustainTime = 20;
							}
						}
					}
				}

				// If 106 is the attacker
				if (ev.Attacker?.Role.Type == RoleType.Scp106 && ev.Target != ev.Attacker)
				{
					SCP106CustomClass customClass = (SCP106CustomClass)ev.Attacker.CustomClassManager().CustomClass;
					Vanish ability = (Vanish)customClass.ActiveAbilities[0];

					if (ability.IsVanish)
					{
						// If the target is in the ghost list means that 106 CANNOT see him so DONT allow the damage
						if (ev.Attacker.TargetGhostsHashSet.Contains(ev.Target.Id))
						{
							ev.IsAllowed = false;
							ev.Amount = 0;
						}
					}
				}
			}

			// Damage tweaks for custom classes
			if (ev.Target.CustomClassManager().CustomClass?.Name == "NTF Scout" ||
			    ev.Target.CustomClassManager().CustomClass?.Name == "Chaos Scout" ||
			    ev.Target.CustomClassManager().CustomClass?.Name == "Chaos Hunter" ||
			    ev.Target.CustomClassManager().CustomClass?.Name == "NTF Hunter")
			{
				if (TranslationID == DeathTranslations.Scp207.Id)
				{
					ev.Amount = 0;
				}
			}
			
			// Containment Specialist
			if(ev.Attacker != null && (ev.Attacker.CustomClassManager().CustomClass?.Name == "NTF Containment Specialist"  ||
			   ev.Attacker.CustomClassManager().CustomClass?.Name == "Chaos Containment Specialist"))
			{
				if (type == ItemType.GunCOM18)
					ev.Amount *= 1.4f;
				
				else if (type == ItemType.GunRevolver)
				{
					if(ev.Attacker.Role.Team == Team.MTF && (ev.Target.Role.Team != Team.MTF && ev.Target.Role.Team != Team.RSC && ev.Target.Role.Team != Team.TUT && ev.Target.Role.Team != Team.RIP))
						ev.Target.EnableEffect(EffectType.Burned, 10, false);
					
					else if(ev.Attacker.Role.Team == Team.CHI && (ev.Target.Role.Team != Team.CHI && ev.Target.Role.Team != Team.CDP && ev.Target.Role.Team != Team.TUT && ev.Target.Role.Team != Team.RIP))
						ev.Target.EnableEffect(EffectType.Burned, 10, false);
				}
			}
			
			if(ev.Attacker != null && ev.Attacker.Role.Type == RoleType.NtfCaptain)
			{
				if (ev.Attacker.CustomClassManager().CustomClass?.Name == "NTF Captain")
				{
					if(((HotBullets)ev.Attacker.CustomClassManager().CustomClass.ActiveAbilities[0]).IsActive)
						ev.Target.EnableEffect(EffectType.Burned, 10, false);
				}
			}
			
			// Zombie overclocking shit
			if (ev.Target.CustomClassManager().CustomClass?.Name == "Zombie Overclocker" && TranslationID == DeathTranslations.Scp207.Id)
			{
				ev.Amount = 18;
			}
			
			// Making Sprinter immune to 207 Damage 
			if (ev.Target.CustomClassManager().CustomClass?.Name == "Zombie Sprinter")
			{
				if (TranslationID == DeathTranslations.Scp207.Id)
				{
					ev.Amount = 0;
				}
			}
			
			// Sprinter's Attack Damage 
			if (ev.Attacker != null && ev.Target.CustomClassManager().CustomClass?.Name == "Zombie Sprinter" && TranslationID == DeathTranslations.Zombie.Id)
			{
				ev.Amount = 20;
			}

			
			// Damage calculations
			int damageAmount = 0;
			if (type == ItemType.GrenadeHE)
			{
				if (ev.Target.Role.Type != RoleType.Tutorial && !ev.Target.IsGodModeEnabled)
					damageAmount = (int)Math.Ceiling(ev.Amount);
			}
			else if (TranslationID == DeathTranslations.Scp049.Id || TranslationID == DeathTranslations.Scp096.Id || TranslationID == DeathTranslations.Scp173.Id)
			{
				if (ev.Target.Role.Type != RoleType.Tutorial && !ev.Target.IsGodModeEnabled)
					damageAmount = (int)Math.Ceiling(ev.Target.Health);
			}
			else if (TranslationID == DeathTranslations.Warhead.Id || 
			         TranslationID == DeathTranslations.Decontamination.Id || 
			         TranslationID == DeathTranslations.Recontained.Id || 
			         TranslationID == DeathTranslations.PocketDecay.Id || 
			         TranslationID == DeathTranslations.UsedAs106Bait.Id || 
			         TranslationID == DeathTranslations.Unknown.Id || 
			         TranslationID == DeathTranslations.Falldown.Id || 
			         TranslationID == DeathTranslations.Crushed.Id || 
			         TranslationID == DeathTranslations.SeveredHands.Id || 
			         TranslationID == DeathTranslations.Warhead.Id || 
			         TranslationID == DeathTranslations.Warhead.Id 
			         )
			{
				damageAmount = 0;
			}
			else
			{
				damageAmount = (int)Math.Ceiling(ev.Amount);
			}

			if (damageAmount == -420)
			{
				IEnumerable<Player> player106 = Player.List.Where(ply => ply.Role.Type == RoleType.Scp106);
				foreach (Player ply in player106)
				{
					if (!ply.DoNotTrack)
					{
						if (damageDealt.ContainsKey(ply))
						{
							if (ev.Amount == 999990)
							{
								damageDealt[ply] += (int)Math.Ceiling(ev.Target.Health);
							}
							else
							{
								damageDealt[ply] += (int)Math.Ceiling(ev.Amount);
							}
						}
						else
						{
							if (ev.Amount == 999990)
							{
								damageDealt.Add(ply, (int)Math.Ceiling(ev.Target.Health));
							}
							else
							{
								damageDealt.Add(ply, (int)Math.Ceiling(ev.Amount));
							}
						}
					}
				}
			}
			else
			{
				if (ev.Attacker != null && ev.Target.RawUserId != ev.Attacker.RawUserId)
				{
					if (!ev.Attacker.DoNotTrack)
					{
						if (damageDealt.ContainsKey(ev.Attacker))
						{
							damageDealt[ev.Attacker] += damageAmount;
						}
						else
						{
							damageDealt.Add(ev.Attacker, damageAmount);
						}
					}
				}
			}
		}
		public void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev)
		{
			ev.IsAllowed = false;
		}

		public void OnDestroying(DestroyingEventArgs ev)
		{
			if (IsDeathMatchServer)
			{
				if (KingAndCompetitor.IsKing(ev.Player)) // If the player who left is a king
				{
					var index = KingAndCompetitor.GetIndex(ev.Player);
					var kc = KingAndCompetitor.GetAtIndex(index);

					if (kc.Competitor != null) // If there was actually a competitor
					{
						kc.King = kc.Competitor; // Set the competitor as the new king

						if (kc.Queue.Count >= 1) // Try to replace the competitor
						{
							kc.Competitor = kc.Queue[0];
							kc.Queue.Remove(kc.Queue[0]);

							Timing.CallDelayed(0.2f, () =>
							{
								Deathmatch.StartMatch(kc);
							});
							Log.Info("A1");							
						}
						else // No competitors were able to replace the King that left.
						{
							Timing.KillCoroutines(kc.StalemateChecker);
							Timing.KillCoroutines(kc.Unfreezer);
							kc.Competitor = null;
							Deathmatch.activeMatch[index] = false;
							kc.King.Role.Type = RoleType.Spectator;
							kc.King.Broadcast(5, "Your opponent has left and you have been put into spectator.");
							Log.Info("A2");
						}
					}
					else // Means there was no competitor, set everything to standby
					{
						kc.King = null;
						Deathmatch.activeMatch[index] = false;
					}
				}
				else if (KingAndCompetitor.IsCompetitor(ev.Player))  // If the player who left is a competitor
				{
					var index = KingAndCompetitor.GetIndex(ev.Player);
					var kc = KingAndCompetitor.GetAtIndex(index);

					if (kc.King != null) // If there was actually a king
					{
						if (kc.Queue.Count >= 1) // Try to replace the competitor
						{
							kc.Competitor = kc.Queue[0];
							kc.Queue.Remove(kc.Queue[0]);

							Timing.CallDelayed(0.2f, () =>
							{
								Deathmatch.StartMatch(kc);
							});
							Log.Info("A3");
						}
						else // No competitors were able to replace the one that left.
						{
							Timing.KillCoroutines(kc.StalemateChecker);
							Timing.KillCoroutines(kc.Unfreezer);
							kc.Competitor = null;
							Deathmatch.activeMatch[index] = false;
							kc.King.Role.Type = RoleType.Spectator;
							kc.King.Broadcast(5, "Your opponent has left and you have been put into spectator.");
							Log.Info("A4");							
						}
					}
					else // Means there was no King, set everything to standby
					{
						kc.Competitor = null;
						Deathmatch.activeMatch[index] = false;
						Log.Info("A5");
					}
				}
				else
				{
					if (KingAndCompetitor.IsInQueue(ev.Player)) // This guy was just a spectator
						KingAndCompetitor.RemoveFromQueue(ev.Player);
				}
			}
			else
			{
				// If it is the dev server or if the round is ended or hasn't started yet, do nothing
				if (IsDevServer || !RoundActive) return;
				
				if(Player.List.Count() - 1 <= 0)
					Round.Restart();
			}
		}

		public void OnDecontaminating(DecontaminatingEventArgs ev)
		{
			if (IsDeathMatchServer)
				ev.IsAllowed = false;

			// Cleans all the items
			foreach (ItemPickupBase item in UnityEngine.Object.FindObjectsOfType<ItemPickupBase>())
			{
				float y = item.NetworkInfo.Position.y;
				if (y < 200f && y > -200f)
					item.DestroySelf();
			}

			// Cleans all the ragdolls
			foreach (Ragdoll doll in UnityEngine.Object.FindObjectsOfType<Ragdoll>())
			{
				float y = doll.transform.position.y;
				if (y < 200f && y > -200f)
					NetworkServer.Destroy(doll.gameObject);
			}
		}

		public void OnActivating914(ActivatingEventArgs ev)
		{
			if (IsDeathMatchServer)
				ev.IsAllowed = false;
		}

		public void OnWarheadDetonated()
		{
			Timing.CallDelayed(1f, () =>
			{
				// Cleans all the items
				foreach (ItemPickupBase item in UnityEngine.Object.FindObjectsOfType<ItemPickupBase>())
				{
					float y = item.NetworkInfo.Position.y;
					if (y < 500f)
						item.DestroySelf();
				}

				// Cleans all the ragdolls
				foreach (Ragdoll doll in UnityEngine.Object.FindObjectsOfType<Ragdoll>())
				{
					float y = doll.transform.position.y;
					if (y < 500f)
						NetworkServer.Destroy(doll.gameObject);
				}
			});
		}

		public void PlayerJoined(JoinedEventArgs ev)
		{
			
		}

		public void VerifiedPlayer(VerifiedEventArgs ev)
		{
			foreach (string phrase in Utils.BlacklistedURLs)
			{
				string name = ev.Player.Nickname.ToLower();
				if (name.Contains(phrase) && !Utils.WhitelistedIDs.Contains(ev.Player.RawUserId))
				{
					ServerConsole.Disconnect(ev.Player.GameObject, $"<color=\"cyan\">Detected advertising ({phrase}) in your username. Please change it before joining the server.\nIf you believe this was a mistake, contact a staff member in our discord: https://discord.gg/kognity</color>");
					return;
				}
			}
			
			if(!ev.Player.GameObject.TryGetComponent<PlayhousePluginComponent>(out _))
				ev.Player.GameObject.AddComponent<PlayhousePluginComponent>();

			Task.Run(async () =>
			{
				var ctx = new AccountContext();
				await ctx.Get(ev.Player.UserId);
				using (var player = await ctx.Get(ev.Player.UserId))
				{

					player.GetComponent<EconomyComponent>().Deposit(1);
				}
			});

			UtilityMethods.FindAndSetCustomBadge(ev.Player);
			if (!JoinedPlayers.Contains(ev.Player.RawUserId))
			{
				JoinedPlayers.Add(ev.Player.RawUserId);
				if (!ev.Player.DoNotTrack)
				{
					joins.Add(ev.Player);
				}
			}
			if (!Round.IsStarted && (GameCore.RoundStart.singleton.NetworkTimer > 1 || GameCore.RoundStart.singleton.NetworkTimer == -2))
			{
				Timing.CallDelayed(0.5f, () =>
				{
					if (Round.IsStarted || (GameCore.RoundStart.singleton.NetworkTimer <= 1 &&
					                        GameCore.RoundStart.singleton.NetworkTimer != -2)) return;
					ev.Player.IsOverwatchEnabled = false;
					ev.Player.Role.Type = RoleType.Tutorial;
					Scp096.TurnedPlayers.Add(ev.Player);
				});

				Timing.CallDelayed(1.5f, () =>
				{
					if (Round.IsStarted || (GameCore.RoundStart.singleton.NetworkTimer <= 1 &&
					                        GameCore.RoundStart.singleton.NetworkTimer != -2)) return;
					ev.Player.Position = SpawnPoint + Vector3.up;
					ev.Player.AddItem(ItemType.Coin);
				});
				
				Timing.CallDelayed(1.5f, () =>
				{
					if (Round.IsStarted || (GameCore.RoundStart.singleton.NetworkTimer <= 1 &&
					                        GameCore.RoundStart.singleton.NetworkTimer != -2)) return;
					ev.Player.Inventory.ServerSelectItem(ev.Player.Inventory.UserInventory.Items.FirstOrDefault(x => x.Value.ItemTypeId == ItemType.Coin).Value.ItemSerial);
				});
				
				Timing.CallDelayed(1.5f, () =>
				{
					if (Round.IsStarted || (GameCore.RoundStart.singleton.NetworkTimer <= 1 &&
					                        GameCore.RoundStart.singleton.NetworkTimer != -2)) return;
					ev.Player.Inventory.ServerSelectItem(ev.Player.Inventory.UserInventory.Items.FirstOrDefault(x => x.Value.ItemTypeId == ItemType.Coin).Value.ItemSerial);
				});
			}

			if (IsDeathMatchServer) return;

			if (Round.IsStarted)
			{
				if (SillySundayEventHandler.NerfWarMode)
					return;
				if (SillySundayEventHandler.ohfiverescuemode)
				{
					/*
					Timing.CallDelayed(8, () =>
					{
						ev.Player.ClearBroadcasts();
						ev.Player.Role.Type = RoleType.ChaosConscript;
						ev.Player.Position = RoleExtensions.GetRandomSpawnProperties(ev.Player.Role.Type);
						ev.Player.ClearInventory();
						ev.Player.AddItem(ItemType.GunLogicer);
						ev.Player.AddItem(ItemType.Medkit);
						ev.Player.AddItem(ItemType.Medkit);
						ev.Player.AddItem(ItemType.KeycardChaosInsurgency);
						ev.Player.Ammo[ItemType.Ammo762x39] = 150;
						ev.Player.Broadcast(10, "<color=yellow><i>Welcome to 05 Rescue</i>\n<b>Your objective is to kill all Foundation members</b></color>");
					});*/
				}
				else if (SillySundayEventHandler.slaughterhouse)
				{
					/*
					Timing.CallDelayed(8, () =>
					{
						ev.Player.ClearBroadcasts();
						ev.Player.Role.Type = RoleType.ChaosConscript;
						Timing.CallDelayed(0.2f, () =>
						{
							ev.Player.Position = RoleExtensions.GetRandomSpawnProperties(RoleType.Scp173);
						});
						ev.Player.ClearInventory();
						ev.Player.AddItem(ItemType.GunLogicer);
						ev.Player.AddItem(ItemType.Medkit);
						ev.Player.AddItem(ItemType.Medkit);
						ev.Player.AddItem(ItemType.KeycardChaosInsurgency);
						ev.Player.Ammo[ItemType.Ammo762x39] = 450;
						ev.Player.Broadcast(10, "<color=yellow><i>Welcome to Slaughter House</i>\n<b>Your objective is to try and kill the Scientists.</b></color>");
					});*/
				}
				else if (SillySundayEventHandler.sugarrush)
				{
					Timing.RunCoroutine(SpawnPlayer(ev.Player));
					Timing.CallDelayed(10, () =>
					{
						ev.Player.EnableEffect<MovementBoost>(1800);
						ev.Player.ChangeEffectIntensity<MovementBoost>(30);
					});
				}
				else if (IsEventServer)
				{
					if (!DoNotSpawn.Contains(ev.Player.UserId))
					{
						Timing.RunCoroutine(SpawnPlayer(ev.Player));
					}
				}
				else
				{
					if (!DoNotSpawn.Contains(ev.Player.UserId))
					{
						Timing.RunCoroutine(SpawnPlayer(ev.Player));
					}
				}
			}
			if (SillySunday)
			{
				switch (random.Next(5))
				{
					case 0:
						ev.Player.Broadcast(5, $"<size=50><color=aqua>Welcome <b><color=#ff96de>{ev.Player.Nickname}</color></b> to\nKognity's Playhouse and to Site-69\nEnjoy the events!</color></size>");
						break;
					case 1:
						ev.Player.Broadcast(5, $"<size=50><color=#ff96de>Welcome <b><color=aqua>{ev.Player.Nickname}</color></b> to\nKognity's Playhouse and to Site-69\nEnjoy the events!</color></size>");
						break;
					case 2:
						ev.Player.Broadcast(5, $"<size=50><color=#ff96de>Welcome <b><color=#ff96de>{ev.Player.Nickname}</color></b> to\nKognity's Playhouse and to Site-69\nEnjoy the events!</color></size>");
						break;
					case 3:
						ev.Player.Broadcast(5, $"<size=50><color=aqua>Welcome <b><color=aqua>{ev.Player.Nickname}</color></b> to\nKognity's Playhouse and to Site-69\nEnjoy the events!</color></size>");
						break;
					case 4:
						ev.Player.Broadcast(5, $"<size=50><color=orange>Welcome <b><color=orange>{ev.Player.Nickname}</color></b> to\nKognity's Playhouse and to Site-69\nEnjoy the events!</color></size>");
						break;
				}
			}
			else
			{
				switch (random.Next(5))
				{
					case 0:
						ev.Player.Broadcast(5, $"<size=70><color=aqua>Welcome <b><color=#ff96de>{ev.Player.Nickname}</color></b> to\nKognity's Playhouse and to Site-79</color></size>");
						break;
					case 1:
						ev.Player.Broadcast(5, $"<size=70><color=#ff96de>Welcome <b><color=aqua>{ev.Player.Nickname}</color></b> to\nKognity's Playhouse and to Site-79</color></size>");
						break;
					case 2:
						ev.Player.Broadcast(5, $"<size=70><color=#ff96de>Welcome <b><color=#ff96de>{ev.Player.Nickname}</color></b> to\nKognity's Playhouse and to Site-79</color></size>");
						break;
					case 3:
						ev.Player.Broadcast(5, $"<size=70><color=aqua>Welcome <b><color=aqua>{ev.Player.Nickname}</color></b> to\nKognity's Playhouse and to Site-79</color></size>");
						break;
					case 4:
						ev.Player.Broadcast(5, $"<size=70><color=orange>Welcome <b><color=orange>{ev.Player.Nickname}</color></b> to\nKognity's Playhouse and to Site-79</color></size>");
						break;
				}
			}

			Timing.CallDelayed(15, () =>
			{
				ev.Player.Broadcast(4, "<size=48><color=aqua>Just as a reminder please <color=#ff96de><b>read the rules</b></color> before playing!</color></size>");
				ev.Player.Broadcast(7, "<size=38><color=aqua>Rules can be checked by pressing <color=#ff96de>Escape</color> then clicking on <color=#ff96de>Server Info</color></color></size>");
				ev.Player.Broadcast(3, "<b><size=130><color=#ff96de>Thanks for playing!</color></size></b>");

				/// Canada Day
				//ev.Player.Broadcast(3, "<b><size=70><color=aqua>Thanks for playing!</color>\n<color=#ff96de>Happy Canada Day!</color></size></b>");

				/// Pride Month
				//ev.Player.Broadcast(3, "<ize=70><color=#ff96de>Thanks for playing!</color>\n<color=#ff4000>H</color><color=#ff8000>a</color><color=#ffbf00>p</color><color=#ffff00>p</color><color=#aaff00>y</color><color=#55ff00> </color><color=#00ff00>P</color><color=#00ff40>r</color><color=#00ff80>i</color><color=#00ffbf>d</color><color=#00ffff>e</color><color=#00aaff> </color><color=#0055ff>M</color><color=#0000ff>o</color><color=#4000ff>n</color><color=#8000ff>t</color><color=#bf00ff>h</color><color=#ff00ff>!</color></size>");
			});
		}

		/*
		IEnumerator<float> RepeatSong()
		{
			while (true)
			{
				yield return Timing.WaitForSeconds(160f);
				
				try
				{
					stream = File.OpenRead("/home/ubuntu/tf2Audio/portalradio.mp3.raw");
					VoiceChatManager.Api.Extensions.StreamExtensions.TryPlay(stream, Room.List.Where(x => x.Name.Contains("372")).FirstOrDefault().Position, 100, "Proximity", out VoiceChatManager.Api.Audio.Playback.IStreamedMicrophone streamedMicrophone);
				}
				catch (Exception e)
				{
					Log.Info(e);
				}
			}
		}*/

		public void OnEnterPocketDimension(EnteringPocketDimensionEventArgs ev)
		{
			/*
			if (ev.Player.RawUserId == "76561198059742329")
			{
				ev.Player.ShowCenterDownHint(CustomNotificationMessages.Tony106Responses.PickRandom(), 5);
			}else
				ev.Player.ShowCenterDownHint(CustomNotificationMessages.responses106PD.PickRandom(), 5);
				
			*/
		}

		public void OnItemPickup(PickingUpItemEventArgs ev)
		{
			if(ev.Pickup == Radio)
			{
				ev.Player.ShowCenterDownHint($"<color=yellow>You cannot pickup Music Radios!</color>", 4);
				ev.IsAllowed = false;
				ev.Pickup.Locked = false;
				ev.Pickup.InUse = false;
				return;
			}
			if (PetFollow.Pets.Contains(ev.Pickup))
			{
				ev.Player.ShowCenterDownHint($"<color=yellow>You cannot pickup Pets!</color>", 4);
				ev.IsAllowed = false;
				ev.Pickup.Locked = false;
				ev.Pickup.InUse = false;
				return;
			}
			if (Letters.Contains(ev.Pickup))
			{
				ev.Player.ShowCenterDownHint($"<color=yellow> You cannot pickup letters!</color>", 4);
				ev.IsAllowed = false;
				ev.Pickup.Locked = false;
				ev.Pickup.InUse = false;
				return;
			}
			if (ArtItems.Contains(ev.Pickup))
			{
				ev.Player.ShowCenterDownHint($"<color=yellow> You cannot pickup dev tools!</color>", 4);
				ev.IsAllowed = false;
				ev.Pickup.Locked = false;
				ev.Pickup.InUse = false;
				return;
			}
			KeyValuePair<Player, Pickup[]> playerAndSticky = StickyPositions.Where(x => x.Value.Contains(ev.Pickup)).FirstOrDefault();
			if (playerAndSticky.Value == null || playerAndSticky.Key == null) return;
			ev.Player.ShowCenterDownHint($"<color=yellow> You cannot pickup sticky bombs!</color>", 4);
			ev.IsAllowed = false;
			ev.Pickup.Locked = false;
			ev.Pickup.InUse = false;
		}
		public IEnumerator<float> ServerRestarter()
		{
			while (true)
			{
				//SendMessageAsync("<@216304765494099969> Deathmatch server online!", false);
				yield return Timing.WaitForSeconds(43200);
				//SendMessageAsync("<@216304765494099969> Restarting deathmatch server!", false);
				Server.Restart();
			}
		}

		public void OnStartingWarhead(StartingEventArgs ev)
		{
			if (SillySunday)
			{
				foreach(var room in Room.List)
				{
					if(!room.gameObject.TryGetComponent<RainbowLightController>(out var v))
					{
						room.gameObject.AddComponent<RainbowLightController>();
					}
				}
			}
		}

		public void OnStoppingWarhead(StoppingEventArgs ev)
		{
			if (SillySunday)
			{
				foreach (var room in Room.List)
				{
					if(room.Type != RoomType.LczPlants && room.Type != RoomType.LczGlassBox && room.Type != RoomType.Lcz173 && !room.Name.Contains("Chkp") && !room.Name.Contains("Checkpoint"))
					{
						UnityEngine.Object.Destroy(room.gameObject.GetComponent<RainbowLightController>());
						room.ResetColor();
					}
				}
			}
		}

		public void OnRoundStart()
		{
			if (RoundActive) return;

			RoundActive = true;

			RewardPlayers = Server.PlayerCount <= 17;
			
			if (IsDeathMatchServer)
			{
				coroutines.Add(Timing.RunCoroutine(DeathmatchPlayerStats()));
				Timing.RunCoroutine(ServerRestarter());
			}
			else if (IsEventServer)
			{
				CustomItemSpawning.SpawnItems();
				coroutines.Add(Timing.RunCoroutine(CheckEscape()));
				coroutines.Add(Timing.RunCoroutine(BreakoutBlitz.ItemAndRagdollClear()));
				Stopwatch = Stopwatch.StartNew();
			}
			else
			{
				if(!IsDevServer)
					coroutines.Add(Timing.RunCoroutine(CommonUtilsStuff.AutoNuke()));
				
				CustomItemSpawning.SpawnItems();
				coroutines.Add(Timing.RunCoroutine(CheckEscape()));
			}

			foreach (var room in Room.List)
			{
				room.ResetColor();
				if (room.Type == RoomType.Surface)
				{
					room.LightIntensity = 1.5f;
				}
				else
				{
					if(room.Zone == ZoneType.LightContainment || room.Zone == ZoneType.HeavyContainment)
						room.LightIntensity = 1.25f;
				}
			}
			
			if (SillySunday)
			{
				foreach(var room in Room.List.Where(x => x.Type == RoomType.LczGlassBox || x.Type == RoomType.LczPlants || x.Type == RoomType.Lcz173 || x.Name.Contains("Chkp") || x.Name.Contains("Checkpoint")))
				{
					room.gameObject.AddComponent<RainbowLightController>();
				}
			}

			foreach (Player Ply in Player.List)
			{
				if (!Ply.IsRainbowTagUser())
					continue;

				UtilityMethods.AddRainbowController(Ply);
			}

			if (RewardPlayers)
			{
				foreach (Player Ply in Player.List)
				{
					Ply.ShowCenterDownHint("<b>There are less than 18 players! Redeem your <color=red>reward</color> via \n<color=red>'klp redeem rounds'</color> in #bot-commands!</b>", 12);
				}
			}				

			if(lobby != null)
				lobby.Destroy();

			List<Player> BulkList = Player.List.ToList();
			List<Player> SCPPlayers = new List<Player> { };
			List<Player> ScientistPlayers = new List<Player> { };
			List<Player> GuardPlayers = new List<Player> { };
			List<Player> ClassDPlayers = new List<Player> { };

			List<Player> PlayersToSpawnAsSCP = new List<Player> { };
			List<Player> PlayersToSpawnAsScientist = new List<Player> { };
			List<Player> PlayersToSpawnAsGuard = new List<Player> { };
			List<Player> PlayersToSpawnAsClassD = new List<Player> { };

			int SCPsToSpawn = 0;
			int ClassDsToSpawn = 0;
			int ScientistsToSpawn = 0;
			int GuardsToSpawn = 0;

			List<char> SpawnSequence = new List<char>
				{ '4', '0', '1', '4', '3', '1', '4', '0', '3', '1', '4', '4', '1', '4', '0', '4', '1', '3', '4', '0', '3', '1', '4', '4', '1', '4', '0', '4', '1', '3', '0', '4', '4', '1', '0', '1', '4', '3', '3', '1' };

			for (int x = 0; x < Player.List.ToList().Count; x++)
			{
				switch (SpawnSequence[x])
				{
					case '4':
						ClassDsToSpawn += 1;
						break;
					case '3':
						ScientistsToSpawn += 1;
						break;
					case '1':
						GuardsToSpawn += 1;
						break;
					case '0':
						SCPsToSpawn += 1;
						break;
				}
			}

			foreach (var player in Player.List)
			{

				if (Vector3.Distance(player.Position, SpawnPoint + new Vector3(-7.935f, 0, -13.74f)) <= 3)
				{
					SCPPlayers.Add(player);
					Log.Info($"SCP1: {player}");
				}
				else if (Vector3.Distance(player.Position, SpawnPoint + new Vector3(13.74382f, 0, -7.935f)) <= 3)
				{
					ClassDPlayers.Add(player);
					Log.Info($"ClassD1: {player}");
				}
				else if (Vector3.Distance(player.Position, SpawnPoint + new Vector3(-13.74382f, 0, -7.935f)) <= 3)
				{
					ScientistPlayers.Add(player);
					Log.Info($"Scientist1: {player}");
				}
				else if (Vector3.Distance(player.Position, SpawnPoint + new Vector3(7.934999f, 0, -13.74382f)) <= 3)
				{
					GuardPlayers.Add(player);
					Log.Info($"Guard1: {player}");
				}
				player.Role.Type = RoleType.None;
			}

			// ---------------------------------------------------------------------------------------\\
			// ClassD
			if (ClassDsToSpawn != 0)
			{
				if (ClassDPlayers.Count <= ClassDsToSpawn) // Less people (or equal) voted than what is required in the game.
				{
					foreach (Player ply in ClassDPlayers)
					{
						PlayersToSpawnAsClassD.Add(ply);
						ClassDsToSpawn -= 1;
						BulkList.Remove(ply);
					}
				}
				else // More people voted than what is required, time to play the game of chance.
				{
					for (int x = 0; x < ClassDsToSpawn; x++)
					{
						Player Ply = ClassDPlayers[random.Next(ClassDPlayers.Count)];
						PlayersToSpawnAsClassD.Add(Ply);
						ClassDPlayers.Remove(Ply); // Removing winner from the list
						BulkList.Remove(Ply); // Removing the winners from the bulk list
					}
					ClassDsToSpawn = 0;
				}
			}

			// ---------------------------------------------------------------------------------------\\
			// Scientists
			if (ScientistsToSpawn != 0)
			{
				if (ScientistPlayers.Count <= ScientistsToSpawn) // Less people (or equal) voted than what is required in the game.
				{
					foreach (Player ply in ScientistPlayers)
					{
						PlayersToSpawnAsScientist.Add(ply);
						ScientistsToSpawn -= 1;
						BulkList.Remove(ply);
					}
				}
				else // More people voted than what is required, time to play the game of chance.
				{
					for (int x = 0; x < ScientistsToSpawn; x++)
					{
						Player Ply = ScientistPlayers[random.Next(ScientistPlayers.Count)];
						PlayersToSpawnAsScientist.Add(Ply);
						ScientistPlayers.Remove(Ply); // Removing winner from the list
						BulkList.Remove(Ply); // Removing the winners from the bulk list
					}
					ScientistsToSpawn = 0;
				}
			}

			// ---------------------------------------------------------------------------------------\\
			// Guards
			if (GuardsToSpawn != 0)
			{
				if (GuardPlayers.Count <= GuardsToSpawn) // Less people (or equal) voted than what is required in the game.
				{
					foreach (Player ply in GuardPlayers)
					{
						PlayersToSpawnAsGuard.Add(ply);
						GuardsToSpawn -= 1;
						BulkList.Remove(ply);
					}
				}
				else // More people voted than what is required, time to play the game of chance.
				{
					for (int x = 0; x < GuardsToSpawn; x++)
					{
						Player Ply = GuardPlayers[random.Next(GuardPlayers.Count)];
						PlayersToSpawnAsGuard.Add(Ply);
						GuardPlayers.Remove(Ply); // Removing winner from the list
						BulkList.Remove(Ply); // Removing the winners from the bulk list
					}
					GuardsToSpawn = 0;
				}
			}

			// ---------------------------------------------------------------------------------------\\
			// SCPs
			if (SCPsToSpawn != 0)
			{
				if (SCPPlayers.Count <= SCPsToSpawn) // Less people (or equal) voted than what is required in the game.
				{
					foreach (Player ply in SCPPlayers)
					{
						PlayersToSpawnAsSCP.Add(ply);
						SCPsToSpawn -= 1;
						BulkList.Remove(ply);
					}
				}
				else // More people voted than what is required, time to play the game of chance.
				{
					for (int x = 0; x < SCPsToSpawn; x++)
					{
						Player Ply = SCPPlayers[random.Next(SCPPlayers.Count)];
						SCPPlayers.Remove(Ply);
						PlayersToSpawnAsSCP.Add(Ply); // Removing winner from the list
						BulkList.Remove(Ply); // Removing the winners from the bulk list
					}
					SCPsToSpawn = 0;
				}
			}
			// ---------------------------------------------------------------------------------------\\
			// ---------------------------------------------------------------------------------------\\
			// ---------------------------------------------------------------------------------------\\
			// ---------------------------------------------------------------------------------------\\

			// At this point we need to check for any blanks and fill them in via the bulk list guys
			if (ClassDsToSpawn != 0)
			{
				for (int x = 0; x < ClassDsToSpawn; x++)
				{
					Player Ply = BulkList[random.Next(BulkList.Count)];
					PlayersToSpawnAsClassD.Add(Ply);
					BulkList.Remove(Ply); // Removing the winners from the bulk list
				}
			}
			if (SCPsToSpawn != 0)
			{
				for (int x = 0; x < SCPsToSpawn; x++)
				{
					Player Ply = BulkList[random.Next(BulkList.Count)];
					PlayersToSpawnAsSCP.Add(Ply);
					BulkList.Remove(Ply); // Removing the winners from the bulk list
				}
			}
			if (ScientistsToSpawn != 0)
			{
				for (int x = 0; x < ScientistsToSpawn; x++)
				{
					Player Ply = BulkList[random.Next(BulkList.Count)];
					PlayersToSpawnAsScientist.Add(Ply);
					BulkList.Remove(Ply); // Removing the winners from the bulk list
				}
			}
			if (GuardsToSpawn != 0)
			{
				for (int x = 0; x < GuardsToSpawn; x++)
				{
					Player Ply = BulkList[random.Next(BulkList.Count)];
					PlayersToSpawnAsGuard.Add(Ply);
					BulkList.Remove(Ply); // Removing the winners from the bulk list
				}
			}

			// ---------------------------------------------------------------------------------------\\

			// Okay we have the list! Time to spawn everyone in, we'll leave SCP for last as it has a bit of logic.
			foreach (Player ply in PlayersToSpawnAsClassD)
			{
				Timing.CallDelayed(0.1f, () =>
				{
					ply.Role.Type = RoleType.ClassD;
				});
			}
			foreach (Player ply in PlayersToSpawnAsScientist)
			{
				Timing.CallDelayed(0.1f, () =>
				{
					ply.Role.Type = RoleType.Scientist;
				});
			}
			foreach (Player ply in PlayersToSpawnAsGuard)
			{
				Timing.CallDelayed(0.1f, () =>
				{
					ply.Role.Type = RoleType.FacilityGuard;
				});
			}

			// ---------------------------------------------------------------------------------------\\

			// SCP Logic, preventing SCP-079 from spawning if there isn't at least 2 other SCPs
			List<RoleType> Roles = new List<RoleType>
				{ RoleType.Scp049, RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989 };

			if (PlayersToSpawnAsSCP.Count > 2)
				Roles.Add(RoleType.Scp079);

			foreach (Player ply in PlayersToSpawnAsSCP)
			{
				RoleType role = Roles[random.Next(Roles.Count)];
				Roles.Remove(role);

				Timing.CallDelayed(0.1f, () =>
				{
					ply.Role.Type = role;
					ply.Broadcast(10, "<color=yellow><b> Did you know you can swap classes with other SCP's?</b></color> Simply type <color=orange>.scpswap (role number)</color> in your in-game console (not RA) to swap!)");
				});
			}

			if (Server.Port != 7778 && !IsDeathMatchServer && !IsEventServer)
			{
				Timing.CallDelayed(10f, () =>
				{
					if(!SillySundayEventHandler.NerfWarMode)
						Round.IsLocked = false;
				});
			}

			foreach (var thing in PlayersToSpawnAsGuard)
			{
				Log.Info($"Guard: {thing}");
			}
			foreach (var thing in PlayersToSpawnAsScientist)
			{
				Log.Info($"Scientist: {thing}");
			}
			foreach (var thing in PlayersToSpawnAsClassD)
			{
				Log.Info($"ClassD: {thing}");
			}
			foreach (var thing in PlayersToSpawnAsSCP)
			{
				Log.Info($"SCP: {thing}");
			}

			/*
			var test = new RoundSummary.SumInfo_ClassList
			{
				class_ds = ClassDsToSpawn,
				scientists = ScientistsToSpawn,
				scps_except_zombies = SCPsToSpawn,
				mtf_and_guards = GuardsToSpawn
			};
			RoundSummary.singleton.SetStartClassList(test);*/

			Timing.CallDelayed(3, () =>
			{
				Scp096.TurnedPlayers.Clear();
				Scp173.TurnedPlayers.Clear();
			});

			Timing.RunCoroutine(PromotePlayers());
			if (IsDeathMatchServer)
				coroutines.Add(Timing.RunCoroutine(Deathmatch.TimerDeathMatch()));
			else
			{
				if(!IsEventServer)
					coroutines.Add(Timing.RunCoroutine(Timer()));	
			}

			allowSwaps = true;
			Timing.CallDelayed(120, () => allowSwaps = false);
		}

		public void OnBan(BanningEventArgs ev)
		{
			if (Server.Port == 7777)
			{
				SendMessageAsync($"Banned Player on **Server #1**:\n```Offender: {ev.Target.Nickname} ({ev.Target.UserId})\nReason: {ev.Reason}\nDuration: {ev.Duration} seconds\nIssuer: {ev.Issuer.Nickname} ({ev.Issuer.UserId})```", true);
			}
			else if (Server.Port == 8888)
			{
				SendMessageAsync($"Banned Player on **Server #2**:\n```Offender: {ev.Target.Nickname} ({ev.Target.UserId})\nReason: {ev.Reason}\nDuration: {ev.Duration} seconds\nIssuer: {ev.Issuer.Nickname} ({ev.Issuer.UserId})```", true);
			}
			else if (Server.Port == 9999)
			{
				SendMessageAsync($"Banned Player on **Server #3**:\n```Offender: {ev.Target.Nickname} ({ev.Target.UserId})\nReason: {ev.Reason}\nDuration: {ev.Duration} seconds\nIssuer: {ev.Issuer.Nickname} ({ev.Issuer.UserId})```", true);
			}
			else if (Server.Port == 7778)
			{
				SendMessageAsync($"Banned Player on **Dev Server**:\n```Offender: {ev.Target.Nickname} ({ev.Target.UserId})\nReason: {ev.Reason}\nDuration: {ev.Duration} seconds\nIssuer: {ev.Issuer.Nickname} ({ev.Issuer.UserId})```", true);
			}
			else
			{
				SendMessageAsync($"Banned Player on **Server with port: {Server.Port}**:\n```Offender: {ev.Target.Nickname} ({ev.Target.UserId})\nReason: {ev.Reason}\nDuration: {ev.Duration} seconds\nIssuer: {ev.Issuer.Nickname} ({ev.Issuer.UserId})```", true);
			}

		}

		public void OnEscape(EscapingEventArgs ev)
		{
			if (!ev.Player.DoNotTrack)
			{
				if (escapes.ContainsKey(ev.Player))
				{
					escapes[ev.Player] += 1;
				}
				else
				{
					escapes.Add(ev.Player, 1);
				}
			}

			if (SillySundayEventHandler.ohfiverescuemode)
			{
				Map.Broadcast(10, "MTF Wins! The 05 has escaped!");

				Round.EndRound(true);
			}

			if (IsEventServer)
			{
				switch (ev.NewRole)
				{
					case RoleType.ChaosConscript:
						if (BreakoutBlitz.ClassDEscapes + 1 >= BreakoutBlitz.RequiredClassDEscapes)
						{
							Map.ClearBroadcasts();
							Cassie.Message("pitch_0.3 .g4 .g4 .g4 .g4");
							Map.Broadcast(10, "<size=70><b><color=green>Chaos Won!\n</color></b></size><i>Restarting Round...</i>");
							Timing.CallDelayed(10, () => Round.Restart());
						}
						else
						{
							BreakoutBlitz.ClassDEscapes++;
						}
						break;
					
					case RoleType.NtfPrivate:
					case RoleType.NtfSpecialist:
						if (BreakoutBlitz.ScientistEscapes + 1 >= BreakoutBlitz.RequiredScientistEscapes)
						{
							Map.ClearBroadcasts();
							Cassie.Message("pitch_0.3 .g4 .g4 .g4 .g4");
							Map.Broadcast(10, "<size=70><b><color=blue>NTF Won!\n</color></b></size><i>Restarting Round...</i>");
							Timing.CallDelayed(10, () => Round.Restart());
						}
						else
						{
							BreakoutBlitz.ScientistEscapes++;
						}
						break;
				}
			}
			
			Timing.CallDelayed(0.3f, () =>
			{
				ev.Player.DropItems();
				
				switch (ev.NewRole)
				{
					case RoleType.NtfSpecialist:
						switch (random.Next(6))
						{
							case 0:
								Ntf.MakeNtfEngineer(ev.Player);
								break;
							case 1:
								Ntf.MakeNtfDemo(ev.Player);
								break;
							case 2:
								Ntf.MakeNtfHeavy(ev.Player);
								break;
							case 3:
								Ntf.MakeNtfMedic(ev.Player);
								break;
							case 4:
								Ntf.MakeNtfScout(ev.Player);
								break;
							case 5:
								Ntf.MakeNtfContainmentSpecialist(ev.Player);
								break;
						}
						break;

					case RoleType.NtfPrivate:
						switch (random.Next(6))
						{
							case 0:
								Ntf.MakeNtfEngineer(ev.Player);
								break;
							case 1:
								Ntf.MakeNtfDemo(ev.Player);
								break;
							case 2:
								Ntf.MakeNtfHeavy(ev.Player);
								break;
							case 3:
								Ntf.MakeNtfMedic(ev.Player);
								break;
							case 4:
								Ntf.MakeNtfScout(ev.Player);
								break;
							case 5:
								Ntf.MakeNtfContainmentSpecialist(ev.Player);
								break;
						}
						break;
				
					case RoleType.ChaosConscript:
						switch (random.Next(6))
						{
							case 0:
								CI.MakeChaosHeretic(ev.Player);
								break;
							case 1:
								CI.MakeChaosDemo(ev.Player);
								break;
							case 2:
								CI.MakeChaosBulldozer(ev.Player);
								break;
							case 3:
								CI.MakeChaosHunter(ev.Player);
								break;
							case 4:
								CI.MakeChaosPoisonCarrier(ev.Player);
								break;
							case 5:
								CI.MakeChaosMachinist(ev.Player);
								break;
						}
						break;
				}
			});
		}

		public void OnDying(DyingEventArgs ev)
		{
			if (ev.Target.CustomClassManager().CustomClass?.Name == "Zombie Boomer" && (ev.Handler.Base is FirearmDamageHandler || ev.Handler.Base is ExplosionDamageHandler))
			{
				UtilityMethods.FakeExplode(ev.Target);
				Timing.RunCoroutine(PassiveAbilities.ToxicZone(ev.Target));
			}

			if (SillySundayEventHandler.instantRevive)
			{
				if (ev.Killer != null && ev.Killer.Role.Type == RoleType.Scp049 && ev.Target != ev.Killer)
				{
					ev.Target.Role.Type = RoleType.Scp0492;
					Timing.CallDelayed(0.5f, () =>
					{
						ev.Target.Position = ev.Killer.Position;
					});
				}
			}
			if (SillySundayEventHandler.slaughterhouse)
			{
				Vector3 DeathPosition = ev.Target.Position;
				ev.Target.Broadcast(10, "<i>Respawning in 10 seconds...</i>");
				Timing.CallDelayed(10, () =>
				{
					ev.Target.ClearBroadcasts();
					ev.Target.Role.Type = RoleType.ChaosConscript;
					ev.Target.ClearInventory();
					/*
					ev.Target.AddItem(ItemType.GunLogicer);
					ev.Target.AddItem(ItemType.Medkit);
					ev.Target.AddItem(ItemType.Medkit);
					ev.Target.AddItem(ItemType.KeycardChaosInsurgency);
					ev.Target.Ammo[ItemType.Ammo762x39] = 450;
					*/
					ev.Target.Broadcast(6, "<color=yellow><i>Welcome back...</i>\n<b>Kill the Scientists.</b></color>");
					Timing.CallDelayed(0.5f, () =>
					{
						ev.Target.Position = DeathPosition;
					});
				});
			}

			if (ev.Killer?.Role.Type == RoleType.Scp106)
			{
				ev.Target.ClearInventory();
				PlayerManager.hostHub.characterClassManager.RpcPlaceBlood(ev.Target.ReferenceHub.transform.position, 1, 1);
				DeletePlayerRagdoll.Add(ev.Target);
			}

			if (IsEventServer)
			{
				if (ev.Killer?.Role.Team == Team.SCP && ev.Killer != ev.Target)
				{
					if (BreakoutBlitz.SCPKills + 1 >= BreakoutBlitz.RequiredSCPKills)
					{
						Map.ClearBroadcasts();
						Cassie.Message("pitch_0.3 .g4 .g4 .g4 .g4", isNoisy:false);
						Map.Broadcast(10, "<size=70><b><color=red>SCPs Won!\n</color></b></size><i>Restarting Round...</i>");
						Timing.CallDelayed(10, ()=>Round.Restart());
					}
					else
					{
						BreakoutBlitz.SCPKills++;
					}
				}

				switch (ev.Target.Role.Team)
				{
					case Team.CHI:
					case Team.CDP:
						if (Stopwatch.ElapsedMilliseconds >= 600000)
						{
							if (UtilityMethods.RandomChance(3))
							{
								Timing.CallDelayed(10, () =>
								{
									if (!RoundActive)
										return;
									ev.Target.SetRole(RoleType.ChaosRifleman);
								});

								/*
								Timing.CallDelayed(12, () =>
								{
									if (!RoundActive)
										return;
									switch (random.Next(6))
									{
										case 0:
											CI.MakeChaosHeretic(ev.Target);
											break;
										case 1:
											CI.MakeChaosDemo(ev.Target);
											break;
										case 2:
											CI.MakeChaosBulldozer(ev.Target);
											break;
										case 3:
											CI.MakeChaosHunter(ev.Target);
											break;
										case 4:
											CI.MakeChaosPoisonCarrier(ev.Target);
											break;
										case 5:
											CI.MakeChaosMachinist(ev.Target);
											break;
									}
								});*/
							}
							else
							{
								Timing.CallDelayed(10, () =>
								{
									if (!RoundActive)
										return;
									ev.Target.SetRole(RoleType.ClassD);
								});
								
								Timing.CallDelayed(12, () =>
								{
									if (!RoundActive)
										return;
									ev.Target.Position = RoleType.Scientist.GetRandomSpawnProperties()
										.Item1;
									
									if (UtilityMethods.RandomChance(2))
										ev.Target.AddItem(ItemType.KeycardJanitor);
								
									if (UtilityMethods.RandomChance(5))
										ev.Target.AddItem(ItemType.GunCOM15);
								});
							}
						}
						else
						{
							Timing.CallDelayed(10, () =>
							{
								if (!RoundActive)
									return;
								ev.Target.SetRole(RoleType.ClassD);
							});
							
							Timing.CallDelayed(12, () =>
							{
								if (!RoundActive)
									return;
								ev.Target.Position = RoleType.Scientist.GetRandomSpawnProperties()
									.Item1;

								if (UtilityMethods.RandomChance(2))
									ev.Target.AddItem(ItemType.KeycardJanitor);
								
								if (UtilityMethods.RandomChance(5))
									ev.Target.AddItem(ItemType.GunCOM15);
							});
						}
						ev.Target.Broadcast(10, "<b>Respawning in 10 seconds...</b>");
						break;
					
					case Team.MTF:
					case Team.RSC:
						if (Stopwatch.ElapsedMilliseconds >= 600000)
						{
							if (UtilityMethods.RandomChance(3))
							{
								if (!RoundActive)
									return;
								
								Timing.CallDelayed(10, () =>
								{
									ev.Target.SetRole(RoleType.NtfSergeant);
									
								});

								/*
								Timing.CallDelayed(12, () =>
								{
									if (!RoundActive)
										return;
									switch (random.Next(6))
									{
										case 0:
											Ntf.MakeNtfEngineer(ev.Target);
											break;
										case 1:
											Ntf.MakeNtfDemo(ev.Target);
											break;
										case 2:
											Ntf.MakeNtfHeavy(ev.Target);
											break;
										case 3:
											Ntf.MakeNtfMedic(ev.Target);
											break;
										case 4:
											Ntf.MakeNtfScout(ev.Target);
											break;
										case 5:
											Ntf.MakeNtfContainmentSpecialist(ev.Target);
											break;
									}
								});*/
							}
							else
							{
								Timing.CallDelayed(10, () =>
								{
									if (!RoundActive)
										return;
									
									if (UtilityMethods.RandomChance(2))
										ev.Target.SetRole(RoleType.Scientist);
									else
										ev.Target.SetRole(RoleType.FacilityGuard);
								});
							}
						}
						else
						{
							Timing.CallDelayed(10, () =>
							{
								if (!RoundActive)
									return;
									
								if (UtilityMethods.RandomChance(2))
									ev.Target.SetRole(RoleType.Scientist);
								else
									ev.Target.SetRole(RoleType.FacilityGuard);
							});
						}
						ev.Target.Broadcast(10, "<b>Respawning in 10 seconds...</b>");
						break;
					
					case Team.SCP:
						RoleType role = ev.Target.Role.Type;

						Timing.CallDelayed(10, () =>
						{
							ev.Target.SetRole(role);
						});
						
						Timing.CallDelayed(13, () =>
						{
							ev.Target.Health /= 5;
							ev.Target.MaxHealth /= 5;
						});
						
						ev.Target.Broadcast(10, "<b>Respawning in 10 seconds...</b>");
						break;
				}
			}

			if (ev.Killer != null && RecentlyUncuffed.Contains(ev.Target.Id) && (ev.Killer.Role.Team != Team.SCP && ev.Target != ev.Killer))
			{
				List<Player> RAPermsPlayers = Player.List.Where(r => r.ReferenceHub.serverRoles.RemoteAdmin).ToList();
				string modsOnline = "";
				foreach (Player Ply in RAPermsPlayers)
				{
					if (Ply.RankName.Contains("Moderator") || Ply.RankName.Contains("Admin") || Ply.RawUserId == "kognity")
					{
						Ply.Broadcast(10, $"Suspected undetain to kill detected, check Discord Channel for info.",
							Broadcast.BroadcastFlags.AdminChat);
						modsOnline += $"{Ply.Nickname} ";
					}
				}
				if (Server.Port == 7777)
				{
					SendMessageAsync($"Suspected Detained Kill (undetain to kill) on **Server #1**:\n```Killer: {ev.Killer.Nickname} ({ev.Killer.UserId}) as {ev.Killer.Role.Type.ToString()}\nDetained Victim: {ev.Target.Nickname} ({ev.Target.UserId}) as {ev.Target.Role.Type.ToString()}\nUncuffer: {Player.Get(RecentlyUncuffer[ev.Target.Id]).Nickname} ({Player.Get(RecentlyUncuffer[ev.Target.Id]).UserId}) as {Player.Get(RecentlyUncuffer[ev.Target.Id]).Role.Type.ToString()}\nMods Online: {modsOnline}```", true);
				}
				else if (Server.Port == 8888)
				{
					SendMessageAsync($"Suspected Detained Kill (undetain to kill) on **Server #2**:\n```Killer: {ev.Killer.Nickname} ({ev.Killer.UserId}) as {ev.Killer.Role.Type.ToString()}\nDetained Victim: {ev.Target.Nickname} ({ev.Target.UserId}) as {ev.Target.Role.Type.ToString()}\nUncuffer: {Player.Get(RecentlyUncuffer[ev.Target.Id]).Nickname} ({Player.Get(RecentlyUncuffer[ev.Target.Id]).UserId}) as {Player.Get(RecentlyUncuffer[ev.Target.Id]).Role.Type.ToString()}\nMods Online: {modsOnline}```", true);
				}
				else if (Server.Port == 9999)
				{
					SendMessageAsync($"Suspected Detained Kill (undetain to kill) on **Server #3**:\n```Killer: {ev.Killer.Nickname} ({ev.Killer.UserId}) as {ev.Killer.Role.Type.ToString()}\nDetained Victim: {ev.Target.Nickname} ({ev.Target.UserId}) as {ev.Target.Role.Type.ToString()}\nUncuffer: {Player.Get(RecentlyUncuffer[ev.Target.Id]).Nickname} ({Player.Get(RecentlyUncuffer[ev.Target.Id]).UserId}) as {Player.Get(RecentlyUncuffer[ev.Target.Id]).Role.Type.ToString()}\nMods Online: {modsOnline}```", true);
				}
				else if (Server.Port == 7778)
				{
					SendMessageAsync($"Suspected Detained Kill (undetain to kill) on **Dev Server**:\n```Killer: {ev.Killer.Nickname} ({ev.Killer.UserId}) as {ev.Killer.Role.Type.ToString()}\nDetained Victim: {ev.Target.Nickname} ({ev.Target.UserId}) as {ev.Target.Role.Type.ToString()}\nUncuffer: {Player.Get(RecentlyUncuffer[ev.Target.Id]).Nickname} ({Player.Get(RecentlyUncuffer[ev.Target.Id]).UserId}) as {Player.Get(RecentlyUncuffer[ev.Target.Id]).Role.Type.ToString()}\nMods Online: {modsOnline}```", true);
				}
				else
				{
					SendMessageAsync($"Suspected Detained Kill (undetain to kill) on **Server with port: {Server.Port}**:\n```Killer:{ev.Killer.Nickname} ({ev.Killer.UserId}) as {ev.Killer.Role.Type.ToString()}\nDetained Victim: {ev.Target.Nickname} ({ev.Target.UserId}) as {ev.Target.Role.Type.ToString()}\nUncuffer: {Player.Get(RecentlyUncuffer[ev.Target.Id]).Nickname}\nMods Online: {modsOnline}```", true);
				}
			}

			if (ev.Killer != null && ev.Target.IsCuffed && ev.Killer.Role.Team != Team.SCP && ev.Target != ev.Killer)
			{
				List<Player> RAPermsPlayers = Player.List.Where(r => r.ReferenceHub.serverRoles.RemoteAdmin).ToList();
				string modsOnline = "";
				foreach (Player Ply in RAPermsPlayers)
				{
					if (Ply.RankName.Contains("Moderator") || Ply.RankName.Contains("Admin") || Ply.RawUserId == "kognity")
					{
						Ply.Broadcast(10,
							$"{ev.Killer.Nickname} as {ev.Killer.Role.Type.ToString()} killed {ev.Target.Nickname} as {ev.Target.Role.Type.ToString()} detained by {ev.Target.Cuffer.Nickname}",
							Broadcast.BroadcastFlags.AdminChat);
						modsOnline += $"{Ply.Nickname} ";
					}
				}
				if (Server.Port == 7777)
				{
					SendMessageAsync($"Detained Kill on **Server #1**:\n```Killer: {ev.Killer.Nickname} ({ev.Killer.UserId}) as {ev.Killer.Role.Type.ToString()}\nDetained Victim: {ev.Target.Nickname} ({ev.Target.UserId}) as {ev.Target.Role.Type.ToString()}\nDetainer: {ev.Target.Cuffer.Nickname}\nMods Online: {modsOnline}```", true);
				}
				else if (Server.Port == 8888)
				{
					SendMessageAsync($"Detained Kill on **Server #2**:\n```Killer: {ev.Killer.Nickname} ({ev.Killer.UserId}) as {ev.Killer.Role.Type.ToString()}\nDetained Victim: {ev.Target.Nickname} ({ev.Target.UserId}) as {ev.Target.Role.Type.ToString()}\nDetainer: {ev.Target.Cuffer.Nickname}\nMods Online: {modsOnline}```", true);
				}
				else if (Server.Port == 9999)
				{
					SendMessageAsync($"Detained Kill on **Server #3**:\n```Killer: {ev.Killer.Nickname} ({ev.Killer.UserId}) as {ev.Killer.Role.Type.ToString()}\nDetained Victim: {ev.Target.Nickname} ({ev.Target.UserId}) as {ev.Target.Role.Type.ToString()}\nDetainer: {ev.Target.Cuffer.Nickname}\nMods Online: {modsOnline}```", true);
				}
				else if (Server.Port == 7778)
				{
					SendMessageAsync($"Detained Kill on **Dev Server**:\n```Killer: {ev.Killer.Nickname} ({ev.Killer.UserId}) as {ev.Killer.Role.Type.ToString()}\nDetained Victim: {ev.Target.Nickname} ({ev.Target.UserId}) as {ev.Target.Role.Type.ToString()}\nDetainer: {ev.Target.Cuffer.Nickname}\nMods Online: {modsOnline}```", true);
				}
				else
				{
					SendMessageAsync($"Detained Kill on **Server with port: {Server.Port}**:\n```Killer:{ev.Killer.Nickname} ({ev.Killer.UserId}) as {ev.Killer.Role.Type.ToString()}\nDetained Victim: {ev.Target.Nickname} ({ev.Target.UserId}) as {ev.Target.Role.Type.ToString()}\nDetainer: {ev.Target.Cuffer.Nickname}\nMods Online: {modsOnline}```", true);
				}
			}
		}

		public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
		{
			if (GrenadesToFake.Contains(ev.Grenade))
			{
				ev.IsAllowed = false;
				GrenadesToFake.Remove(ev.Grenade);
			}
		}

		public void OnHandcuff(HandcuffingEventArgs ev)
		{
			if (ev.Target.Role.Type == RoleType.Tutorial)
			{
				ev.IsAllowed = false;
				return;
			}
		}
		
		public void OnRemoveHandcuffs(RemovingHandcuffsEventArgs ev)
		{
			try
			{
				if (!RecentlyUncuffed.Contains(ev.Target.Id))
					RecentlyUncuffed.Add(ev.Target.Id);

				if (!RecentlyUncuffer.ContainsKey(ev.Cuffer.Id))
					RecentlyUncuffer.Add(ev.Target.Id, ev.Cuffer.Id);

				Timing.CallDelayed(5, () =>
				{
					if (RecentlyUncuffed.Contains(ev.Target.Id))
						RecentlyUncuffed.Remove(ev.Target.Id);

					if (RecentlyUncuffer.ContainsKey(ev.Target.Id))
						RecentlyUncuffer.Remove(ev.Target.Id);
				});
			}
			catch
			{

			}
		}

		public void OnRevive(FinishingRecallEventArgs ev)
		{
			if (SillySundayEventHandler.randomrevive)
			{
				Timing.CallDelayed(0.5f, () =>
				{
					RoleType[] scp_list = new RoleType[] { RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989 };
					int random_scp = random.Next(5);
					ev.Target.Role.Type = scp_list[random_scp];
					Timing.CallDelayed(0.5f, () =>
					{
						ev.Target.Position = ev.Scp049.Position;
					});
					Timing.CallDelayed(1.7f, () =>
					{
						ev.Target.Health *= 0.3f;
					});
				});
				return;
			}
			if (SillySunday)
			{
				ev.Target.Broadcast(10, "<b>Type in 'cmdbind f .zfe' in console.\nThen press F to explode!!</b>");
			}

			Timing.CallDelayed(0.5f, () =>
			{
				PickRandomZombieVariant(ev.Target);
			});
		}

		public void OnShooting(ShootingEventArgs ev)
		{
			if(ev.Shooter.CustomClassManager().CustomClass?.Name == "NTF Containment Specialist" || ev.Shooter.CustomClassManager().CustomClass?.Name == "Chaos Containment Specialist")
			{
				if (ev.Shooter.CurrentItem.Type == ItemType.GunRevolver)
				{
					// Tracer
					var circle = UnityEngine.Object.Instantiate(Utils.PrimitiveBaseObject);
					circle.NetworkPrimitiveType = PrimitiveType.Cylinder;
					circle.NetworkMaterialColor = new Color(1, 0, 0, 0.7f);
					circle.NetworkMovementSmoothing = 60;
					//circle.transform.position = ev.Shooter.CameraTransform.position;

					Vector3 point;
					if (Physics.Raycast(
						    ev.Shooter.CameraTransform.position + ev.Shooter.CameraTransform.forward * 0.5f,
						    ev.Shooter.CameraTransform.forward,
						    out RaycastHit hit,
						    500,
						    LayerMask.GetMask("Default", "Hitbox", "Glass", "CCTV", "Door", "Locker")))
					{
						point = hit.point;
					}
					else
					{
						point = ev.Shooter.CameraTransform.position + ev.Shooter.CameraTransform.forward * 500;
					}

					var origin = ev.Shooter.CameraTransform.position + Vector3.down * 0.1f +
					             ev.Shooter.CameraTransform.right * 0.2f;

					Vector3 pos = ev.Shooter.CameraTransform.position;

					Timing.CallDelayed(0.1f, () =>
					{
						circle.transform.position = (origin + point) / 2;
						circle.transform.localScale = new Vector3(-0.05f,
							-Math.Abs(Vector3.Distance(pos, point)) / 2, -0.05f);
						circle.transform.rotation = Quaternion.LookRotation((point - origin).normalized);
						circle.transform.rotation = Quaternion.Euler(circle.transform.rotation.eulerAngles.x - 90,
							circle.transform.rotation.eulerAngles.y, 0);

						NetworkServer.Spawn(circle.gameObject);
						circle.UpdatePositionServer();
						Timing.RunCoroutine(UtilityMethods.FadeAway(circle));
					});
					
					// Recoil
					var pos1 = ev.Shooter.CameraTransform.position + ev.Shooter.CameraTransform.forward +
					          Vector3.up * 0.3f;
					
					ev.Shooter.CameraTransform.LookAt(pos1);
					ev.Shooter.Rotation = new Vector2(0f - ev.Shooter.CameraTransform.eulerAngles.x, ev.Shooter.CameraTransform.eulerAngles.y);
				}
			}

			var p = Player.Get(ev.TargetNetId);
			// 106 stuff
			if (p != null)
			{
				if (p.Role.Type == RoleType.Scp106)
				{
					SCP106CustomClass customClass = (SCP106CustomClass) p.CustomClassManager().CustomClass;
					Vanish ability = (Vanish) customClass.ActiveAbilities[0];
					
					if (ability.IsVanish)
					{
						// If they are NOT in the list they can DO damage
						if (!p.TargetGhostsHashSet.Contains(ev.Shooter.Id) && !ev.Shooter.TargetGhostsHashSet.Contains(p.Id))
						{
							ev.IsAllowed = true;
						}
						else
						{
							ev.IsAllowed = false;
						}
					}
				}
			}


			if (IsDevServer)
			{
				/*
				var handler = new CustomReasonDamageHandler("T H E B I G O N E", float.MaxValue);
				handler.StartVelocity = ev.Shooter.CameraTransform.forward * 10
				                        + ev.Shooter.CameraTransform.up;
				var ragdoll = new Exiled.API.Features.Ragdoll(new RagdollInfo(Exiled.API.Features.Server.Host.ReferenceHub, handler, RoleType.Scientist, ev.Shooter.Position, ev.Shooter.CameraTransform.rotation, "Tiny Dr.Coomer", 1.0));
				ragdoll.Scale = new Vector3(0.7f, 0.7f, 0.7f);
					
				ragdoll.Spawn();

				Timing.CallDelayed(8f, () =>
				{
					ragdoll.Delete();
				});*/
				
				/*
				var gasGrenade = MapUtils.GetSchematicDataByName("ChefHat1");
				var gasGrenadeObject = ObjectSpawner.SpawnSchematic(new SchematicObject() {SchematicName = "ChefHat1"},
					ev.Shooter.CameraTransform.position + ev.Shooter.CameraTransform.forward*1f + Vector3.up, Quaternion.identity, Vector3.one*0.3f, gasGrenade);

				var t = gasGrenadeObject.gameObject.AddComponent<Rigidbody>();
				t.mass = 0.1f;
				t.AddForce(ev.Shooter.CameraTransform.forward, ForceMode.Impulse);

				Timing.CallDelayed(10, () => gasGrenadeObject.Destroy());
*/
			}
			
			/*
			if (IsDevServer)
			{
				var item = (Item)Item.Create(ev.Shooter.CurrentItem.Type).Spawn(ev.Shooter.CameraTransform.position);
				item.Base.Rb.velocity = ev.Shooter.CameraTransform.forward * 10;
			}*/
			if (ContentGun.ContainsKey(ev.Shooter))
			{
				if (ContentGun[ev.Shooter].First > ContentGun[ev.Shooter].Second)
				{
					ev.IsAllowed = false;

					ContentGun[ev.Shooter].Second += 1;
					
					var handler = new CustomReasonDamageHandler("Spawned by your nearby patreon supporter :trollface:", float.MaxValue);
					handler.StartVelocity = ev.Shooter.CameraTransform.forward * 10
					                        + ev.Shooter.CameraTransform.up;
					var ragdoll = new Exiled.API.Features.Ragdoll(new RagdollInfo(Exiled.API.Features.Server.Host.ReferenceHub, handler, RoleType.Scientist, ev.Shooter.Position, ev.Shooter.CameraTransform.rotation, "gaming", 1.0));
					ragdoll.Scale = new Vector3(UnityEngine.Random.Range(0.25f, 6), UnityEngine.Random.Range(0.25f, 6), UnityEngine.Random.Range(0.25f, 6));
					
					ragdoll.Spawn();

					Timing.CallDelayed(8f, () =>
					{
						ragdoll.Delete();
					});

					ev.Shooter.ShowCenterDownHint($"<color=blue>{ContentGun[ev.Shooter].First - ContentGun[ev.Shooter].Second} ragdolls left</color>",1);
				}
				else
				{
					if (ContentGun[ev.Shooter].First != -1)
					{
						ev.Shooter.Broadcast(5, "<i>You have run out of content gun rounds!</i>");
						ContentGun[ev.Shooter].First = -1;
						ContentGun[ev.Shooter].Second = -1;
					}
				}
			}

			/*
			Vector3 forward = ev.Shooter.CameraTransform.forward;
			if (Physics.Raycast(ev.Shooter.CameraTransform.position + forward, forward, out var hit, 500))
			{
				Pickup sticky = hit.collider.gameObject.GetComponentInParent<Pickup>();
				if (sticky == null)
				{
					return;
				}

				KeyValuePair<Player, Pickup[]> playerAndSticky = StickyPositions.Where(x => x.Value.Contains(sticky)).FirstOrDefault();
				if (playerAndSticky.Value == null || playerAndSticky.Key == null) return;

				if (playerAndSticky.Key.Role.Team == ev.Shooter.Role.Team && playerAndSticky.Key != ev.Shooter) return;

				StickyPositions[playerAndSticky.Key][playerAndSticky.Value.IndexOf(sticky)] = null;
				NetworkServer.Destroy(sticky.Base.gameObject);
			}*/
		}

		public void RagdollSpawning(SpawningRagdollEventArgs ev)
		{
			if (DeletePlayerRagdoll.Contains(ev.Owner))
			{
				ev.IsAllowed = false;
				DeletePlayerRagdoll.Remove(ev.Owner);
			}
				/*
				if (Server.Port == 7778)
				{
					ev.Velocity *= 2.5f;
				}
	
				var d = Donator.Donators.Where(x => x.UserId == ev.Owner.RawUserId).FirstOrDefault();
				if (d != null)
					if (d.DonatorNum >= 3)
						ev.Velocity *= 2.5f;
				return;
				*/
			Timing.RunCoroutine(UtilityMethods.DensifyAmmoBoxes(ev));
		}

		public void OnSetClass(ChangingRoleEventArgs ev)
		{
			ev.Player.IsGodModeEnabled = ev.NewRole == RoleType.Tutorial;
			
			if(ev.NewRole != RoleType.Tutorial && ev.Player.Role.Type != RoleType.Tutorial)
				ev.Player.CustomClassManager().DisposeCustomClass();

			switch (ev.NewRole)
			{
				case RoleType.Scp049:
					ev.Player.CustomClassManager().DisposeCustomClass();
					ev.Player.CustomClassManager().CustomClass = new SCP049CustomClass(ev.Player);
					break;
				
				case RoleType.Scp079:
					ev.Player.CustomClassManager().DisposeCustomClass();
					ev.Player.CustomClassManager().CustomClass = new SCP079CustomClass(ev.Player);
					break;
				
				case RoleType.Scp106:
					ev.Player.CustomClassManager().DisposeCustomClass();
					ev.Player.CustomClassManager().CustomClass = new SCP106CustomClass(ev.Player);
					break;
				
				case RoleType.Scp096:
					ev.Player.CustomClassManager().DisposeCustomClass();
					ev.Player.CustomClassManager().CustomClass = new SCP096CustomClass(ev.Player);
					break;
				
				case RoleType.Scp173:
					ev.Player.CustomClassManager().DisposeCustomClass();
					ev.Player.CustomClassManager().CustomClass = new SCP173CustomClass(ev.Player);
					break;
				
				case RoleType.Scp93953:
					ev.Player.CustomClassManager().DisposeCustomClass();
					ev.Player.CustomClassManager().CustomClass = new SCP93953CustomClass(ev.Player);
					break;
				
				case RoleType.Scp93989:
					ev.Player.CustomClassManager().DisposeCustomClass();
					ev.Player.CustomClassManager().CustomClass = new SCP93989CustomClass(ev.Player);
					break;
				
				case RoleType.NtfSergeant:
					ev.Player.CustomClassManager().DisposeCustomClass();
					ev.Player.CustomClassManager().CustomClass = new NTFSergeant(ev.Player);
					break;
				
				case RoleType.NtfCaptain:
					ev.Player.CustomClassManager().DisposeCustomClass();
					ev.Player.CustomClassManager().CustomClass = new NTFCaptain(ev.Player);
					break;
			}

			if (ev.NewRole == RoleType.Scp0492 && SillySunday)
			{
				ev.Player.Broadcast(12, "<b>Type in 'cmdbind f.zfe' in console.\nThen press F to explode!!</b>");
			}
		}

		public void OnDied(DiedEventArgs ev)
		{
			if (!ev.Target.IsVerified)
				return;

			if (SillySundayEventHandler.ohfiverescuemode)
			{
				if(ev.Target == SillySundayEventHandler.OhFivePlayer)
				{
					Map.Broadcast(10, "The 05 has died. Chaos wins! Facility wipe initiated!");
					Warhead.Start();
					Warhead.IsLocked = true;
					SillySundayEventHandler.ohfivedied = true;
					SillySundayEventHandler.OhFivePlayer = null;
				}

				if (SillySundayEventHandler.ohfivedied)
				{
					ev.Target.Broadcast(10, "<i>Respawning in 10 seconds...</i>");
					Timing.CallDelayed(10, () =>
					{
						ev.Target.ClearBroadcasts();
						ev.Target.Role.Type = RoleType.ChaosConscript;
						ev.Target.ClearInventory();
						/*
						ev.Target.AddItem(ItemType.GunLogicer);
						ev.Target.AddItem(ItemType.Medkit);
						ev.Target.AddItem(ItemType.Medkit);
						ev.Target.AddItem(ItemType.KeycardChaosInsurgency);
						ev.Target.Ammo[ItemType.Ammo762x39] = 450;
						*/
						ev.Target.Broadcast(6, "<color=yellow><i>Welcome back...</i>\n<b>Kill the remaining foundation personnel.</b></color>");
					});
				}
			}

			if (SillySundayInfectionController.InfectionEnabled)
			{
				if (ev.Killer.Role.Type == SillySundayInfectionController.InfectedRole)
				{
					ev.Target.Role.Type = ev.Killer.Role.Type;
					Timing.CallDelayed(0.5f, () =>
					{
						ev.Target.Position = ev.Killer.Position;
					});
				}				
			}
			
			if (InfectedPlayers.Contains(ev.Target))
			{
				if (ev.Killer != null && ev.Killer != ev.Target) // If its not a suicide
				{
					InfectedPlayers.Remove(ev.Target);
					Timing.RunCoroutine(ChangeToSCP(ev.Target, ev.Killer, false));
				}
				else // This was a suicide
				{
					// Lets see if 049 is alive
					foreach (Player Ply in Player.List)
					{
						if (Ply.Role.Type == RoleType.Scp049)
						{
							InfectedPlayers.Remove(ev.Target);

							Timing.RunCoroutine(ChangeToSCP(ev.Target, Ply, false));
							return;
						}
					}
					
					// 049 wasn't alive let's see if ANY zombies are alive
					foreach (Player Ply in Player.List)
					{
						if (Ply.Role.Type == RoleType.Scp0492)
						{
							InfectedPlayers.Remove(ev.Target);

							Timing.RunCoroutine(ChangeToSCP(ev.Target, Ply, false));
							return;
						}
					}
					
					// Welp no zombies or 049 is alive. Put the poor guy on surface.
					InfectedPlayers.Remove(ev.Target);
					Timing.RunCoroutine(ChangeToSCP(ev.Target, ev.Killer, true));
				}
			}

			if (!DoNotSpawn.Contains(ev.Target.UserId) && ev.Target.IsVerified && ev.Handler.Type == DamageType.Unknown)
			{
				if (ev.Killer == null || ev.Killer.Role.Team == Team.SCP)
				{
					DoNotSpawn.Add(ev.Target.UserId);
				}
			}

			if (IsDeathMatchServer)
			{
				if (ev.Killer != null)
				{
					int index = KingAndCompetitor.GetIndex(ev.Killer);
					var obj = KingAndCompetitor.GetAtIndex(index);
					Timing.KillCoroutines(obj.StalemateChecker);
					Timing.KillCoroutines(obj.Unfreezer);
					if (Deathmatch.roundPaused)
					{
						Timing.CallDelayed(0.5f, () =>
						{
							UtilityMethods.CleanupRagdollsAndItems();
						});
						return;
					}

					if ((KingAndCompetitor.IsKing(ev.Killer) || KingAndCompetitor.IsKing(ev.Target)) && ev.Killer != ev.Target)
					{
						if (Deathmatch.playerAndWins.ContainsKey(ev.Killer.UserId))
						{
							Deathmatch.playerAndWins[ev.Killer.UserId] += 1;
						}
						else
						{
							Deathmatch.playerAndWins.Add(ev.Killer.UserId, 1);
						}

						//RespawnEffectsController.ClearQueue();
						//RespawnEffectsController.ClearQueue();

						// Giving notifications to the Winner and Loser specifically
						//ev.Killer.PlayCassieAnnouncement("pitch_0.3 .g5 .g5 .g5", false, false);
						//ev.Target.PlayCassieAnnouncement("pitch_0.3 .g5 .g5 .g5", false, false);

						ev.Killer.Broadcast(10, $"{ev.Killer.Nickname} won!\nThey have {Deathmatch.playerAndWins[ev.Killer.UserId]} total wins!");
						ev.Target.Broadcast(10, $"{ev.Killer.Nickname} won!\nThey have {Deathmatch.playerAndWins[ev.Killer.UserId]} total wins!");

						// Giving notifications to global players who are not actively playing
						foreach(Player p in Player.List)
						{
							if(p.Role.Type == RoleType.Tutorial || p.Role.Type == RoleType.Spectator && p != ev.Target)
								p.Broadcast(3, $"{ev.Killer.Nickname} won!\nThey have {Deathmatch.playerAndWins[ev.Killer.UserId]} total wins!");
						}

						if (!ev.Killer.DoNotTrack)
						{
							if (dmWins.ContainsKey(ev.Killer))
								dmWins[ev.Killer] += 1;
							else
								dmWins.Add(ev.Killer, 1);
						}
						if (!ev.Target.DoNotTrack)
						{
							if (dmLosses.ContainsKey(ev.Target))
								dmLosses[ev.Target] += 1;
							else
								dmLosses.Add(ev.Target, 1);
						}

						// King lost
						if (!KingAndCompetitor.IsKing(ev.Killer))
						{
							Player newKing = obj.Competitor;
							Player oldKing = obj.King;
							obj.Queue.Add(oldKing); // Add old king to player queue

							obj.King = newKing; // Set new King as the killer of the old king.
							obj.Competitor = obj.Queue[0]; // Set competitor to the first person in the queue.
							obj.Queue.Remove(obj.Competitor); // Now remove them from the queue.



							/*
						playerQueue.Add(ev.Target); // Add the old king to the player queue

						King[King.IndexOf(ev.Target)] = ev.Killer; // Replacing the old king with the new king
						playerQueue.Remove(ev.Killer); // Remove the new king from the player queue
						*/
						}
						else
						{
							// King won, NEXT!
							obj.Queue.Add(obj.Competitor);
							obj.Competitor = obj.Queue[0]; // Set competitor to the first person in the queue.
							obj.Queue.Remove(obj.Competitor); // Now remove them from the queue.
						}

						Timing.CallDelayed(0.5f, () =>
						{
							UtilityMethods.CleanupRagdollsAndItems();
						});

						Timing.CallDelayed(5, () =>
						{
							ev.Killer.Role.Type = RoleType.Spectator;
						});

						Timing.CallDelayed(7, () =>
						{
							Deathmatch.StartMatch(obj);
						});
					}
				}
			}
			if (ev.Killer != null && !ev.Killer.DoNotTrack)
			{
				if (ev.Handler.Base is UniversalDamageHandler universalDamageHandler1)
				{
					if (universalDamageHandler1.TranslationId == DeathTranslations.PocketDecay.Id)
					{
						IEnumerable<Player> player106 = Player.List.Where(ply => ply.Role.Type == RoleType.Scp106);
						foreach (Player ply in player106)
						{
							if (!ply.DoNotTrack)
							{
								if (damageDealt.ContainsKey(ply))
								{
									damageDealt[ply] += (int) Math.Ceiling(ev.Target.Health);
								}
								else
								{
									damageDealt.Add(ply, (int) Math.Ceiling(ev.Target.Health));
								}
							}
						}
					}
				}
				if (kills.ContainsKey(ev.Killer))
				{
					kills[ev.Killer] += 1;
				}
				else
				{

					kills.Add(ev.Killer, 1);
				}
			}
			if (!ev.Target.DoNotTrack)
			{
				if (deaths.ContainsKey(ev.Target))
				{
					deaths[ev.Target] += 1;
				}
				else
				{

					deaths.Add(ev.Target, 1);
				}
			}

			if (ev.Handler.Base is UniversalDamageHandler universalDamageHandler2)
			{
				if (universalDamageHandler2.TranslationId == DeathTranslations.PocketDecay.Id)
				{
					Player ply = Player.List.Where(x => x.Role.Type == RoleType.Scp106).FirstOrDefault();
					if (ply != null)
					{
						AddGeneralKills(ply);
						AddSCPKills(ply);
						return;
					}
				}
				else
					return;
			}

			if (ev.Killer == null || ev.Killer == ev.Target) return;
			switch (ev.Killer.Role.Team)
			{
				case Team.SCP:
					AddGeneralKills(ev.Killer);
					AddSCPKills(ev.Killer);
					break;
				case Team.MTF:
					AddGeneralKills(ev.Killer);
					AddSquadKills(ev.Killer);
					AddHumanKills(ev.Killer);
					break;
				case Team.CDP:
					AddGeneralKills(ev.Killer);
					AddHumanKills(ev.Killer);
					break;
				case Team.RSC:
					AddGeneralKills(ev.Killer);
					AddHumanKills(ev.Killer);
					break;
				case Team.CHI:
					AddGeneralKills(ev.Killer);
					AddHumanKills(ev.Killer);
					break;
			}
		}

		public void AddGeneralKills(Player ply)
		{
			if (GeneralKills.ContainsKey(ply))
				GeneralKills[ply] += 1;
			else
				GeneralKills.Add(ply, 1);
		}

		public void AddSCPKills(Player ply)
		{
			if (SCPKills.ContainsKey(ply))
				SCPKills[ply] += 1;
			else
				SCPKills.Add(ply, 1);
		}

		public void AddHumanKills(Player ply)
		{
			if (HumanKills.ContainsKey(ply))
				HumanKills[ply] += 1;
			else
				HumanKills.Add(ply, 1);
		}

		public void AddSquadKills(Player ply)
		{
			if (MTFSquadKills.ContainsKey(ply.ReferenceHub.characterClassManager.CurUnitName))
				MTFSquadKills[ply.ReferenceHub.characterClassManager.CurUnitName] += 1;
			else
				MTFSquadKills.Add(ply.ReferenceHub.characterClassManager.CurUnitName, 1);
		}

		public void OnRespawn(RespawningTeamEventArgs ev)
		{
			if (IsEventServer)
			{
				ev.IsAllowed = false;
				return;
			}
			if (IsDeathMatchServer)
			{
				ev.IsAllowed = false;
				return;
			}
			else if (SillySundayEventHandler.NerfWarMode || SillySundayEventHandler.slaughterhouse)
			{
				ev.IsAllowed = false;
			}
			else
			{
				ev.MaximumRespawnAmount = 100;
				foreach (var player in Player.List)
				{
					if (!player.IsVerified) continue;
					if (player.IsAlive) continue;
					if (player.IsOverwatchEnabled) continue;
					if (!ev.Players.Contains(player))
					{
						ev.Players.Add(player);
					}
				}
				
				List<Player> playersToSpawn = new List<Player>(ev.Players);
				
				Timing.RunCoroutine(ChangeTeamPlayers(ev, playersToSpawn));
				Timing.RunCoroutine(UtilityMethods.SoftCleanItems());
				
				if (ev.NextKnownTeam == SpawnableTeamType.NineTailedFox && !MTFSign)
				{
					//Timing.RunCoroutine(MTFLetter(new Vector3(157,999,-45f)));
					MTFSign = true;
				}
				if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency && !CISign)
				{
					//Timing.RunCoroutine(CILetter(new Vector3(42, 993, -67)));
					CISign = true;
				}

				if (ObjectivePointController.RapidSpawnWaves)
				{
					Timing.CallDelayed(2, () =>
					{
						RespawnManager.Singleton.NextKnownTeam = ObjectivePointController.Team;
						RespawnManager.Singleton._timeForNextSequence = 60;
					});
				}
			}
		}

		private IEnumerator<float> CleanupAmmo()
		{
			yield return Timing.WaitForSeconds(3f);

			foreach (ItemPickupBase item in UnityEngine.Object.FindObjectsOfType<ItemPickupBase>())
			{
				var pickup = new Pickup(item);

				if(pickup.Type.IsAmmo())
					if (Vector3.Distance(new Vector3(179, 993, -59), pickup.Position) <= 9.4)
						pickup.Destroy();
			}
		}

		private IEnumerator<float> ChangeTeamPlayers(RespawningTeamEventArgs ev, List<Player> playersToSpawn)
		{
			if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency && ev.Players.Count != 0)
			{
				var f = (FlashGrenade)FlashGrenade.Create(ItemType.GrenadeFlash);
				f.FuseTime = 0.5f;
				f.SpawnActive(new Vector3(12, 999, -58));
				yield return Timing.WaitForSeconds(0.5f);
				foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, new Vector3(-9f, 1004, -59)) <= 80))
				{
					if (ply.Role.Team != Team.CDP && ply.Role.Team != Team.CHI && ply.Role.Type != RoleType.Scp079 && ply.Role.Type != RoleType.Tutorial)
					{
						ply.EnableEffect<Flashed>(7);
					}
				}
			}
			else
				yield return Timing.WaitForSeconds(0.5f);
			
			if (ev.NextKnownTeam == SpawnableTeamType.NineTailedFox)
			{
				List<Player> Cadets = playersToSpawn.Where(r => r.Role.Type == RoleType.NtfPrivate).ToList();
				List<Player> Lieutenants = playersToSpawn.Where(r => r.Role.Type == RoleType.NtfSergeant).ToList();
				List<Player> Commanders = playersToSpawn.Where(r => r.Role.Type == RoleType.NtfCaptain).ToList();

				if (Cadets.Count != 0)
				{

					Player ContainmentSpecialist = Cadets[random.Next(Cadets.Count)];
					CustomClass.Ntf.MakeNtfContainmentSpecialist(ContainmentSpecialist);
					Cadets.Remove(ContainmentSpecialist);
				}

				for (int x = 0; x < 2; x++)
				{
					if (Cadets.Count != 0)
					{

						Player Engineer = Cadets[random.Next(Cadets.Count)];
						CustomClass.Ntf.MakeNtfEngineer(Engineer);
						Cadets.Remove(Engineer);
					}
					else
						break;

					if (Cadets.Count != 0)
					{

						Player Demoman = Cadets[random.Next(Cadets.Count)];
						CustomClass.Ntf.MakeNtfDemo(Demoman);
						Cadets.Remove(Demoman);
					}
					else
						break;

					if (Cadets.Count != 0)
					{

						Player Medic = Cadets[random.Next(Cadets.Count)];
						CustomClass.Ntf.MakeNtfMedic(Medic);
						Cadets.Remove(Medic);
					}
					else
						break;

					if (Cadets.Count != 0)
					{

						Player Heavy = Cadets[random.Next(Cadets.Count)];
						CustomClass.Ntf.MakeNtfHeavy(Heavy);
						Cadets.Remove(Heavy);
					}
					else
						break;

					if (Cadets.Count != 0)
					{

						Player Scout = Cadets[random.Next(Cadets.Count)];
						CustomClass.Ntf.MakeNtfScout(Scout);
						Cadets.Remove(Scout);
					}
					else
						break;
				}
			}
			else if (ev.NextKnownTeam.ToString() == "ChaosInsurgency")
			{
				List<Player> Shotguns = playersToSpawn.Where(r => r.Role.Type == RoleType.ChaosMarauder).ToList();
				List<Player> Logicers = playersToSpawn.Where(r => r.Role.Type == RoleType.ChaosRepressor).ToList();
				List<Player> Riflemen = playersToSpawn.Where(r => r.Role.Type == RoleType.ChaosRifleman).ToList();
				
				Player Ply;
				foreach (var player in Shotguns)
				{
					CI.MakeChaosHunter(player);
				}
				
				foreach (var player in Logicers)
				{
					CI.MakeChaosBulldozer(player);
				}

				bool flip = false;
				while (true)
				{
					if (Riflemen.Count != 0)
					{
						Ply = Riflemen[random.Next(Riflemen.Count)];
						CustomClass.CI.MakeChaosDemo(Ply);
						Riflemen.Remove(Ply);
					}
					else
						break;
					
					if (Riflemen.Count != 0)
					{
						Ply = Riflemen[random.Next(Riflemen.Count)];
						Riflemen.Remove(Ply);
					}
					else
						break;
					
					if (Riflemen.Count != 0)
					{
						Ply = Riflemen[random.Next(Riflemen.Count)];
						CustomClass.CI.MakeChaosPoisonCarrier(Ply);
						Riflemen.Remove(Ply);
					}
					else
						break;
					
					if (Riflemen.Count != 0)
					{
						Ply = Riflemen[random.Next(Riflemen.Count)];
						CustomClass.CI.MakeChaosHeretic(Ply);
						Riflemen.Remove(Ply);
					}
					else
						break;
					
					if (Riflemen.Count != 0)
					{
						Ply = Riflemen[random.Next(Riflemen.Count)];
						CustomClass.CI.MakeChaosMachinist(Ply);
						Riflemen.Remove(Ply);
					}
					else
						break;
					
					if (Riflemen.Count != 0)
					{
						Ply = Riflemen[random.Next(Riflemen.Count)];
						Riflemen.Remove(Ply);
					}
					else
						break;
				}
			}
		}

		private IEnumerator<float> PromotePlayers()
		{
			yield return Timing.WaitForSeconds(3f);
			List<Player> Guards = Player.List.Where(r => r.Role.Type == RoleType.FacilityGuard).ToList();
			List<Player> Scientists = Player.List.Where(r => r.Role.Type == RoleType.Scientist).ToList();
			List<Player> ClassD = Player.List.Where(r => r.Role.Type == RoleType.ClassD).ToList();

			List<Player> GuardsMessaged = new List<Player> { };

			if (ClassD.Count != 0)
			{
				Player Ply;
				while (true)
				{
					if (ClassD.Count != 0)
					{
						Ply = ClassD[random.Next(ClassD.Count)];
						CustomClass.CDP.MakeClassDChad(Ply);
						ClassD.Remove(Ply);
					}
					else
						break;

					if (ClassD.Count != 0)
					{
						Ply = ClassD[random.Next(ClassD.Count)];
						CustomClass.CDP.MakeClassDJanitor(Ply);
						ClassD.Remove(Ply);
					}
					else
						break;

					if (ClassD.Count != 0)
					{
						Ply = ClassD[random.Next(ClassD.Count)];
						if(UtilityMethods.RandomChance(2))
							Ply.AddItem(ItemType.Coin);
						ClassD.Remove(Ply);
					}
					else
						break;

					if (ClassD.Count != 0)
					{
						Ply = ClassD[random.Next(ClassD.Count)];
						if(UtilityMethods.RandomChance(2))
							Ply.AddItem(ItemType.Coin);
						ClassD.Remove(Ply);
					}
					else
						break;
				}
			}
			if (Guards.Count != 0)
			{
				Player GuardManager = Guards[random.Next(Guards.Count)];
				FGD.MakeGuardManager(GuardManager);
				Guards.Remove(GuardManager);
				GuardsMessaged.Add(GuardManager);
			}

			Player SeniorGuard;
			while (true)
			{
				// I know this says senior guards for the 2nd and 3rd one, just pretend its a normal guard lmao
				if (Guards.Count != 0)
				{
					SeniorGuard = Guards[random.Next(Guards.Count)];
					FGD.MakeSeniorGuard(SeniorGuard);
					Guards.Remove(SeniorGuard);
					GuardsMessaged.Add(SeniorGuard);
				}
				else
					break;

				if (Guards.Count != 0)
				{
					SeniorGuard = Guards[random.Next(Guards.Count)];
					Guards.Remove(SeniorGuard);
				}
				else
					break;

				if (Guards.Count != 0)
				{
					SeniorGuard = Guards[random.Next(Guards.Count)];
					Guards.Remove(SeniorGuard);
				}
				else
					break;
			}

			if (Scientists.Count != 0)
			{
				Player MajorScientist = Scientists[random.Next(Scientists.Count)];
				RSC.MakeMajorScientist(MajorScientist);
				Scientists.Remove(MajorScientist);
			}

			Player Scienist;
			while (true)
			{
				if (Scientists.Count != 0)
				{
					Scienist = Scientists[random.Next(Scientists.Count)];
					RSC.MakeMajorScientist(Scienist);
					Scientists.Remove(Scienist);
				}
				else
					break;

				if (Scientists.Count != 0)
				{
					Scienist = Scientists[random.Next(Scientists.Count)];
					Scientists.Remove(Scienist);
				}
				else
					break;

				if (Scientists.Count != 0)
				{
					Scienist = Scientists[random.Next(Scientists.Count)];
					Scientists.Remove(Scienist);
				}
				else
					break;
			}
		}

		private IEnumerator<float> SpawnPlayer(Player Ply)
		{
			yield return Timing.WaitForSeconds(8f);
			if ((RoundSummary.roundTime) - 8 < 180 && RoundSummary.RoundInProgress() && Ply.IsDead)
			{

				int chance = random.Next(0, 100);

				if (chance < 20)
				{
					Ply.Role.Type = RoleType.FacilityGuard;
					Ply.Position = RoleExtensions.GetRandomSpawnProperties(RoleType.FacilityGuard).Item1;
					Ply.MaxHealth = 100;
					Ply.Health = 100;
				}
				else if (chance < 50)
				{
					Ply.Role.Type = RoleType.Scientist;
					Ply.Position = RoleExtensions.GetRandomSpawnProperties(RoleType.Scientist).Item1;
					Ply.MaxHealth = 100;
					Ply.Health = 100;
				}
				else
				{
					Ply.Role.Type = RoleType.ClassD;
					Ply.Position = RoleExtensions.GetRandomSpawnProperties(RoleType.ClassD).Item1;
					Ply.MaxHealth = 100;
					Ply.Health = 100;
				}
			}
		}

		public async void SendMessageAsync(string message, bool staffChat)
		{
			string staffChatChannelURL = "";
			string webhooksChannelURL = "";
			Dictionary<string, string> values = new Dictionary<string, string>
			{
				{"content", message}
			};

			FormUrlEncodedContent content = new FormUrlEncodedContent(values);

			if (staffChat)
			{
				HttpResponseMessage response = await client.PostAsync(staffChatChannelURL, content);
				string responseString = await response.Content.ReadAsStringAsync();
			}
			else
			{
				HttpResponseMessage response = await client.PostAsync(webhooksChannelURL, content);
				string responseString = await response.Content.ReadAsStringAsync();
			}
		}

		public void CheckandGive05(Player Ply)
		{
			foreach (var item in Ply.Inventory.UserInventory.Items)
			{
				if (item.Value.ItemTypeId == ItemType.KeycardO5)
				{
					Timing.CallDelayed(0.1f, () =>
					{
						Ply.AddItem(ItemType.KeycardO5);
					});
					break;
				}
			}
		}

		public void PickRandomZombieVariant(Player Ply)
		{
			switch (random.Next(5))
			{
				case 0:
					CustomClass.SCP.SCP0492.MedicalStudentZombie(Ply);
					break;
				case 1:
					CustomClass.SCP.SCP0492.SpeedyZombie(Ply);
					break;
				case 2:
					CustomClass.SCP.SCP0492.BoomerZombie(Ply);
					break;
				case 3:
					CustomClass.SCP.SCP0492.Overdoser(Ply);
					break;
				case 4:
					CustomClass.SCP.SCP0492.Overclocker(Ply);
					break;
			}
		}

		//--------------------------------------------------------------------------------------------------------------------------------------------------------------//
		IEnumerator<float> ChangeToSCP(Player Ply, Player Killer, bool Fallback)
		{
			Vector3 pos = Killer.Position;
			yield return Timing.WaitForSeconds(1f);
			
			Ply.Role.Type = RoleType.Scp0492;
			Ply.Scale = new Vector3(1, 1, 1);
			SCP0492.Overclocker(Ply);
			
			yield return Timing.WaitForSeconds(1f);
			
			if (Fallback)
			{
				Ply.Position = RoleExtensions.GetRandomSpawnProperties(RoleType.NtfCaptain).Item1;
			}
			else
			{
				Ply.Position = pos;
			}
		}

		//--------------------------------------------------------------------------------------------------------------------------------------------------------------//
		private IEnumerator<float> CheckEscape()
		{
			List<Player> PlayersAffected = new List<Player>();
			while (RoundActive)
			{
				try
				{
					PlayersAffected.Clear();
					foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, escapeArea) <= Escape.radius))
					{
						if (!ply.IsCuffed) continue;
						if (PlayersAffected.Contains(ply)) continue;

						if (ply.CustomClassManager().CustomClass != null)
						{
							ply.CustomClassManager().CustomClass.Escape();
							PlayersAffected.Add(ply);
							continue;
						}

						switch (ply.Role.Team)
						{
							case Team.MTF:
								ply.Role.Type = RoleType.ChaosRifleman;
								PlayersAffected.Add(ply);
								break;
							case Team.CHI:
								ply.Role.Type = RoleType.NtfSergeant;
								PlayersAffected.Add(ply);
								break;
						}
					}
				}
				catch (Exception e)
				{
					Log.Error(e.ToString());
					Log.Error(e.StackTrace);
				}

				yield return Timing.WaitForSeconds(1f);
			}
		}

		// Taken from RespawnTimer Plugin, modified for Silly Sunday
		private IEnumerator<float> Timer()
		{
			yield return Timing.WaitForSeconds(3);
			while (Round.IsStarted)
			{
				yield return Timing.WaitForSeconds(1);

				if (!Respawn.IsSpawning && false) continue;

				text.Clear();
				
				text.Append("<color=#ff96de>You will respawn in: </color>");

				text.Append("<b>{minutes} min.</b>");
				text.Append(" <b>{seconds} s</b>");

				if (Respawn.IsSpawning)
				{
					text.Replace("{minutes}", (Respawn.TimeUntilRespawn / 60).ToString());
					text.Replace("{seconds}", ((Respawn.TimeUntilRespawn % 60)).ToString());

				}
				else
				{
					text.Replace("{minutes}", ((Respawn.TimeUntilRespawn + 15) / 60).ToString());
					text.Replace("{seconds}", ((Respawn.TimeUntilRespawn + 15) % 60).ToString());
				}


				if (RespawnManager.Singleton.NextKnownTeam != SpawnableTeamType.None)
				{
					if (ObjectivePointController.RapidSpawnWaves)
					{
						RespawnManager.Singleton.NextKnownTeam = ObjectivePointController.Team;
						if(RespawnManager.Singleton._curSequence != RespawnManager.RespawnSequencePhase.SpawningSelectedTeam &&
						   RespawnManager.Singleton._curSequence != RespawnManager.RespawnSequencePhase.PlayingEntryAnimations)
							RespawnManager.Singleton._timeForNextSequence = 60;
					}

					if (SillySundayEventHandler.ohfiverescuemode)
						RespawnManager.Singleton.NextKnownTeam = SpawnableTeamType.ChaosInsurgency;

					text.Append("\n");
					text.Append("You will spawn as: ");

					if (false)
					{
						text.Append($"<color=red>{CustomNotificationMessages.autoNukeMessages.PickRandom()}</color>");
					}
					else if (SillySunday)
					{
						switch (RespawnManager.Singleton.NextKnownTeam)
						{
							case SpawnableTeamType.NineTailedFox:
								text.Append($"<color=blue>{SillySundayNames.NTFSundayNames.PickRandom()}</color>");
								break;
							case SpawnableTeamType.ChaosInsurgency:
								text.Append($"<color=green>{SillySundayNames.ChaosSundayNames.PickRandom()}</color>");
								break;
						}
					}
					else
					{
						switch (RespawnManager.Singleton.NextKnownTeam)
						{
							case SpawnableTeamType.NineTailedFox: text.Append("<color=blue>Nine-Tailed Fox</color>"); break;
							case SpawnableTeamType.ChaosInsurgency: text.Append("<color=green>Chaos Insurgency</color>"); break;
						}
					}
				}

				foreach (Player ply in Player.List)
				{
					if (ply.Role.Team == Team.RIP)
					{
						ply.ShowCenterDownHint(text.ToString(), 1);
					}
				}
			}
		}

		public IEnumerator<float> DeathmatchPlayerStats()
		{
			while (true)
			{
				yield return Timing.WaitForSeconds(180);

				Log.Info("Syncing data");

				string messageToSend = "";

				foreach (Player Ply in kills.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&kill%&@&{kills[Ply]}\n";

				foreach (Player Ply in deaths.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&death%&@&{deaths[Ply]}\n";

				foreach (Player Ply in damageDealt.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&damage%&@&{damageDealt[Ply]}\n";

				foreach (Player Ply in dmWins.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&dmwins%&@&{dmWins[Ply]}\n";

				foreach (Player Ply in dmLosses.Keys)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&dmlosses%&@&{dmLosses[Ply]}\n";

				foreach (Player Ply in deletes)
					messageToSend += $"{Ply.RawUserId}%&@&{Ply.Nickname}%&@&delete%&@&0";				

				if (messageToSend != "")
				{
					if (ws.IsAlive)
						ws.Send(messageToSend);
					else
					{
						ws.Connect();
						Timing.CallDelayed(0.4f, () =>
						{
							ws.Send(messageToSend);
						});
					}
					kills.Clear();
					deaths.Clear();
					damageDealt.Clear();
					dmWins.Clear();
					dmLosses.Clear();
					deletes.Clear();
				}
			}
		}
	}
}