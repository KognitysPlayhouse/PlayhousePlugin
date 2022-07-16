using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class NTFMachinist : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "NTF Machinist";
        public override int AbilitiesNum { get; } = 2;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        public override void Escape()
        {
            Ply.Role.Type = RoleType.NtfPrivate;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new ChaosMachinist(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new ChaosMachinist(ply);
            });
        }
        
        public override void Dispose()
        {
            var minidispenser = ((MiniDispenserNTF)ActiveAbilities[0]).BuildingMapObject;
            var randomizerTP = ((RandomizerTPNTF)ActiveAbilities[1]).BuildingMapObject;
            
            if (minidispenser != null && minidispenser.gameObject != null)
            {
                minidispenser.Destroy();
            }
            
            if (randomizerTP != null && randomizerTP.gameObject != null)
            {
                randomizerTP.Destroy();
            }
            
            base.Dispose();
        }

        public NTFMachinist(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[]
            {
                new MiniDispenserNTF(ply),
                new RandomizerTPNTF(ply)
            };
            PassiveAbilities = new CoroutineHandle[] { };
            ply.CustomClassManager().AbilityIndex = 0;
            
            ply.ClearInventory();
            ply.AddItem(ItemType.GunShotgun);
            ply.AddItem(ItemType.GunCOM15);
            ply.AddItem(ItemType.ArmorCombat);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Painkillers);
            ply.AddItem(ItemType.KeycardNTFLieutenant);
            
            ply.Ammo[ItemType.Ammo12gauge] = 54;
            ply.Ammo[ItemType.Ammo9x19] = 60;

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.NTFMachinistSundayNames.PickRandom()}\n(Custom Class)", -1, "mint");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "NTF Machinist\n(Custom Class)", -1, "mint");
            }


            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=navy>NTF Machinist</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: NTF Machinist\n\nDescription: You still feel like you got a few loose screws and bolts. But you're blue now so like lmao kek.\n\nAbility1: Mini Dispenser\nAbility2: Relocator\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}