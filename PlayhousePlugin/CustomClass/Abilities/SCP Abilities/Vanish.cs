using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.SCP;
using Player = Exiled.API.Features.Player;

namespace PlayhousePlugin.CustomClass.SCP_Abilities
{
    public class Vanish : CooldownAbilityBase
    {
        public override string Name { get; } = "Vanish";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 30;
        public bool IsVanish = false;
        public bool RecentlyVanished = false;
        public override bool UseCooldownAbility()
        {
            if (!IsVanish && !Ply.ReferenceHub.scp106PlayerScript.goingViaThePortal)
            {
                Timing.RunCoroutine(VanishAbility());
                return true;
            }

            return false;
        }

        public override string GenerateHud()
        {
            if (!IsVanish)
            {
                if (IsCooldown())
                {
                    return $"Selected: {Name} ({(Math.Round(Cooldown - _sw.Elapsed.TotalSeconds) < 1 ? 1 : Math.Round(Cooldown - _sw.Elapsed.TotalSeconds))}s)";
                }

                return $"Selected: {Name} (Ready)";

            }

            if(Cooldown - _sw.Elapsed.TotalSeconds <= 0)
                return $"Selected: {Name} (Exiting Vanish...)";
                
            return $"Selected: {Name} ({Math.Round(Cooldown - _sw.Elapsed.TotalSeconds)} Seconds of {Name} Remaining)";

        }

        private IEnumerator<float> VanishAbility()
        {
            if (IsVanish)
                yield break;
            
            IsVanish = true;
            ((SCP106CustomClass) Ply.CustomClassManager().CustomClass).HasVanished = true;
            Cooldown = 15;

            // Sink into the ground
            Ply.ReferenceHub.scp106PlayerScript.RpcTeleportAnimation();
            
            yield return Timing.WaitForSeconds(2.5f);

            // Ghosting code
            foreach(Player ply in Player.List)
            {
                if (ply.IsAlive && !ply.IsScp)
                {
                    if (ply.Health <= 40 || ply.GetEffectActive<Amnesia>() || ply.GetEffectActive<Deafened>() ||
                        ply.GetEffectActive<Concussed>() || ply.GetEffectActive<Burned>() ||
                        ply.GetEffectActive<Poisoned>() || ply.GetEffectActive<Hemorrhage>())
                    {
                    }
                    else
                    {
                        ply.TargetGhostsHashSet.Add(Ply.Id);
                        Ply.TargetGhostsHashSet.Add(ply.Id);
                    }
                }
                else
                {
                    if (!ply.IsAlive)
                    {
                        Ply.TargetGhostsHashSet.Add(ply.Id);
                    }
                }
            }
            
            Ply.IsGodModeEnabled = false;
            yield return Timing.WaitForSeconds(2.5f);

            // Vanish state
            Ply.ChangeEffectIntensity<MovementBoost>(50);
            Ply.ChangeEffectIntensity<Scp207>(4);
            ((SCP106CustomClass) Ply.CustomClassManager().CustomClass).Shield.DecayRate = -50;
            yield return Timing.WaitForSeconds(12f);
            
            // Exiting Vanish
            ((SCP106CustomClass) Ply.CustomClassManager().CustomClass).Shield.DecayRate = 0;

            // Sinking into ground
            Ply.ReferenceHub.scp106PlayerScript.RpcTeleportAnimation();
            yield return Timing.WaitForSeconds(2.5f);
            Ply.IsGodModeEnabled = true;
            
            // Clear people from ghost
            foreach(Player ply in Player.List)
            {
                ply.TargetGhostsHashSet.Clear();
            }

            yield return Timing.WaitForSeconds(2.5f);
            Ply.IsGodModeEnabled = false;
            
            Ply.ChangeEffectIntensity<MovementBoost>(15);
            Ply.ChangeEffectIntensity<Scp207>(0);
            IsVanish = false;
            RecentlyVanished = true;
            Cooldown = 30;
            _sw.Restart();
            
            yield return Timing.WaitForSeconds(3f);
            Ply.ChangeEffectIntensity<MovementBoost>(10);
            yield return Timing.WaitForSeconds(1f);
            RecentlyVanished = false;
        }
        
        public Vanish(Player ply)
        {
            Ply = ply;
        }
    }
}