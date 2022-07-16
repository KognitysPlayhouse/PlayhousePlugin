using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class ChaosEngineer : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Chaos Engineer";
        public override int AbilitiesNum { get; } = 4;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Escape()
        {
            Ply.Role.Type = RoleType.NtfPrivate;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new NtfEngineer(Ply);
        }

        public override void Replace(Player ply)
        {
            Ply.CustomClassManager().DisposeCustomClass();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new ChaosEngineer(ply);
            });
        }
        
        public override void Dispose()
        {
            var chaosDispenser = ((ChaosDispenser)ActiveAbilities[0]).BuildingMapObject;
            var chaosSpeedpad = ((ChaosSpeedpad)ActiveAbilities[1]).BuildingMapObject;
            var chaosTPentrance = ((ChaosTeleporterEntrance)ActiveAbilities[2]).BuildingMapObject;
            var chaosTPexit = ((ChaosTeleporterExit)ActiveAbilities[3]).BuildingMapObject;
            
            if(chaosDispenser != null && chaosDispenser.gameObject != null)
                chaosDispenser.Destroy();
            
            if(chaosSpeedpad != null && chaosSpeedpad.gameObject != null)
                chaosSpeedpad.Destroy();
            
            if(chaosTPentrance != null && chaosTPentrance.gameObject != null)
                chaosTPentrance.Destroy();
            
            if(chaosTPexit != null && chaosTPexit.gameObject != null)
                chaosTPexit.Destroy();

            base.Dispose();
        }

        public ChaosEngineer(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[] { };
            ActiveAbilities = new AbilityBase[]
            {
                new ChaosDispenser(ply),
                new ChaosSpeedpad(ply),
                new ChaosTeleporterEntrance(ply),
                new ChaosTeleporterExit(ply)
            };
            ply.CustomClassManager().AbilityIndex = 0;

            ply.ClearInventory();
            ply.AddItem(ItemType.KeycardChaosInsurgency);
            ply.AddItem(ItemType.GunShotgun);
            ply.AddItem(ItemType.GunCOM18);
            ply.AddItem(ItemType.Adrenaline);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.ArmorCombat);
            
            ply.Ammo[ItemType.Ammo12gauge] = 42;
            ply.Ammo[ItemType.Ammo9x19] = 60;

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.NTFEngineerSundayNames.PickRandom()}\n(Custom Class)", -1, "army_green");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Chaos Engineer\n(Custom Class)", -1, "army_green");
            }

            ply.Broadcast(10, "<size=40><b><i>You have spawned as an <color=green>Engineer!</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: Chaos Engineer\n\nDescription: If you play your cards right you can fucking destroy the NTF, also please appreciate the fact we modelled the buildings green. So if you see this please go in general chat and type \"I appreciate the fact that Mokl modelled 2 sets of buildings\" thanks!\n\nAbility1: Dispensers Build/Destroy Toggle\nAbility2: Speedpad Build/Destroy Toggle\nAbility3: Teleporter Entrance Build/Destroy Toggle\nAbility4: Teleporter Exit Build/Destroy Toggle\n\nPassive Buffs: None\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}