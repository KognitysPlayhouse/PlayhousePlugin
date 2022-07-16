using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class ChaosSpeedpad : CooldownAbilityBase
    {
        public override string Name { get; } = "Speedpad";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 30;
        
        public bool IsBuilt = false;
        public const int SpeedpadRadius = 3;
        public SchematicObject BuildingMapObject;
        public CoroutineHandle BuildingCoroutine;
        public static List<Player> ImmunePlayers = new List<Player>(); // Use static list for checking 207 damage in this very specififc case or just use movement boost 
        public ChaosSpeedpad(Player ply)
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
	            Ply.ShowCenterDownHint($"<color=yellow>Speedpad goin' up!</color>", 3);
                BuildingCoroutine = Timing.RunCoroutine(SpeedPad());
                return true;
            }
            else
            {
	            Ply.ShowCenterDownHint($"<color=yellow>Speedpad destroyed</color>", 3);
                Timing.KillCoroutines(BuildingCoroutine);
                BuildingMapObject.Destroy();
                IsBuilt = false;
                return true;
            }
        }
        
        public IEnumerator<float> SpeedPad()
		{
			Vector3 buildingPosition = Ply.Position + Vector3.down*1.3f;

			BuildingMapObject = UtilityMethods.SpawnSchematic("SpeedPad", buildingPosition,
				Quaternion.Euler(0, Ply.CameraTransform.rotation.eulerAngles.y + 180, 0), Vector3.one*2);
			
			IsBuilt = true;
			
			while (Ply.CustomClassManager().CustomClass?.Name == "Chaos Engineer")
			{
				yield return Timing.WaitForSeconds(1f);
				
				foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, buildingPosition) <= SpeedpadRadius))
				{
					if (ply.Role.Team == Team.CHI || ply.Role.Team == Team.CDP || ply.IsCuffed)
					{
						GiveSpeed(ply);
					}
				}
			}
			BuildingMapObject.Destroy();
			IsBuilt = false;
		}
        
        public static void GiveSpeed(Player p)
        {
	        if (p.GetEffect(EffectType.MovementBoost).Intensity == 15)
	        {
		        p.GetEffect(EffectType.MovementBoost).TimeLeft += 10 - p.GetEffect(EffectType.MovementBoost).TimeLeft;
	        }
	        else
	        {
		        p.EnableEffect(EffectType.MovementBoost, 10);
		        p.GetEffect(EffectType.MovementBoost).Intensity = 15;
		        p.ShowCenterDownHint("<color=red>Given 10s speed boost!</color>");
	        }
        }
    }
}