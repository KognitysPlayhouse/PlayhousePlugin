using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class NtfEngineer : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "NTF Engineer";
        public override int AbilitiesNum { get; } = 4;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Escape()
        {
            Ply.Role.Type = RoleType.ChaosRifleman;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new ChaosEngineer(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new NtfEngineer(ply);
            });
        }

        public override void Dispose()
        {
            var dispenser = ((Dispenser)ActiveAbilities[0]).BuildingMapObject;
            var speedpad = ((Speedpad)ActiveAbilities[1]).BuildingMapObject;
            var tpentrance = ((TeleporterEntrance)ActiveAbilities[2]).BuildingMapObject;
            var tpexit = ((TeleporterExit)ActiveAbilities[3]).BuildingMapObject;
            
            if (dispenser != null && dispenser.gameObject != null)
                dispenser.Destroy();
            
            if (speedpad != null && speedpad.gameObject != null)
                speedpad.Destroy();
            
            if (tpentrance != null && tpentrance.gameObject != null)
                tpentrance.Destroy();
            
            if (tpexit != null && tpexit.gameObject != null)
                tpexit.Destroy();

            base.Dispose();
        }

        public NtfEngineer(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[] { };
            ActiveAbilities = new AbilityBase[]
            {
                new Dispenser(ply),
                new Speedpad(ply),
                new TeleporterEntrance(ply),
                new TeleporterExit(ply)
            };
            ply.CustomClassManager().AbilityIndex = 0;

            ply.ClearInventory();
            ply.AddItem(ItemType.KeycardNTFLieutenant);
            ply.AddItem(ItemType.GunShotgun);
            ply.AddItem(ItemType.GunCOM18);
            ply.AddItem(ItemType.Radio);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.ArmorCombat);
            
            ply.Ammo[ItemType.Ammo12gauge] = 42;
            ply.Ammo[ItemType.Ammo9x19] = 60;

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.NTFEngineerSundayNames.PickRandom()}\n(Custom Class)", -1, "mint");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Engineer\n(Custom Class)", -1, "mint");
            }

            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=navy>Engineer!</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: NTF Engineer\n\nDescription: Build dispensers, speedpads and teleporters to support your team!\n\nAbility1: Dispensers Build/Destroy Toggle\nAbility2: Speedpad Build/Destroy Toggle\nAbility3: Teleporter Entrance Build/Destroy Toggle\nAbility4: Teleporter Exit Build/Destroy Toggle\n\nPassive Buffs: None\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}