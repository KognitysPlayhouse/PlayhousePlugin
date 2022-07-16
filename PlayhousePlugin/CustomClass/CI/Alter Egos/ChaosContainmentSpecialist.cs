using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class ChaosContainmentSpecialist : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Chaos Containment Specialist";
        public override int AbilitiesNum { get; } = 0;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Escape()
        {
            Ply.Role.Type = RoleType.NtfPrivate;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new NtfContainmentSpecialist(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new ChaosContainmentSpecialist(ply);
            });
        }

        public ChaosContainmentSpecialist(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[] { };
            ActiveAbilities = new AbilityBase[] { };
            
            ply.ClearInventory();
            ply.AddItem(ItemType.KeycardFacilityManager);
            ply.AddItem(ItemType.GunCOM18);
            ply.AddItem(ItemType.GunRevolver);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Adrenaline);
            ply.AddItem(ItemType.ArmorCombat);
            ply.AddItem(ItemType.GrenadeFlash);
            
            ply.Ammo[ItemType.Ammo9x19] = 160;
            ply.Ammo[ItemType.Ammo44cal] = 48;

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.NTFContainmentSpecialistSundayNames.PickRandom()}\n(Custom Class)", -1, "army_green");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Chaos Containment Specialist\n(Custom Class)", -1, "army_green");
            }


            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=green>Containment Specialist!</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: Chaos Containment Specialist\n\nDescription: You're like regular Containment Specialist but like green?\n\nPassive Buffs: 1.4x Damage buff with COM18, Apply burned status effect to targets when shooting with Revolver\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}