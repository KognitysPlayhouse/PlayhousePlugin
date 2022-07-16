using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.SCP_Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.SCP
{
    public class MedicalStudent : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Zombie Medical Student";
        public override int AbilitiesNum { get; } = 1;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            ply.CustomClassManager().CustomClass = new MedicalStudent(ply);
        }

        public MedicalStudent(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[]
            {
                new ZombieRevive(ply),
            };
            PassiveAbilities = new CoroutineHandle[]
            {
                Timing.RunCoroutine(EngageZombieHealing())
            };
            ply.CustomClassManager().AbilityIndex = 0;
            
            Ply.Health = 550;
            Ply.AddItem(ItemType.Medkit);

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(Ply, $"{SillySundayNames.MedicalStudentSundayNames.PickRandom()}\n(Custom Class)", -1, "C50000");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(Ply, "Medical Student\n(Custom Class)", -1, "C50000");
            }

            Ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=red>Medical Student</color>!\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            Ply.SendConsoleMessage("Name: Medical Student\n\nDescription: You're a Doctor under 049's wing. Heal your fellow mistakes of nature friends and bring the undead back from the dead. Careful you could be targetted as a Healer!\n\nAbility1: Undead Revive (40s Cooldown)\n\nPassive Buffs: 550HP Max, Zombie Healing Aura\nPassive Debuffs: Indicator that you're a healer\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
        
        // Heals the HP of nearby 049-2
        private IEnumerator<float> EngageZombieHealing()
        {
            yield return Timing.WaitForSeconds(2f);
            List<Player> PlayersAlreadyAffected = new List<Player>();
            while (true)
            {
                PlayersAlreadyAffected.Clear();
                if (Ply.CustomClassManager().CustomClass.Name != "Zombie Medical Student")
                    break;
                
                
                foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, Ply.Position) <= 5))
                {
                    if (ply == Ply) continue;
                    if (ply.Role.Type == RoleType.Scp0492)
                    {
                        if (!PlayersAlreadyAffected.Contains(ply))
                        {
                            UtilityMethods.ApplyHeal(ply, 30, true, Ply);
                            PlayersAlreadyAffected.Add(ply);
                        }
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}