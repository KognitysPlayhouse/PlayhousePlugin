using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.SCP_Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.SCP
{
    public class Overdoser : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Zombie Overdoser";
        public override int AbilitiesNum { get; } = 1;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Dispose()
        {
            Ply.Scale = Vector3.one;
            base.Dispose();
        }
        
        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            ply.CustomClassManager().CustomClass = new Overdoser(ply);
        }

        public Overdoser(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[]
            {
                new DrugDose(ply),
            };
            PassiveAbilities = new[]
            {
                Timing.RunCoroutine(EngageSCPOverhealing())
            };
            ply.CustomClassManager().AbilityIndex = 0;
            
            Ply.Health = 525;
            Ply.Scale = new Vector3(0.8f, 1f, 0.8f);
            Ply.AddItem(ItemType.Adrenaline);

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(Ply, $"{SillySundayNames.OverdoserSundayNames.PickRandom()}\n(Custom Class)", -1, "C50000");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(Ply, "Overdoser\n(Custom Class)", -1, "C50000");
            }

            Ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=red>Overdoser</color>!\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            Ply.SendConsoleMessage("Name: Overdoser\n\nDescription: If people want the good stuff you have the good stuff. Use your drugs to support your entire SCP team and give yourself a dose of drugs into battle.\n\nAbility1: Drug Dose (60s Cooldown)\n\nPassive Buffs: 525HP Max, SCP AHP Healing Aura, Thin\nPassive Debuffs: Distorted vision, indicator you're a Overdoser\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
        
        // Heals the AHP of nearby SCPs
        private IEnumerator<float> EngageSCPOverhealing()
        {
            yield return Timing.WaitForSeconds(2f);
            List<Player> PlayersAlreadyAffected = new List<Player>();
            while (true)
            {
                PlayersAlreadyAffected.Clear();
                if (Ply.CustomClassManager().CustomClass.Name != "Zombie Overdoser")
                    break;
                
                foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, Ply.Position) <= 5))
                {
                    if (ply == Ply) continue;
                    if (ply.Role.Team == Team.SCP && ply.Role.Type != RoleType.Scp106)
                    {
                        if (!PlayersAlreadyAffected.Contains(ply))
                        {
                            UtilityMethods.ApplyOverheal(ply, 5, true, Ply);
                            PlayersAlreadyAffected.Add(ply);
                        }
                    }
                }
                PlayersAlreadyAffected.Clear();
                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}