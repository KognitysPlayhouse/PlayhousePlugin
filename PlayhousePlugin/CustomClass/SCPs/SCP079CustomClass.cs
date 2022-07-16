using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.Abilities;

namespace PlayhousePlugin.CustomClass.SCP
{
    public class SCP079CustomClass : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "SCP-079";
        public override int AbilitiesNum { get; } = 4;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
        }

        public SCP079CustomClass(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[]
            {
                new Stalk(ply),
                new GeneratorOverride(ply),
                new WarheadJam(ply),
                new Neurotoxin(ply)
            };
            PassiveAbilities = new CoroutineHandle[] { };
            ply.CustomClassManager().AbilityIndex = 0;
            
            ply.Broadcast(10, "<size=40><b><i>You have spawned as <color=red>SCP-079</color>\nThis is a class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: SCP-079\n\nDescription: Stalk players, blackout zones, delay the nuke's detonation and trap people while poisoning them. SCP-079 is equipped with the tools needed to unleash hell to Site-79\n\nAbility1: Stalk [Tier 2+]\nAbility2: Generator Override [Tier 3+] (60s Cooldown)\nAbility3: Warhead Jam [Tier 4+] (60s Cooldown)\nAbility4: Neurotoxin [Tier 5] (20s Cooldown)\n\nPassive Buffs: None\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}