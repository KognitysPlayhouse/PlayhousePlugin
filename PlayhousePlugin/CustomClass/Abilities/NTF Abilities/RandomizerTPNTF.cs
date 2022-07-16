using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class RandomizerTPNTF : CooldownAbilityBase
    {
        public override string Name { get; } = "Relocator";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 20;
        public bool IsBuilt = false;
        public const float TeleporterRadius = 1.6f;
        public SchematicObject BuildingMapObject;
        public CoroutineHandle BuildingCoroutine;
        public RandomizerTPNTF(Player ply)
        {
            Ply = ply;
        }
        
        public override bool UseCooldownAbility()
        {
	        if (Ply.Role.Type == RoleType.Tutorial)
		        return false;
	        
            if (!Ply.ReferenceHub.playerMovementSync.Grounded)
            {
                Ply.ShowCenterDownHint("<color=red>You are not on the ground</color>", 3);
                return false;
            }

            if (Ply.CurrentRoom.Type == RoomType.Pocket)
            {
                Ply.ShowCenterDownHint("<color=red>You can't build in the Pocket Dimension</color>", 3);
                return false;
            }
	        
            if (!IsBuilt)
            {
	            Ply.ShowCenterDownHint($"<color=yellow>Relocator goin' up!</color>", 3);
                BuildingCoroutine = Timing.RunCoroutine(Teleporter());
                return true;
            }
            else
            {
	            Ply.ShowCenterDownHint($"<color=yellow>Relocator Destroyed</color>", 3);
                Timing.KillCoroutines(BuildingCoroutine);
                BuildingMapObject.Destroy();
                IsBuilt = false;
                return true;
            }
        }

        /* Randomizer Teleporter Sequence of Order:
           1) - Get players in the radius
           2) - Assign rooms for each player
           3) - After 3 seconds of standing freeze player so he can't move and spawn a green light at the spawn teleport location
           4) - Teleport player and delete light
        */
        
        private IEnumerator<float> Teleporter()
		{
			Vector3 buildingPosition = Ply.Position + Vector3.down*1.3f;
			Dictionary<Player, TeleporterPlayers> teleporterPlayers = new Dictionary<Player, TeleporterPlayers>();
			Dictionary<Player, LightSourceToy> blueLights = new Dictionary<Player, LightSourceToy>();
			List<Player> RecentlyTeleported = new List<Player>();

			BuildingMapObject = UtilityMethods.SpawnSchematic("RandomizerMTF", buildingPosition);
			IsBuilt = true;

			while (Ply.CustomClassManager().CustomClass?.Name == "NTF Machinist")
			{
				yield return Timing.WaitForSeconds(1f);
				
				foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, buildingPosition) <= TeleporterRadius)) // 1) Get players in radius
				{
					if (ply.Role.Team != Team.MTF && ply.Role.Team != Team.RSC) continue;

					if (RecentlyTeleported.Contains(ply)) continue;
					if (teleporterPlayers.ContainsKey(ply))
					{
						var p = teleporterPlayers[ply];
						if (p.CheckedFor) continue;
						
						p.Counter += 1;
						p.CheckedFor = true;

						if (p.Counter == 3) // 3) Freeze Player and adding a green light
						{
							ply.ShowCenterDownHint($"<color=#1fadff>You will be teleported to: <color=#d1eeff>{RoomName[teleporterPlayers[ply].Room.Type]}</color></color>", 3);
							ply.EnableEffect(EffectType.Ensnared, 0);
							var SpotLight = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
							SpotLight.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
				
							SpotLight.GetComponent<LightSourceToy>().NetworkLightColor = Color.blue;
							SpotLight.GetComponent<LightSourceToy>().transform.position = p.Room.Position + Vector3.up*2;
							SpotLight.GetComponent<LightSourceToy>().LightIntensity = 2f;
							
							blueLights.Add(ply, SpotLight.GetComponent<LightSourceToy>());
						}
						
						if (p.Counter != 5) continue;

						// 4) Teleport player and delete light
						ply.DisableEffect(EffectType.Ensnared);
						var g = blueLights[ply].gameObject;
						Timing.CallDelayed(2, () =>
						{
							NetworkServer.Destroy(g);
						});
						blueLights.Remove(ply);
						
						ply.Position = p.Room.Position + Vector3.up * 2f;
						
						teleporterPlayers.Remove(ply);
						RecentlyTeleported.Add(ply);
						ply.ShowCenterDownHint($"", 1);
					}
					else
					{
						// 2) Assign random room
						if (ply.Zone == ZoneType.Surface)
						{
							List<Room> Rooms;
							if (EventHandler.random.Next(2) == 0)
							{
								// Entrance Zone
								Rooms = Room.List.Where(x =>
									(x.Type != RoomType.HczTesla && x.Type != RoomType.Pocket && x.Type != RoomType.EzShelter && x.Type != RoomType.EzCollapsedTunnel) &&
									x.Zone == ZoneType.Entrance).ToList();
							}
							else
							{
								// Heavy Zone
								Rooms = Room.List.Where(x =>
									(x.Type != RoomType.HczTesla && x.Type != RoomType.Pocket && x.Type != RoomType.EzShelter && x.Type != RoomType.EzCollapsedTunnel) &&
									x.Zone == ZoneType.HeavyContainment).ToList();
							}
							
							
							teleporterPlayers.Add(ply,
								new TeleporterPlayers(1,
									true,
									Rooms.PickRandom()));
							ply.ShowCenterDownHint($"<color=#1fadff>Teleporter charging, stand still!</color>", 3);	
						}
						else
						{
							// Whatever zone they're in
							List<Room> Rooms = Room.List.Where(x =>
								(x.Type != RoomType.HczTesla && x.Type != RoomType.Pocket && x.Type != RoomType.EzShelter && x.Type != RoomType.EzCollapsedTunnel) &&
								x.Zone == ply.Zone).ToList();;
							
							teleporterPlayers.Add(ply,
								new TeleporterPlayers(1,
									true,
									Rooms.PickRandom()));
							ply.ShowCenterDownHint($"<color=#1fadff>Teleporter charging, stand still!</color>", 3);
						}
						
					}
				}

				// Remove players who didn't stand on the teleporter for this loop
				var playersToRemove = (from t in teleporterPlayers where !t.Value.CheckedFor select t.Key).ToList();
				foreach (var p in playersToRemove)
				{
					teleporterPlayers.Remove(p);
					p.ShowCenterDownHint($"", 1);
				}

				foreach (var p in teleporterPlayers)
					p.Value.CheckedFor = false;

				RecentlyTeleported.Clear();
			}
			
			BuildingMapObject.Destroy();
			IsBuilt = false;
		}
        
        private class TeleporterPlayers
        {
	        public int Counter { get; set; }
	        public bool CheckedFor { get; set; }
	        public Room Room { get; set; }

	        public TeleporterPlayers(int counter, bool checkedFor, Room room)
	        {
		        Counter = counter;
		        CheckedFor = checkedFor;
		        Room = room;
	        }
        }

        public static Dictionary<RoomType, string> RoomName = new Dictionary<RoomType, string>
        {
	        {RoomType.Hcz049, "049 Chamber"},
	        {RoomType.Hcz079, "079's Room"},
	        {RoomType.Hcz096, "096's Room"},
	        {RoomType.Hcz106, "106's Room"},
	        {RoomType.Hcz939, "939 Chamber"},
	        {RoomType.HczArmory, "Heavy Armoury"},
	        {RoomType.HczCrossing, "Heavy 4 Way"},
	        {RoomType.HczCurve, "Heavy Curve"},
	        {RoomType.HczHid, "MicroHID Room"},
	        {RoomType.HczNuke, "Heavy Nuke"},
	        {RoomType.HczServers, "Server Room"},
	        {RoomType.HczStraight, "Heavy Hallway"},
	        {RoomType.HczTesla, "Tesla"},
	        {RoomType.HczChkpA, "Heavy Elevator A"},
	        {RoomType.HczChkpB, "Heavy Elevator B"},
	        {RoomType.HczEzCheckpoint, "Heavy Checkpoint"},
	        {RoomType.HczTCross, "Heavy T Room"},
	        {RoomType.EzCafeteria, "Entrance Cafe"},
	        {RoomType.EzConference, "Entrance Conference"},
	        {RoomType.EzCrossing, "Entrance 4 Way"},
	        {RoomType.EzCurve, "Entrance Curve"},
	        {RoomType.EzIntercom, "Intercom"},
	        {RoomType.EzPcs, "Entrance PCs"},
	        {RoomType.EzShelter, "Evac Shelter"},
	        {RoomType.EzStraight, "Entrance Hallway"},
	        {RoomType.EzVent, "Entrance Red Room"},
	        {RoomType.EzCollapsedTunnel, "Collapsed Room"},
	        {RoomType.EzDownstairsPcs, "Entrance Downstairs PCs"},
	        {RoomType.EzGateA, "Gate A"},
	        {RoomType.EzGateB, "Gate B"},
	        {RoomType.EzTCross, "Entrance T Room"},
	        {RoomType.EzUpstairsPcs, "Entrance Upstairs PCs"},
	        {RoomType.Lcz012, "012"},
	        {RoomType.Lcz173, "173's Chamber"},
	        {RoomType.Lcz914, "914"},
	        {RoomType.LczAirlock, "Airlock"},
	        {RoomType.LczArmory, "Light Armoury"},
	        {RoomType.LczCafe, "Light Cafe"},
	        {RoomType.LczCrossing, "Light 4 Way"},
	        {RoomType.LczCurve, "Light Curve"},
	        {RoomType.LczPlants, "Light Weed Room"},
	        {RoomType.LczStraight, "Light Hallway"},
	        {RoomType.LczToilets, "Light Washrooms"},
	        {RoomType.LczChkpA, "Light Elevator A"},
	        {RoomType.LczChkpB, "Light Elevator B"},
	        {RoomType.LczGlassBox, "GR-18"},
	        {RoomType.LczTCross, "Light T Room"},
	        {RoomType.LczClassDSpawn, "Class D Spawn"},
	        {RoomType.Pocket, "Pocket Dimension"},
	        {RoomType.Surface, "Surface"},
	        {RoomType.Unknown, "V O I D"},
        };
    }
}