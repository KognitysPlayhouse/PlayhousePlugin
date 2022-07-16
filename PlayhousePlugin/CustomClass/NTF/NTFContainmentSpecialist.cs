using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class NtfContainmentSpecialist : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "NTF Containment Specialist";
        public override int AbilitiesNum { get; } = 0;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Escape()
        {
            Ply.Role.Type = RoleType.ChaosRifleman;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new ChaosContainmentSpecialist(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new NtfContainmentSpecialist(ply);
            });
        }

        public NtfContainmentSpecialist(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[] { };
            ActiveAbilities = new AbilityBase[] { };
            
            ply.ClearInventory();
            ply.AddItem(ItemType.KeycardFacilityManager);
            ply.AddItem(ItemType.GunCOM18);
            ply.AddItem(ItemType.GunRevolver);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Radio);
            ply.AddItem(ItemType.ArmorCombat);
            ply.AddItem(ItemType.GrenadeFlash);
            
            ply.Ammo[ItemType.Ammo9x19] = 160;
            ply.Ammo[ItemType.Ammo44cal] = 48;

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.NTFContainmentSpecialistSundayNames.PickRandom()}\n(Custom Class)", -1, "mint");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Containment Specialist\n(Custom Class)", -1, "mint");
            }


            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=navy>Containment Specialist!</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: NTF Containment Specialist\n\nDescription: Use your high access card and high damage sidearms to support your team!\n\nPassive Buffs: 1.4x Damage buff with COM18, Apply burned status effect to targets when shooting with Revolver\nPassive Debuffs: No Armory Access\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}