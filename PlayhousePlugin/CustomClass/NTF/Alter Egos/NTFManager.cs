using CustomPlayerEffects;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class NTFManager : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "NTF Manager";
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
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new NTFManager(ply);
            });
        }

        public NTFManager(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[]{};
            ActiveAbilities = new AbilityBase[]{};
            
            ply.ClearInventory();
            ply.AddItem(ItemType.KeycardO5);
            ply.AddItem(ItemType.GunE11SR);
            ply.AddItem(ItemType.GunCrossvec);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Radio);
            ply.AddItem(ItemType.ArmorHeavy);
            ply.AddItem(ItemType.GrenadeFlash);
            
            ply.Ammo[ItemType.Ammo556x45] = 200;
            ply.Ammo[ItemType.Ammo9x19] = 200;
            
            ply.Health = 400;
            ply.MaxHealth = 400;
            ply.ReferenceHub.playerEffectsController.EnableEffect<MovementBoost>();
            ply.ChangeEffectIntensity<MovementBoost>(25);

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.GuardManagerSundayNames.PickRandom()}\n(Custom Class)", -1, "mint");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "NTF Manager\n(Custom Class)", -1, "mint");
            }
            
            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=navy>NTF Manager</color>!\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: NTF Manager\n\nDescription: You want an 05 my friend? It's yours you fucking beast getting detained twice like that MMMM good work.\n\nPassive Buffs: Spawns with 05, 300 HP, Non-damaging 1x 207 Effect\nPassive Debuffs: None\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}