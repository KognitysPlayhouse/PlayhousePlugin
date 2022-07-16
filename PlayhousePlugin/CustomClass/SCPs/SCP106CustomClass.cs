using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using PlayableScps.Interfaces;
using PlayerStatsSystem;
using PlayhousePlugin.CustomClass.SCP_Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.SCP
{
    public class SCP106CustomClass : CustomClassBase, IShielded
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "SCP-106";
        public override int AbilitiesNum { get; } = 1;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }

        public bool HasVanished { get; set; } = false;
        public bool RecentlyHit = false;
        
        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
        }

        public override void Dispose()
        {
            // Unregister events
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= PlayerOnEnteringPocketDimension;
            Exiled.Events.Handlers.Scp106.Teleporting -= Scp106OnTeleporting;
            Exiled.Events.Handlers.Player.TriggeringTesla -= PlayerOnTriggeringTesla;
            
            foreach(Player ply in Player.List)
            {
                ply.TargetGhostsHashSet.Clear();
            }

            base.Dispose();
        }

        private void PlayerOnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (ev.Player != Ply) return;
            if (((Vanish) Ply.CustomClassManager().CustomClass.ActiveAbilities[0]).IsVanish)
            {
                ev.IsTriggerable = false;
                ev.IsInHurtingRange = false;
                ev.IsInIdleRange = true;
            }
        }

        public SCP106CustomClass(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[] {new Vanish(ply)};
            PassiveAbilities = new CoroutineHandle[] {Timing.RunCoroutine(PassiveZombieHealingAura())};
            ply.CustomClassManager().AbilityIndex = 0;

            // Register Events related to 106
            Exiled.Events.Handlers.Player.EnteringPocketDimension += PlayerOnEnteringPocketDimension;
            Exiled.Events.Handlers.Scp106.Teleporting += Scp106OnTeleporting;
            Exiled.Events.Handlers.Player.TriggeringTesla += PlayerOnTriggeringTesla;

            // Moving his portal to his location
            Timing.CallDelayed(1f, () =>
            {
                if (Physics.Raycast(
                        new Ray(ply.ReferenceHub.scp106PlayerScript.transform.position,
                            -ply.ReferenceHub.scp106PlayerScript.transform.up),
                        out RaycastHit hit, 10, ply.ReferenceHub.scp106PlayerScript.teleportPlacementMask))
                    ply.ReferenceHub.scp106PlayerScript.SetPortalPosition(Vector3.zero, hit.point - Vector3.up);
            });

            ply.EnableEffect<MovementBoost>();
            ply.ChangeEffectIntensity<MovementBoost>(10);

            Timing.CallDelayed(0.1f, () =>
            {
                Shield.CurrentAmount = MaxShield;
                Shield.DecayRate = -20;
                Shield.Limit = MaxShield;
                Shield.SustainTime = 20;

                ply.Health = 1000;
                ply.MaxHealth = 1000;
            });
        }

        private void Scp106OnTeleporting(TeleportingEventArgs ev)
        {
            if (((Vanish) Ply.CustomClassManager().CustomClass.ActiveAbilities[0]).IsVanish)
            {
                ev.IsAllowed = false;
            }
        }

        private void PlayerOnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            ev.IsAllowed = false;

            if (RecentlyHit) return;
            
            Ply.ReferenceHub.scp106PlayerScript.TargetHitMarker(Ply.Connection, 2);
            ev.Player.ReferenceHub.playerStats.DealDamage(new ScpDamageHandler(Ply.ReferenceHub,70, PlayerStatsSystem.DeathTranslations.PocketDecay));
            ev.Player.EnableEffect<Burned>(15);
            ev.Player.EnableEffect<Concussed>(10);
            ev.Player.EnableEffect<Deafened>(15);
            ev.Player.EnableEffect<Amnesia>(6);
            RecentlyHit = true;
            
            // If the person isn't in vanish and they are still in cooldown and (current lapsed seconds is greater than 3 OR they recently vanished) then set the cooldown to 3 and restart the counter
            if (!((Vanish) Ply.CustomClassManager().CustomClass.ActiveAbilities[0]).IsVanish &&
                ((Vanish) Ply.CustomClassManager().CustomClass.ActiveAbilities[0]).IsCooldown() &&
                (((Vanish) Ply.CustomClassManager().CustomClass.ActiveAbilities[0])._sw.Elapsed.TotalSeconds > 3 ||
                 ((Vanish) Ply.CustomClassManager().CustomClass.ActiveAbilities[0]).RecentlyVanished ) )
            {
                ((Vanish) Ply.CustomClassManager().CustomClass.ActiveAbilities[0])._sw.Restart();
                ((Vanish) Ply.CustomClassManager().CustomClass.ActiveAbilities[0]).Cooldown = 3;
            }
            
            Timing.CallDelayed(2, ()=> RecentlyHit = false);
        }

        private void ApplyHealEffects(Player p, float h)
        {
            float HpGiven = 0;
            if (p.Health + h > p.MaxHealth)
            {
                HpGiven = p.MaxHealth - p.Health;
                p.Health = p.MaxHealth;
            }
            else
            {
                HpGiven = h;
                p.Health += h;
            }
        }
        private IEnumerator<float> PassiveZombieHealingAura()
        {
            List<Player> PlayersAlreadyAffected = new List<Player>();
            while (true)
            {
                PlayersAlreadyAffected.Clear();
                foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, Ply.Position) <= 7))
                {
                    if (ply == Ply) continue;
                    if (ply.Role.Type == RoleType.Scp0492)
                    {
                        if (!PlayersAlreadyAffected.Contains(ply))
                        {
                            ApplyHealEffects(ply, 7);
                            PlayersAlreadyAffected.Add(ply);
                        }
                    }
                }
                yield return Timing.WaitForSeconds(1f);
            }
        }
        
        /// <summary>
        /// How fast the shield recharges, in amount per second.
        /// Like shield amount, this is stored serverside.
        /// </summary>
        private const float ShieldRechargeRate = 0;

        /// <summary>
        /// The time in seconds to wait until shield regeneration begins.
        /// When 106 takes damage, their regen timer will reset to this.
        /// </summary>
        private const float ShieldRegenWaitTime = 7;

        /// <summary>
        /// The max shield that 106 has.
        /// </summary>
        private const float MaxShield = 1500;

        private AhpStat.AhpProcess _ahpProcess;
        
        public AhpStat.AhpProcess Shield
        {
            get
            {
                if (_ahpProcess == null)
                    _ahpProcess = Ply.ReferenceHub.playerStats.GetModule<AhpStat>().ServerAddProcess(0, 0, 0, 0.9f, 0, true);

                return _ahpProcess;
            }
        }
    }
}