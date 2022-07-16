using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class ChaosMachinist : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Machinist";
        public override int AbilitiesNum { get; } = 2;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        public override void Escape()
        {
            Ply.Role.Type = RoleType.NtfPrivate;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new NTFMachinist(Ply);
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
            var miniDispenser = ((MiniDispenser)ActiveAbilities[0]).BuildingMapObject;
            var randomizerTPChaos = ((RandomizerTPChaos)ActiveAbilities[1]).BuildingMapObject;
            
            if (miniDispenser != null && miniDispenser.gameObject != null)
                miniDispenser.Destroy();
            
            if (randomizerTPChaos != null && randomizerTPChaos.gameObject != null)
                randomizerTPChaos.Destroy();

            base.Dispose();
        }

        public ChaosMachinist(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[]
            {
                new MiniDispenser(ply),
                new RandomizerTPChaos(ply)
            };
            PassiveAbilities = new CoroutineHandle[] { };
            ply.CustomClassManager().AbilityIndex = 0;
            
            ply.ClearInventory();
            ply.AddItem(ItemType.GunShotgun);
            ply.AddItem(ItemType.GunCOM15);
            ply.AddItem(ItemType.ArmorCombat);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Painkillers);
            ply.AddItem(ItemType.KeycardChaosInsurgency);
            
            ply.Ammo[ItemType.Ammo12gauge] = 54;
            ply.Ammo[ItemType.Ammo9x19] = 60;

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.ChaosMachinist.PickRandom()}\n(Custom Class)", -1, "army_green");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Machinist\n(Custom Class)", -1, "army_green");
            }


            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=green>Machinist</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: Chaos Machinist\n\nDescription: You got a few loose bolts in your noggin, use that to your advantage and cause chaos...\n\nAbility1: Mini Dispenser\nAbility2: Relocator\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}