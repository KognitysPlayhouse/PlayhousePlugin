using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Features;
using MEC;
using Mirror;
using PlayerStatsSystem;
using PlayhousePlugin.CustomClass.SCP;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.SCP_Abilities
{
    public class ZombieRevive : CooldownAbilityBase
    {
	    public override string Name { get; } = "Zombie Revive";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 30;
        public ZombieRevive(Player ply)
        {
	        Ply = ply;
        }
        public override bool UseCooldownAbility()
        {
            List<Collider> colliders = Physics.OverlapSphere(Ply.Position, 3f, LayerMask.GetMask("Ragdoll")).Where(e => e.gameObject.GetComponentInParent<Ragdoll>() != null).ToList();
            
            colliders.Sort((x, y) => Vector3.Distance(x.gameObject.transform.position, Ply.Position).CompareTo(Vector3.Distance(y.gameObject.transform.position, Ply.Position)));

            if (colliders.Count == 0)
            {
                Ply.ShowCenterDownHint($"<color=yellow>There are no bodies nearby</color>",3);
            	return false;
            }

            Ragdoll doll = colliders[0].gameObject.GetComponentInParent<Ragdoll>();
            if (doll.GetRole() != RoleType.Scp0492)
            {
                Ply.ShowCenterDownHint($"<color=yellow>This body has not been cured from the pestilence.</color>",3);
            	return false;
            }

            Player Patient = Player.Get(doll.Info.OwnerHub);
            if (Patient == null)
            {
	            Ply.ShowCenterDownHint($"<color=yellow>This is an unrevivable body</color>", 3);
	            return false;
            }

            if (Patient.IsAlive)
            {
	            Ply.ShowCenterDownHint($"<color=yellow>This person is already alive</color>",3);
	            return false;
            }

            Patient.Role.Type = RoleType.Scp0492;
            SCP0492.Overclocker(Patient);
            Vector3 pos = Ply.Position;
            
            Timing.CallDelayed(0.75f, () =>
            {
	            Patient.EnableEffect<MovementBoost>(10);
	            Patient.ChangeEffectIntensity<MovementBoost>(15);
	            Patient.ReferenceHub.playerStats.GetModule<AhpStat>().ServerAddProcess(70).DecayRate = 0f;
	            Patient.Position = pos + Vector3.up*1.6f;
	            Patient.Broadcast(7, $"<size=60><b><i>You have been revived by a <color=red>Medical Student Zombie</color>!</i></b></size>");
	            Ply.ShowCenterDownHint($"<color=yellow>Revived Patient!</color>",3);
            });
                
            NetworkServer.Destroy(doll.gameObject);
            return true;
        }
    }
}