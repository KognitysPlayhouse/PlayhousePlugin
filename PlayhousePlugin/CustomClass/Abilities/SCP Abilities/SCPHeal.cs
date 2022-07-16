using System;
using Exiled.API.Features;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.SCP_Abilities
{
    public class SCPHeal : NonCooldownAbilityBase
    {
        public override string Name { get; } = "SCP Heal";
        public override Player Ply { get; }
        private float _049multiplier = 1;
        public SCPHeal(Player ply)
        {
            Ply = ply;
        }

        public override string GenerateHud()
        {
	        return $"Selected: {Name} ({_049multiplier*100}% Efficency - {100 * _049multiplier} HP to Heal next)";
        }

        public override bool UseAbility()
        {
	        if (Ply.Health <= 200)
            {
	            Ply.ShowCenterDownHint("<color=red>You don't have enough health to heal!</color>", 4);
	            return false;
            }
            				
            Physics.Raycast(new Ray(Ply.CameraTransform.position, Ply.CameraTransform.forward),
	            out RaycastHit raycastHit, 5);
            
            var patient = Player.Get(raycastHit.rigidbody.gameObject);
            if (patient == null || (patient.Role.Team != Team.SCP || patient.Role.Type == RoleType.Scp0492))
            {
	            Ply.ShowCenterDownHint("<color=red>There are no patients within range.</color>", 4);
	            return false;
            }
            
            float hpToGive;
            if (patient.Role.Type == RoleType.Scp106)
            {
	            hpToGive = 10 * _049multiplier;
	            if (patient.Health + hpToGive >= patient.MaxHealth)
	            {
		            patient.Health = patient.MaxHealth;
	            }
	            else
	            {
		            patient.Health += hpToGive;
	            }
            
	            if (_049multiplier > 0.2f)
		            _049multiplier -= 0.04f;
	            else
		            _049multiplier = 0.2f;
            }
            else
            {
	            hpToGive = 100 * _049multiplier;
	            if (patient.Health + hpToGive >= patient.MaxHealth)
	            {
		            patient.Health = patient.MaxHealth;
	            }
	            else
	            {
		            patient.Health += hpToGive;
	            }
            
	            if (_049multiplier > 0.2f)
		            _049multiplier -= 0.04f;
	            else
		            _049multiplier = 0.2f;
            }
            
            Ply.Health -= 100;
            Ply.ShowCenterDownHint($"<color=green>Healed {Math.Round(hpToGive, 1)}</color>", 4);
            return true;
        }
    }
}