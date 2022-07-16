using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.SCP
{
    public class SCP93953CustomClass : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "SCP-939-53";
        public override int AbilitiesNum { get; } = 0;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
        }

        public SCP93953CustomClass(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[] { };
            PassiveAbilities = new CoroutineHandle[] { Timing.RunCoroutine(PassiveZombieHealingAura())};
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
    }
}