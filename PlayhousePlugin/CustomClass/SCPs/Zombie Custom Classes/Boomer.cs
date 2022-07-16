using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.SCP_Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.SCP
{
    public class Boomer : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Zombie Boomer";
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
            ply.CustomClassManager().CustomClass = new Boomer(ply);
        }

        public Boomer(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[]
            {
                new Sneeze(ply),
            };
            PassiveAbilities = new CoroutineHandle[] { };
            ply.CustomClassManager().AbilityIndex = 0;
            
            Ply.Health = 600;
            Ply.Scale = new Vector3(1.1f, 1.1f, 1.1f);
            Ply.AddItem(ItemType.GrenadeHE);

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(Ply, $"{SillySundayNames.BoomerSundayNames.PickRandom()}\n(Custom Class)", -1, "C50000");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(Ply, "Boomer\n(Custom Class)", -1, "C50000");
            }

            Ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=red>Boomer</color>!\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            Ply.SendConsoleMessage("Name: Boomer\n\nDescription: You're an Anti-Masker, Anti-Vaxxer and a closed minded individual! Show the world your stupidity- err I mean yourself!\n\nAbility1: Sneeze (30s Cooldown)\n\nPassive Buffs: 600HP Max, Death position creates a toxic zone for 10 seconds\nPassive Debuffs: Tall, Indicator that you're a Boomer\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}