using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features.Objects;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class ChaosDispenser : CooldownAbilityBase
    {
        public override string Name { get; } = "Dispenser";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 30;
        
        public bool IsBuilt = false;
        public const int DispenserRadius = 4;
        public SchematicObject BuildingMapObject;
        public CoroutineHandle DispenserCoroutine;

        public ChaosDispenser(Player ply)
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
	            Ply.ShowCenterDownHint($"<color=yellow>Dispenser goin' up!</color>", 3);
	            DispenserCoroutine = Timing.RunCoroutine(BuildDispenser());
	            return true;
            }
            else
            {
	            Ply.ShowCenterDownHint($"<color=yellow>Dispenser destroyed</color>", 3);
	            Timing.KillCoroutines(DispenserCoroutine);
	            BuildingMapObject.Destroy();
	            IsBuilt = false;
	            return true;
            }
        }
        
        private IEnumerator<float> BuildDispenser()
		{
			Vector3 buildingPosition = Ply.Position + Vector3.down*1.3f;

			BuildingMapObject = UtilityMethods.SpawnSchematic("DispenserGreen", buildingPosition,
				Quaternion.Euler(0, Mathf.Round(Ply.CameraTransform.rotation.eulerAngles.y / 90) * 90, 0));
			
			IsBuilt = true;
			
			while (Ply.CustomClassManager().CustomClass?.Name == "Chaos Engineer")
			{
				yield return Timing.WaitForSeconds(1f);
				
				foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, buildingPosition) <= DispenserRadius))
				{
					if (ply.Role.Team == Team.CHI || ply.Role.Team == Team.CDP || ply.IsCuffed)
					{
						Dispense(ply, 10, 10);
					}
				}
			}
			BuildingMapObject.Destroy();
			IsBuilt = false;
		}

        private static void Dispense(Player p, float h, int ammoCount)
        {
        	float hpGiven = 0;
            bool ammoGiven = false;
            
        	if (p.Health + h > p.MaxHealth)
        	{
        		hpGiven = p.MaxHealth - p.Health;
        		p.Health = p.MaxHealth;
        	}
        	else
        	{
        		hpGiven = h;
        		Scp330Bag.AddSimpleRegeneration(p.ReferenceHub, 2, 5);
        	}

            if (!p.IsCuffed)
            {
	            List<ItemType> typesToGive = new List<ItemType>();
	            foreach (var gun in p.Items.Where(x => x.Type.IsWeapon()))
	            {
		            Firearm firearm = (Firearm) gun;
		            if (!typesToGive.Contains(firearm.AmmoType.GetItemType()))
			            typesToGive.Add(firearm.AmmoType.GetItemType());
	            }

	            ArmourAmmo limits;
	            var armour = p.Items.FirstOrDefault(x => x.Type.IsArmor());

	            if (armour == null)
		            limits = Utils.ArmourAmmoLimits[ItemType.None];
	            else
		            limits = Utils.ArmourAmmoLimits[armour.Type];

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
            }

            if (hpGiven != 0 && ammoGiven)
        	{
        		p.ShowCenterDownHint($"<color=red>+HP & Ammo Replenished</color>");
        		return;
        	}
        	
        	if (ammoGiven)
        	{
        		p.ShowCenterDownHint($"<color=red>Ammo Replenished</color>", 1);
        		return;
        	}

            if (hpGiven == 0) return;
            p.ShowCenterDownHint($"<color=red>+HP</color>");
        }
    }
}