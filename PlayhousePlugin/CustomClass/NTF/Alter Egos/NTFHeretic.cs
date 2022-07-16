using System.Numerics;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.Abilities;
using Vector3 = UnityEngine.Vector3;

namespace PlayhousePlugin.CustomClass
{
    public class NTFHeretic : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "NTF Heretic";
        public override int AbilitiesNum { get; } = 1;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        public override void Escape()
        {
            Ply.Role.Type = RoleType.NtfPrivate;
            Ply.CustomClassManager().DisposeCustomClass();
            Ply.CustomClassManager().CustomClass = new ChaosHeretic(Ply);
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
                ply.CustomClassManager().CustomClass = new NTFHeretic(ply);
            });
        }
        public override void Dispose()
        {
            Timing.KillCoroutines(((TonicShot)ActiveAbilities[0])._coroutineHandle);
            Ply.ChangeRunningSpeed(ServerConfigSynchronizer.Singleton.NetworkHumanSprintSpeedMultiplier);
            base.Dispose();
        }
        public NTFHeretic(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[]
            {
                new TonicShot(ply),
            };
            PassiveAbilities = new CoroutineHandle[] { };
            ply.CustomClassManager().AbilityIndex = 0;
            
            ply.ClearInventory();
            ply.AddItem(ItemType.GunE11SR);
            ply.AddItem(ItemType.GunRevolver);
            ply.AddItem(ItemType.ArmorCombat);
            ply.AddItem(ItemType.Painkillers);
            ply.AddItem(ItemType.Painkillers);
            ply.AddItem(ItemType.KeycardNTFLieutenant);
            
            ply.Ammo[ItemType.Ammo44cal] = 48;
            ply.Ammo[ItemType.Ammo556x45] = 120;

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, $"{SillySundayNames.NTFHereticSundayNames.PickRandom()}\n(Custom Class)", -1, "mint");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(ply, "NTF Heretic\n(Custom Class)", -1, "mint");
            }


            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=navy>NTF Heretic</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: NTF Heretic\n\nDescription: Gain temporary strength at a long term cost.\n\nAbility1: Tonic Shot\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}