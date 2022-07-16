using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.SCP_Abilities;

namespace PlayhousePlugin.CustomClass.SCP
{
    public class Overclocker : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Zombie Overclocker";
        public override int AbilitiesNum { get; } = 1;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            ply.CustomClassManager().CustomClass = new Overclocker(ply);
        }

        public Overclocker(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[]
            {
                new OverclockToggle(ply),
            };
            PassiveAbilities = new CoroutineHandle[] { };
            ply.CustomClassManager().AbilityIndex = 0;
            
            Ply.AddItem(ItemType.Painkillers);
            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(Ply, $"{SillySundayNames.OverclockerSundayNames.PickRandom()}\n(Custom Class)", -1, "C50000");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(Ply, "Overclocker\n(Custom Class)", -1, "C50000");
            }

            Ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=red>Overclocker</color>!\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            Ply.SendConsoleMessage("Name: Overclocker\n\nDescription: You have the ability to increase your metabolic rate by 18x more than normal, use that speed to kill your enemies.\n\nAbility1: Overclock Toggle\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}