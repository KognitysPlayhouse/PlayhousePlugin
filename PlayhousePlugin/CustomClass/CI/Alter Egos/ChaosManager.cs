using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class ChaosManager : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Chaos Manager";
        public override int AbilitiesNum { get; } = 0;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Escape()
        {
            Ply.Role.Type = RoleType.NtfCaptain;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new NTFManager(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new ChaosManager(ply);
            });
        }

        public ChaosManager(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[]{};
            ActiveAbilities = new AbilityBase[]{};
            
            ply.ClearInventory();
            ply.AddItem(ItemType.KeycardChaosInsurgency);
            ply.AddItem(ItemType.GunAK);
            ply.AddItem(ItemType.GunRevolver);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Adrenaline);
            ply.AddItem(ItemType.ArmorHeavy);
            ply.AddItem(ItemType.GrenadeFlash);
            ply.Ammo[ItemType.Ammo762x39] = 120;
            ply.Ammo[ItemType.Ammo44cal] = 42;

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.GuardManagerSundayNames.PickRandom()}\n(Custom Class)", -1, "army_green");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Chaos Manager\n(Custom Class)", -1, "army_green");
            }
            
            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=green>Chaos Manager</color>!\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: Chaos Manager\n\nDescription: Wha- How- Why did you get this? How did you pull this off????\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}