using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class TeleporterExit : CooldownAbilityBase
    {
        public override string Name { get; } = "Telporter Exit";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 30;
        
        public bool IsBuilt = false;
        public const int TeleporterRadius = 2;
        public SchematicObject BuildingMapObject;
        public CoroutineHandle BuildingCoroutine;
        public Room Room;
        public Vector3 Position { get; set; }

        public TeleporterExit(Player ply)
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
                Ply.ShowCenterDownHint($"<color=yellow>Teleporter Exit comin' up!</color>", 3);
                BuildingCoroutine = Timing.RunCoroutine(BuildTeleporterExit());
                return true;
            }
            else
            {
                Ply.ShowCenterDownHint($"<color=yellow>Exit Destroyed</color>", 3);
                Timing.KillCoroutines(BuildingCoroutine);
                BuildingMapObject.Destroy();
                IsBuilt = false;
                Room = null;
                Position = Vector3.zero;
                return true;
            }
        }

        private IEnumerator<float> BuildTeleporterExit()
        {
            Vector3 buildingPosition = Ply.Position + Vector3.down*1.3f;
            Position = buildingPosition;
            
            BuildingMapObject = UtilityMethods.SpawnSchematic("TeleporterExitBlue", buildingPosition);
            Room = BuildingMapObject.CurrentRoom;
            IsBuilt = true;

            yield return Timing.WaitForSeconds(0.1f);
        }
    }
}