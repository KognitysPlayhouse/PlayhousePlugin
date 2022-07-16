using CustomPlayerEffects;
using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class NTFHunter : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "NTF Hunter";
        public override int AbilitiesNum { get; } = 1;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        public override void Dispose()
        {
            Ply.Scale = Vector3.one;
            base.Dispose();
        }
        
        public override void Escape()
        {
            Ply.Role.Type = RoleType.ChaosRepressor;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new ChaosHunter(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new NTFHunter(ply);
            });
        }
        
        public NTFHunter(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[] {new InfraredVision(ply)};
            PassiveAbilities = new CoroutineHandle[] { };
            ply.CustomClassManager().AbilityIndex = 0;
            
            ply.ClearInventory();
            ply.AddItem(ItemType.GunShotgun);
            ply.AddItem(ItemType.GunRevolver);
            ply.AddItem(ItemType.ArmorCombat);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Radio);
            ply.AddItem(ItemType.KeycardNTFLieutenant);

            ply.Ammo[ItemType.Ammo12gauge] = 56;
            ply.Ammo[ItemType.Ammo44cal] = 30;

            ply.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.ChaosHunterSundayNames.PickRandom()}\n(Custom Class)", -1, "mint");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Hunter\n(Custom Class)", -1, "mint");
            }

            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=navy>NTF Hunter</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: NTF Hunter\n\nDescription: You use your funny goggles to shoot people better.\n\nAbility1: Infrared Vision\n\nPassive Buffs: 1x Cola\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}