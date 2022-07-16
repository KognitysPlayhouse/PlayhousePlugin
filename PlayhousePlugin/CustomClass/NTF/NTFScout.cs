using CustomPlayerEffects;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class NtfScout : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "NTF Scout";
        public override int AbilitiesNum { get; } = 0;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }

        public override void Dispose()
        {
            Ply.Scale = Vector3.one;
            base.Dispose();
        }

        public override void Escape()
        {
            Ply.Role.Type = RoleType.ChaosRifleman;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new ChaosScout(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new NtfScout(ply);
            });
        }
        
        public NtfScout(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[] { };
            ActiveAbilities = new AbilityBase[] { };
            
            ply.ClearInventory();
            ply.AddItem(ItemType.GunShotgun);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Radio);
            ply.AddItem(ItemType.KeycardNTFLieutenant);
            ply.AddItem(ItemType.ArmorCombat);
            
            ply.Ammo[ItemType.Ammo9x19] = 0;
            ply.Ammo[ItemType.Ammo556x45] = 0;
            ply.Ammo[ItemType.Ammo12gauge] = 56;

            ply.Scale = new Vector3((float)0.9, (float)0.9, (float)0.9);
            ply.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.NTFScoutSundayNames.PickRandom()}\n(Custom Class)", -1, "mint");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Scout\n(Custom Class)", -1, "mint");
            }

            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=navy>Scout</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: NTF Scout\n\nDescription: Use your speed and size to flank, distract and activate the generators! Quick and hard hitting, make your life count.\n\nPassive Buffs: Non-damaging 1x 207 Effect, Smaller Size.\nPassive Debuffs: Easy to spot due to unique size\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}