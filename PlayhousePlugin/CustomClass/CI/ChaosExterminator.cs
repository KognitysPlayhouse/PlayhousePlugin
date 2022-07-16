using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class ChaosExterminator : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Chaos Exterminator";
        public override int AbilitiesNum { get; } = 1;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Escape()
        {
            Ply.Role.Type = RoleType.NtfPrivate;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new NTFExterminator(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new ChaosExterminator(ply);
            });
        }

        public ChaosExterminator(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[]
            {
                new GasGrenade(ply)
            };
            PassiveAbilities = new CoroutineHandle[] { };
            ply.CustomClassManager().AbilityIndex = 0;
            
            ply.ClearInventory();
            ply.AddItem(ItemType.GunAK);
            ply.AddItem(ItemType.ArmorCombat);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.Medkit);
            ply.AddItem(ItemType.KeycardChaosInsurgency);
            
            ply.Ammo[ItemType.Ammo762x39] = 120;

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.ChaosExterminatorSundayNames.PickRandom()}\n(Custom Class)", -1, "army_green");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "Exterminator\n(Custom Class)", -1, "army_green");
            }


            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=green>Exterminator</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: Chaos Exterminator\n\nDescription: Weaken enemies with your throwable gas grenades!\n\nAbility1: Gas Grenade\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}