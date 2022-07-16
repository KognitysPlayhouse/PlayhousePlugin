using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.SCP_Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.SCP
{
    public class SCP049CustomClass : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "SCP-049";
        public override int AbilitiesNum { get; } = 1;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
        }

        public SCP049CustomClass(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[]
            {
                new SCPHeal(ply),
            };
            PassiveAbilities = new CoroutineHandle[] { Timing.RunCoroutine(PassiveZombieHealingAura())};
            ply.CustomClassManager().AbilityIndex = 0;

            ply.Broadcast(10, "<size=40><b><i>You have spawned as <color=red>SCP-049</color>\nThis is a class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: SCP-049\n\nDescription: SCP-049 with his medical background allows him to create zombies with supportive abilities and heal other SCPs directly. A true power class if played correctly.\n\nAbility1: SCP Healing (Degrades in effectiveness overtime)\n\nPassive Buffs: Heals zombies and Zombies heal you\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
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
                            ApplyHealEffects(Ply, 5);
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