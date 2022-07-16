using System;
using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Interactables.Interobjects.DoorUtils;
using MEC;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;

namespace PlayhousePlugin
{
	public class KingAndCompetitor : IEquatable<KingAndCompetitor>
	{
		public Player King { get; set; }
		public Player Competitor { get; set; }
		public CoroutineHandle StalemateChecker;
		public CoroutineHandle Unfreezer;
		public List<Player> Queue = new List<Player>();

		public static List<KingAndCompetitor> KingAndCompetitors = new List<KingAndCompetitor>
		{
			new KingAndCompetitor(),
			new KingAndCompetitor(),
			new KingAndCompetitor(),
			new KingAndCompetitor(),
		};

		public static KingAndCompetitor GetAtIndex(int index) => KingAndCompetitors[index];

		public bool CanStartMatch
		{
			get { return (King != null && Competitor != null); }
		}

		public int GetIndex()
		{
			foreach (var t in KingAndCompetitors)
			{
				if (t.King == King || t.Competitor == Competitor)
				{
					return KingAndCompetitors.IndexOf(t);
				}
			}

			return -1;
		}

		public static void ClearQueues()
		{
			foreach (var t in KingAndCompetitors)
			{
				t.ClearQueue();
			}
		}

		public void ClearQueue()
		{
			Queue.Clear();
		}

		public static int GetIndex(Player Ply)
		{
			foreach (var t in KingAndCompetitors)
			{
				if (t.King == Ply || t.Competitor == Ply)
				{
					return KingAndCompetitors.IndexOf(t);
				}
			}

			return -1;
		}

		public static bool IsKing(Player ply)
		{
			foreach (var t in KingAndCompetitors)
			{
				if (t.King == ply)
				{
					return true;
				}
			}

			return false;
		}

		public static bool IsCompetitor(Player ply)
		{
			foreach (var t in KingAndCompetitors)
			{
				if (t.Competitor == ply)
				{
					return true;
				}
			}

			return false;
		}

		public static bool IsPlayer(Player ply)
		{
			foreach (var t in KingAndCompetitors)
			{
				if (t.King == ply || t.Competitor == ply)
				{
					return true;
				}
			}

			return false;
		}

		public static bool IsInQueue(Player ply)
		{
			foreach (var t in KingAndCompetitors)
			{
				if (t.Queue.Contains(ply))
					return true;
			}

			return false;
		}

		public static int FindQueueIndex(Player ply)
		{
			if (!IsInQueue(ply))
				return -1;

			foreach (var t in KingAndCompetitors)
			{
				if (t.Queue.Contains(ply))
					return KingAndCompetitors.IndexOf(t);
			}

			return -1;
		}

		public static void RemoveFromQueue(Player ply)
		{
			if (IsInQueue(ply))
				GetAtIndex(FindQueueIndex(ply)).Queue.Remove(ply);
		}

		public static void AddToQueue(Player ply, int arenaIndex)
		{
			if (IsInQueue(ply))
			{
				RemoveFromQueue(ply);
			}

			GetAtIndex(arenaIndex).Queue.Add(ply);
		}

