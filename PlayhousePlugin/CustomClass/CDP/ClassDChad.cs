using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class ClassDChad : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Class D Chad";
        public override int AbilitiesNum { get; } = 0;
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
                ply.CustomClassManager().CustomClass = new ClassDChad(ply);
            });
        }
        
        public override void Dispose()
        {
            Ply.Scale = Vector3.one;
            base.Dispose();
        }
        
        public ClassDChad(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[]{};
            ActiveAbilities = new AbilityBase[]{};
            
            ply.Health = 125;
            ply.MaxHealth = 125;
            ply.Scale = new Vector3((float)1.1, (float)1.1, (float)1.1);
            
            ply.AddItem(ItemType.KeycardJanitor);
            ply.AddItem(ItemType.Flashlight);
            ply.AddItem(ItemType.Coin);
            if (UtilityMethods.RandomChance(2))
                ply.AddItem(ItemType.Coin);


            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.ClassDChadSundayNames.PickRandom()}\n(Custom Class)", -1, "FF9966");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Chad\n(Custom Class)", -1, "FF9966");
            }

            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=orange>Class D Chad</color>!\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            //ply.Broadcast(10, "<size=60><b><i>You have spawned as a <color=orange>Class D Chad</color></i></b></size>");
            ply.SendConsoleMessage("Name: Class D Chad\n\nDescription: You're the Chad of the Class Ds use your card to get into 914 faster and help your fellow Orange Bois to Victory!\n\nPassive Buffs: 120HP\nPassive Debuffs: Larger Size\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}