using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class ChaosBulldozer : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Chaos Bulldozer";
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
            Ply.Role.Type = RoleType.NtfPrivate;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new NTFBulldozer(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new ChaosBulldozer(ply);
            });
        }

        public ChaosBulldozer(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new[] { new MorphineShot(ply) };
            PassiveAbilities = new CoroutineHandle[] { };
            ply.CustomClassManager().AbilityIndex = 0;
            
            ply.ClearInventory();
            ply.AddItem(ItemType.GunLogicer);
            ply.AddItem(ItemType.GunAK);
            ply.AddItem(ItemType.ArmorHeavy);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.KeycardChaosInsurgency);
            Timing.CallDelayed(1.8f, () =>
            {
                ply.MaxHealth = 200;
                ply.Health = 200;
            });
            ply.Ammo[ItemType.Ammo762x39] = 200;

            ply.Scale = new Vector3((float)1.1, (float)1.1, (float)1.1);
            
            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.ChaosBulldozerSundayNames.PickRandom()}\n(Custom Class)", -1, "army_green");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Bulldozer\n(Custom Class)", -1, "army_green");
            }

            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=green>Bulldozer</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: Chaos Bulldozer\n\nDescription: Use your large health and large ammo reserve to your advantage to kill off all Foundation Personnel.\n\nAbility1: Morphine Shot (45s Cooldown)\n\nPassive Buffs: 200 Max HP\nPassive Debuffs: Larger hitbox\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}