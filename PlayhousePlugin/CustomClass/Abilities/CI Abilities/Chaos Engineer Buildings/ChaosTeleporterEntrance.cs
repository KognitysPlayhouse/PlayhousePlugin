using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using Respawning;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class ChaosTeleporterEntrance : CooldownAbilityBase
    {
        public override string Name { get; } = "Teleporter Entrance";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 30;
        
        public bool IsBuilt = false;
        public const float TeleporterRadius = 1.6f;
        public SchematicObject BuildingMapObject;
        public CoroutineHandle BuildingCoroutine;
        private bool IsSpinning = false;

        public ChaosTeleporterEntrance(Player ply)
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
	            Ply.ShowCenterDownHint($"<color=yellow>Teleporter Entrance goin' up!</color>", 3);
                BuildingCoroutine = Timing.RunCoroutine(TeleporterEntrance());
                return true;
            }
            else
            {
	            Ply.ShowCenterDownHint($"<color=yellow>Entrance Destroyed</color>", 3);
                Timing.KillCoroutines(BuildingCoroutine);
                BuildingMapObject.Destroy();
                IsBuilt = false;
                return true;
            }
        }

        private IEnumerator<float> TeleporterEntrance()
		{
			Vector3 buildingPosition = Ply.Position + Vector3.down*1.3f;
			Dictionary<Player, TeleporterPlayers> teleporterPlayers = new Dictionary<Player, TeleporterPlayers>();
			List<Player> RecentlyTeleported = new List<Player>();

			BuildingMapObject = UtilityMethods.SpawnSchematic("TeleporterGreenStatic", buildingPosition);
			IsBuilt = true;
			IsSpinning = false;
			
			while (Ply.CustomClassManager().CustomClass?.Name == "Chaos Engineer")
			{
				yield return Timing.WaitForSeconds(1f);
				var teleporterExit = ((ChaosTeleporterExit) Ply.CustomClassManager().CustomClass.ActiveAbilities[3]);

				if (Warhead.IsInProgress ||
				    Warhead.IsDetonated ||
				    Map.IsLczDecontaminated ||
				    (ObjectivePointController.RapidSpawnWaves && ObjectivePointController.Team == SpawnableTeamType.NineTailedFox)
				)
				{
					if (Map.IsLczDecontaminated)
					{
						if (!(teleporterExit.Room is null) && teleporterExit.Room.Zone == ZoneType.LightContainment)
						{
							if (IsSpinning)
							{
								BuildingMapObject.Destroy();
								BuildingMapObject = UtilityMethods.SpawnSchematic("TeleporterGreenStatic", buildingPosition);
								IsSpinning = false;
							}
							
							continue;
						}
					}

					if (ObjectivePointController.RapidSpawnWaves && ObjectivePointController.Team == SpawnableTeamType.NineTailedFox)
					{
						if (!(teleporterExit.Room is null) && teleporterExit.Room.Zone != ZoneType.Surface)
						{
							if (IsSpinning)
							{
								BuildingMapObject.Destroy();
								BuildingMapObject = UtilityMethods.SpawnSchematic("TeleporterGreenStatic", buildingPosition);
								IsSpinning = false;
							}
							
							continue;
						}
					}

					if (Warhead.IsDetonated || Warhead.IsInProgress)
					{
						if (IsSpinning)
						{
							BuildingMapObject.Destroy();
							BuildingMapObject = UtilityMethods.SpawnSchematic("TeleporterGreenstatic", buildingPosition);
							IsSpinning = false;
						}

						continue;
					}
				}

				if (teleporterExit.IsBuilt && !IsSpinning)
				{
					// There is an exit but the teleporter isn't spinning
					BuildingMapObject.Destroy();
					BuildingMapObject = UtilityMethods.SpawnSchematic("TeleporterGreenSpinning", buildingPosition);
					IsSpinning = true;
					continue;
				}

				if (!teleporterExit.IsBuilt && IsSpinning)
				{
					// There is no exit but the teleporter is spinning
					BuildingMapObject.Destroy();
					BuildingMapObject = UtilityMethods.SpawnSchematic("TeleporterGreenStatic", buildingPosition);
					IsSpinning = false;
					continue;
				}
				
				if(!teleporterExit.IsBuilt)
					continue;
				
				foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, buildingPosition) <= TeleporterRadius))
				{
					if (ply.Role.Team != Team.CHI && ply.Role.Team != Team.CDP && !ply.IsCuffed) continue;

					if (RecentlyTeleported.Contains(ply)) continue;
					if (teleporterPlayers.ContainsKey(ply))
					{
						var p = teleporterPlayers[ply];
						if (p.CheckedFor) continue;
						
						p.Counter += 1;
						p.CheckedFor = true;
						if (p.Counter != 5) continue;

						// Teleport player
						teleporterPlayers.Remove(ply);
						ply.Position =
							((ChaosTeleporterExit) Ply.CustomClassManager().CustomClass.ActiveAbilities[3])
							.Position + Vector3.up * 1.5f;
						RecentlyTeleported.Add(ply);
						
						
					}
					else
					{
						teleporterPlayers.Add(ply, new TeleporterPlayers(1, true));
						ply.ShowCenterDownHint($"<color=#007808>Teleporter charging, stand still!\nYou will be teleported to: <color=#00d90e>{RoomName[teleporterExit.Room.Type]}</color></color>", 5);
					}
				}

				// Remove players who didn't stand on the teleporter for this loop
				var playersToRemove = (from t in teleporterPlayers where !t.Value.CheckedFor select t.Key).ToList();
				foreach (var p in playersToRemove)
					teleporterPlayers.Remove(p);

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

	        public TeleporterPlayers(int counter, bool checkedFor)
	        {
		        Counter = counter;
		        CheckedFor = checkedFor;
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