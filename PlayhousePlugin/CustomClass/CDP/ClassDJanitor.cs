using Exiled.API.Features;
using InventorySystem.Items.Coin;
using MEC;
using PlayhousePlugin.CustomClass.Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class ClassDJanitor : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Class D Janitor";
        public override int AbilitiesNum { get; } = 1;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new ClassDJanitor(ply);
            });
        }
        
        public override void Dispose()
        {
            Ply.Scale = Vector3.one;
            base.Dispose();
        }

        public ClassDJanitor(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[]{};
            ActiveAbilities = new AbilityBase[]{ new JanitorCleanup(ply) };
            ply.CustomClassManager().AbilityIndex = 0;
            
            ply.Scale = new Vector3((float)0.9, (float)0.9, (float)0.9);
            ply.AddItem(ItemType.KeycardJanitor);
            ply.AddItem(ItemType.Flashlight);
            ply.AddItem(ItemType.Coin);
            if (UtilityMethods.RandomChance(2))
                ply.AddItem(ItemType.Coin);

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.ClassDJanitorSundayNames.PickRandom()}\n(Custom Class)", -1, "FF9966");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Janitor\n(Custom Class)", -1, "FF9966");
            }

            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=orange>Class D Janitor</color>!\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: Class D Janitor\n\nDescription: For some reason it is your job to clean up the dead bodies and the mess of Site-79. I'm sorry.\n\nAbility1: Body Clean Up\n\nPassive Buffs: 120HP Max, Smaller Body Size\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}