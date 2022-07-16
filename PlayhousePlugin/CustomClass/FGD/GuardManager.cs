using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class GuardManager : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Guard Manager";
        public override int AbilitiesNum { get; } = 0;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }

        public override void Escape()
        {
            Ply.Role.Type = RoleType.ChaosRifleman;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new ChaosManager(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = Ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new GuardManager(ply);
            });
        }
        
        public GuardManager(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[]{};
            ActiveAbilities = new AbilityBase[]{};
            
            ply.ClearInventory();
            ply.AddItem(ItemType.KeycardNTFOfficer);
            ply.AddItem(ItemType.GunE11SR);
            ply.AddItem(ItemType.GunCOM15);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Radio);
            ply.AddItem(ItemType.ArmorHeavy);
            ply.AddItem(ItemType.GrenadeFlash);
            ply.Ammo[ItemType.Ammo556x45] = 60;
            ply.Ammo[ItemType.Ammo9x19] = 60;
            //GuardManager.Health = 110;
            //GuardManager.MaxHealth = 110;


            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.GuardManagerSundayNames.PickRandom()}\n(Custom Class)", -1, "mint");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Guard Manager\n(Custom Class)", -1, "mint");
            }

            return; // Remove when abilities
            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=grey>Guard Manager</color>!\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: Guard Manager\n\nDescription: Coordinate your Senior Guards and use your ability to create more fighters!\n\nAbility1: Cuffed Class D Conversion\nAbility2: None\nAbility3: None\nAbility4: None\n\nPassive Buffs: None\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}