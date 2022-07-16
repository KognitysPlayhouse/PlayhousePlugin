using CustomPlayerEffects;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class ChaosScout : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Chaos Scout";
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
            Ply.Role.Type = RoleType.NtfPrivate;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new NtfScout(Ply);
        }

        public override void Replace(Player ply)
        {
            Ply.CustomClassManager().DisposeCustomClass();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = Ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new ChaosScout(ply);
            });
        }

        public ChaosScout(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[] { };
            ActiveAbilities = new AbilityBase[] { };
            
            ply.ClearInventory();
            ply.AddItem(ItemType.GunShotgun);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Adrenaline);
            ply.AddItem(ItemType.KeycardChaosInsurgency);
            ply.AddItem(ItemType.ArmorCombat);
            
            ply.Ammo[ItemType.Ammo12gauge] = 56;

            ply.Scale = new Vector3((float)0.9, (float)0.9, (float)0.9);
            ply.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.NTFScoutSundayNames.PickRandom()}\n(Custom Class)", -1, "army_green");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Chaos Scout\n(Custom Class)", -1, "army_green");
            }

            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=green>Chaos Scout</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: Chaos Scout\n\nDescription: You're basically the NTF Scout but green! Nice job on getting detained, shoot people I guess?\n\nPassive Buffs: Non-damaging 1x 207 Effect, Smaller Size.\nPassive Debuffs: Easy to spot due to unique size\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}