		public bool Equals(KingAndCompetitor other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Queue, other.Queue) && Equals(King, other.King) && Equals(Competitor, other.Competitor);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((KingAndCompetitor) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Queue != null ? Queue.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (King != null ? King.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Competitor != null ? Competitor.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(KingAndCompetitor left, KingAndCompetitor right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(KingAndCompetitor left, KingAndCompetitor right)
		{
			return !Equals(left, right);
		}
	}
	public class Deathmatch
	{
		public static void SetupArenas()
		{
			var pickups = new List<Pickup>();
			var doorPrefab = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Where(x => x.name == "HCZ BreakableDoor").FirstOrDefault();
			// Arena 2 Doors (EZ Small Ring)
			GameObject Arena2Door1 = UnityEngine.Object.Instantiate(doorPrefab);
			GameObject Arena2Door2 = UnityEngine.Object.Instantiate(doorPrefab);
			GameObject Arena2Door3 = UnityEngine.Object.Instantiate(doorPrefab);
			GameObject Arena2Door4 = UnityEngine.Object.Instantiate(doorPrefab);

			Arena2Door1.transform.position = new Vector3(-82.47f, -998.5f, 119.7f);
			Arena2Door2.transform.position = new Vector3(-82.47f, -998.5f, 148.1f);
			Arena2Door3.transform.position = new Vector3(-41.1f, -998.5f, 147.7f);
			Arena2Door4.transform.position = new Vector3(-37.7f, -998.5f, 144.3f);

			Arena2Door1.transform.localScale = new Vector3(3, 1, 1);
			Arena2Door2.transform.localScale = new Vector3(3, 1, 1);
			Arena2Door3.transform.localScale = new Vector3(3, 1, 1);
			Arena2Door4.transform.localScale = new Vector3(3, 1, 1);

			Arena2Door1.transform.rotation = Quaternion.Euler(0, 0, 0);
			Arena2Door2.transform.rotation = Quaternion.Euler(0, 0, 0);
			Arena2Door3.transform.rotation = Quaternion.Euler(0, 0, 0);
			Arena2Door4.transform.rotation = Quaternion.Euler(0, 90, 0);

			NetworkServer.Spawn(Arena2Door1);
			NetworkServer.Spawn(Arena2Door2);
			NetworkServer.Spawn(Arena2Door3);
			NetworkServer.Spawn(Arena2Door4);

			Arena2Door1.GetComponent<DoorVariant>().NetworkActiveLocks = (ushort)DoorLockType.AdminCommand;
			Arena2Door2.GetComponent<DoorVariant>().NetworkActiveLocks = (ushort)DoorLockType.AdminCommand;
			Arena2Door3.GetComponent<DoorVariant>().NetworkActiveLocks = (ushort)DoorLockType.AdminCommand;
			Arena2Door4.GetComponent<DoorVariant>().NetworkActiveLocks = (ushort)DoorLockType.AdminCommand;

			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-82, -999, 123)));
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-82, -999, 144)));
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-59, -999, 144)));
			
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-42, -999, 144)));
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-41, -999, 123)));
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-64, -999, 123)));
			
			var color1 = new Color(200f/255f,200f/255f,255f/255f);

			Map.FindParentRoom(pickups[0].Base.gameObject).Color = color1;
			Map.FindParentRoom(pickups[1].Base.gameObject).Color = color1;
			Map.FindParentRoom(pickups[2].Base.gameObject).Color = color1;

			Map.FindParentRoom(pickups[3].Base.gameObject).Color = color1;
			Map.FindParentRoom(pickups[4].Base.gameObject).Color = color1;
			Map.FindParentRoom(pickups[5].Base.gameObject).Color = color1;

			for (var index = 0; index < pickups.Count; index++)
			{
				Pickup pickup = pickups[index];
				pickup.Destroy();
			}
			pickups.Clear();

			// ------------------------------------------------ \\

			// Arena 4 Doors (EZ Large Ring)
			GameObject Arena4Door1 = UnityEngine.Object.Instantiate(doorPrefab);
			GameObject Arena4Door2 = UnityEngine.Object.Instantiate(doorPrefab);
			GameObject Arena4Door3 = UnityEngine.Object.Instantiate(doorPrefab);
			GameObject Arena4Door4 = UnityEngine.Object.Instantiate(doorPrefab);
			GameObject Arena4Door5 = UnityEngine.Object.Instantiate(doorPrefab);
			GameObject Arena4Door6 = UnityEngine.Object.Instantiate(doorPrefab);

			Arena4Door1.transform.position = new Vector3(-82f, -998.5f, 78.23f);
			Arena4Door2.transform.position = new Vector3(-41.2f, -998.5f, 79.5f);
			Arena4Door3.transform.position = new Vector3(-16.36f, -998.5f, 103.2f);
			Arena4Door4.transform.position = new Vector3(-20.7f, -998.5f, 107.2f);
			Arena4Door5.transform.position = new Vector3(-82.6f, -998.5f, 107.16f);
			Arena4Door6.transform.position = new Vector3(-86.68f, -998.5f, 103);

			Arena4Door1.transform.localScale = new Vector3(3, 1, 1);
			Arena4Door2.transform.localScale = new Vector3(3, 1, 1);
			Arena4Door3.transform.localScale = new Vector3(3, 1, 1);
			Arena4Door4.transform.localScale = new Vector3(3, 1, 1);
			Arena4Door5.transform.localScale = new Vector3(3, 1, 1);
			Arena4Door6.transform.localScale = new Vector3(3, 1, 1);

			Arena4Door1.transform.rotation = Quaternion.Euler(0, 0, 0); 
			Arena4Door2.transform.rotation = Quaternion.Euler(0, 0, 0);
			Arena4Door3.transform.rotation = Quaternion.Euler(0, 270, 0);
			Arena4Door4.transform.rotation = Quaternion.Euler(0, 0, 0); 
			Arena4Door5.transform.rotation = Quaternion.Euler(0, 0, 0);
			Arena4Door6.transform.rotation = Quaternion.Euler(0, 90, 0);

			NetworkServer.Spawn(Arena4Door1);
			NetworkServer.Spawn(Arena4Door2);
			NetworkServer.Spawn(Arena4Door3);
			NetworkServer.Spawn(Arena4Door4);
			NetworkServer.Spawn(Arena4Door5);
			NetworkServer.Spawn(Arena4Door6);

			Arena4Door1.GetComponent<DoorVariant>().NetworkActiveLocks = (ushort)DoorLockType.AdminCommand;
			Arena4Door2.GetComponent<DoorVariant>().NetworkActiveLocks = (ushort)DoorLockType.AdminCommand;
			Arena4Door3.GetComponent<DoorVariant>().NetworkActiveLocks = (ushort)DoorLockType.AdminCommand;
			Arena4Door4.GetComponent<DoorVariant>().NetworkActiveLocks = (ushort)DoorLockType.AdminCommand;
			Arena4Door5.GetComponent<DoorVariant>().NetworkActiveLocks = (ushort)DoorLockType.AdminCommand;
			Arena4Door6.GetComponent<DoorVariant>().NetworkActiveLocks = (ushort)DoorLockType.AdminCommand;

			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-82, -999, 103)));
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-61, -999, 103)));
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-40, -999, 103)));
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-21, -999, 103)));
			
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-21, -999, 82)));
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-41, -999, 82)));
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-61, -999, 82)));
			pickups.Add(Item.Create(ItemType.Adrenaline).Spawn(new Vector3(-81, -999, 82)));
			
			var color2 = new Color(200f/255f,255f/255f,200f/255f);
			
			Map.FindParentRoom(pickups[0].Base.gameObject).Color = color2;
			Map.FindParentRoom(pickups[1].Base.gameObject).Color = color2;
			Map.FindParentRoom(pickups[2].Base.gameObject).Color = color2;
			Map.FindParentRoom(pickups[3].Base.gameObject).Color = color2;
			
			Map.FindParentRoom(pickups[4].Base.gameObject).Color = color2;
			Map.FindParentRoom(pickups[5].Base.gameObject).Color = color2;
			Map.FindParentRoom(pickups[6].Base.gameObject).Color = color2;
			Map.FindParentRoom(pickups[7].Base.gameObject).Color = color2;

			for (var index = 0; index < pickups.Count; index++)
			{
				Pickup pickup = pickups[index];
				pickup.Destroy();
			}
			pickups.Clear();
			
			Round.Start();
		}
		
		public static List<Vector3> Position1 = new List<Vector3>
		{
			new Vector3(132.5f, -1014, 105.2f), // 939's chamber
			new Vector3(-82.54f, -998f, 135.1f), // EZ Small Ring
			new Vector3(192.26f, 1.6f, 171.39f), // 914
			new Vector3(-20.52f, -998f, 91.84f) // EZ Large Ring
		};

		public static List<Vector3> Position2 = new List<Vector3>
		{
			new Vector3(115, -1014, 105),
			new Vector3(-41.22f, -998f, 134.76f),
			new Vector3(192.26f, 1.6f, 156.7f), 
			new Vector3(-80.4f, -998f, 91.66f)
		};
		
		public static Dictionary<string, int> playerAndWins = new Dictionary<string, int> { };
		public static List<bool> activeMatch = new List<bool> { false , false, false, false };

		public static List<ItemType> GunList = new List<ItemType>{
			ItemType.GunCOM18,
			ItemType.GunCOM15,
			ItemType.GunFSP9,
			ItemType.GunCrossvec,
			ItemType.GunE11SR,
			ItemType.GunAK,
			ItemType.GunLogicer,
			ItemType.GunShotgun,
			ItemType.GunRevolver,
		};

		public static void AttemptToStartMatch(KingAndCompetitor kingAndCompetitor)
		{
			if (kingAndCompetitor.CanStartMatch && !activeMatch[kingAndCompetitor.GetIndex()])
			{
				StartMatch(kingAndCompetitor);
			}
		}
		
		public static bool roundPaused = false;
		
		public static IEnumerator<float> CheckStaleMate(KingAndCompetitor kc)
        {
            yield return Timing.WaitForSeconds(30);
            kc.King.Broadcast(5, "90 Seconds remaining");
            kc.Competitor.Broadcast(5, "90 Seconds remaining");
            yield return Timing.WaitForSeconds(30);
            kc.King.Broadcast(5, "60 Seconds remaining");
            kc.Competitor.Broadcast(5, "60 Seconds remaining");
            yield return Timing.WaitForSeconds(30);
            kc.King.Broadcast(5, "30 Seconds remaining");
            kc.Competitor.Broadcast(5, "30 Seconds remaining");
            yield return Timing.WaitForSeconds(30);

            kc.King.Broadcast(5, "Stalemate!");
            kc.Competitor.Broadcast(5, "Stalemate!");

            kc.Queue.Add(kc.King);
            kc.Queue.Add(kc.Competitor);

            kc.King.Role.Type = RoleType.Spectator;
            kc.Competitor.Role.Type = RoleType.Spectator;

            kc.King = null;
            kc.Competitor = null;

            kc.King = kc.Queue[0];
            kc.Queue.Remove(kc.King);

            kc.Competitor = kc.Queue[0];
            kc.Queue.Remove(kc.Competitor);

            yield return Timing.WaitForSeconds(3);
            StartMatch(kc);
        }
            
	    public static void StartMatch(KingAndCompetitor kingandcompetitor)
		{
			// ply1 is king, ply2 is competitor
			if (roundPaused)
				return;

			Player ply1 = kingandcompetitor.King;
			Player ply2 = kingandcompetitor.Competitor;

			// The index in the KingAndCompetitor, determining which match we're dealing with.
			int index = kingandcompetitor.GetIndex();

			ply1.ClearBroadcasts();
			ply2.ClearBroadcasts();

			Timing.CallDelayed(0.5f, () =>
			{
				UtilityMethods.CleanupRagdollsAndItems();
			});

			var gun = GunList.PickRandom();

			ply1.Role.Type = RoleType.ClassD;
			//ply1.ReferenceHub.PlayerCameraReference.rotation = new Quaternion(0, 0, 0, 0);
			Timing.CallDelayed(1f, () =>
			{
				
				ply1.Ammo[ItemType.Ammo9x19] = 300;
				ply1.Ammo[ItemType.Ammo556x45] = 300;
				ply1.Ammo[ItemType.Ammo762x39] = 300;
				ply1.Ammo[ItemType.Ammo12gauge] = 300;
				ply1.Ammo[ItemType.Ammo44cal] = 300;
				ply1.AddItem(gun);
				
				ply1.Position = Position1[index];
				ply1.ReferenceHub.playerEffectsController.EnableEffect<Ensnared>();
				ply1.Health = 150;
			});

			ply2.Role.Type = RoleType.Scientist;
			//ply1.ReferenceHub.PlayerCameraReference.rotation = new Quaternion(0, 0, 0, 0);
			Timing.CallDelayed(1f, () =>
			{
				ply2.ClearInventory();

				ply2.Ammo[ItemType.Ammo9x19] = 300;
				ply2.Ammo[ItemType.Ammo556x45] = 300;
				ply2.Ammo[ItemType.Ammo762x39] = 300;
				ply2.Ammo[ItemType.Ammo12gauge] = 300;
				ply2.Ammo[ItemType.Ammo44cal] = 300;
				ply2.AddItem(gun);

				ply2.Position = Position2[index];
				ply2.ReferenceHub.playerEffectsController.EnableEffect<Ensnared>();
				ply2.Health = 150;
			});
			//RespawnEffectsController.ClearQueue();
			//RespawnEffectsController.ClearQueue();
			
			ply1.PlayCassieAnnouncement("yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 pitch_0.6 .g6", false, false);
			//ply1.PlayCassieAnnouncement("yd_1 10 yd_1 9 yd_1 8 yd_1 7 yd_1 6 yd_1 5 yd_1 4 yd_1 3 yd_1 2 yd_1 1 yd_1 pitch_0.6 .g6", false, false);
			ply1.Broadcast(1, "10");
			ply1.Broadcast(1, "9");
			ply1.Broadcast(1, "8");
			ply1.Broadcast(1, "7");
			ply1.Broadcast(1, "6");
			ply1.Broadcast(1, "5");
			ply1.Broadcast(1, "4");
			ply1.Broadcast(1, "3");
			ply1.Broadcast(1, "2");
			ply1.Broadcast(1, "1");

			ply2.PlayCassieAnnouncement("yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 .g1 yd_1 pitch_0.6 .g6", false, false);
			//ply2.PlayCassieAnnouncement("yd_1 10 yd_1 9 yd_1 8 yd_1 7 yd_1 6 yd_1 5 yd_1 4 yd_1 3 yd_1 2 yd_1 1 yd_1 pitch_0.6 .g6", false, false);
			ply2.Broadcast(1, "10");
			ply2.Broadcast(1, "9");
			ply2.Broadcast(1, "8");
			ply2.Broadcast(1, "7");
			ply2.Broadcast(1, "6");
			ply2.Broadcast(1, "5");
			ply2.Broadcast(1, "4");
			ply2.Broadcast(1, "3");
			ply2.Broadcast(1, "2");
			ply2.Broadcast(1, "1");

			//Cassie.Message();

			kingandcompetitor.Unfreezer = Timing.RunCoroutine(Unfreezer(kingandcompetitor));
			activeMatch[index] = true;
		}

	    public static IEnumerator<float> Unfreezer(KingAndCompetitor kc)
	    {
		    yield return Timing.WaitForSeconds(10f);
		    kc.King.Broadcast(5, "<size=150>FIGHT!</size>");
		    kc.Competitor.Broadcast(5, "<size=150>FIGHT!</size>");

		    kc.King.ReferenceHub.playerEffectsController.DisableEffect<Ensnared>();
		    kc.Competitor.ReferenceHub.playerEffectsController.DisableEffect<Ensnared>();

		    kc.StalemateChecker = Timing.RunCoroutine(CheckStaleMate(kc));
	    }

	    public static IEnumerator<float> TimerDeathMatch()
	    {
		    yield return Timing.WaitForSeconds(3);
		    while (Round.IsStarted)
		    {
			    yield return Timing.WaitForSeconds(1);

			    foreach (Player ply in Player.List)
			    {
				    if(ply.Role.Type == RoleType.Tutorial)
				    {
					    if (KingAndCompetitor.IsInQueue(ply))
					    {
						    var kandc = KingAndCompetitor.GetAtIndex(KingAndCompetitor.FindQueueIndex(ply));
						    ply.ShowCenterDownHint($"<size=70><color=#ff96de>You are {kandc.Queue.IndexOf(ply)+1} in queue</color></size>", 1);
					    }
				    }

				    if (ply.Role.Team == Team.RIP)
				    {
					    if(KingAndCompetitor.IsKing(ply))
						    ply.ShowCenterDownHint($"<size=70><color=#ff96de>You are currently the King</color></size>\n", 1);
					    else if (KingAndCompetitor.IsCompetitor(ply))
						    ply.ShowCenterDownHint($"<size=70><color=#ff96de>You are currently the Competitor</color></size>\n", 1);
					    else
					    {
						    if (KingAndCompetitor.IsInQueue(ply))
						    {
							    var kandc = KingAndCompetitor.GetAtIndex(KingAndCompetitor.FindQueueIndex(ply));
							    ply.ShowCenterDownHint($"<size=70><color=#ff96de>You are {kandc.Queue.IndexOf(ply)+1} in queue</color></size>", 1);
						    }
						    else
						    {
							    ply.ShowCenterDownHint($"<size=70><color=#ff96de>You are not in the queue!\n\nType \".add\" in your console!</color></size>", 1);
						    }
					    }
				    }
			    }
		    }
	    }
	}
}