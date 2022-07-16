using CustomPlayerEffects;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.SCP
{
    public class Sprinter : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "Zombie Sprinter";
        public override int AbilitiesNum { get; } = 0;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Dispose()
        {
            Ply.Scale = Vector3.one;
            base.Dispose();
        }
        
        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            ply.CustomClassManager().CustomClass = new Sprinter(ply);
        }

        public Sprinter(Player ply)
        {
            Ply = ply;
            ActiveAbilities = new AbilityBase[] { };
            PassiveAbilities = new CoroutineHandle[] { };
            
            Ply.Health = 275;
            Ply.MaxHealth = 275;
            Ply.Scale = new Vector3(0.8f, 0.8f, 0.8f);
            Ply.AddItem(ItemType.SCP207);

            Ply.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();
            Ply.ReferenceHub.playerEffectsController.ChangeEffectIntensity<Scp207>(2);

            if (EventHandler.SillySunday)
            {
                UtilityMethods.GiveCustomPlayerInfo(Ply, $"{SillySundayNames.SpeedySundayNames.PickRandom()}\n(Custom Class)", -1, "C50000");
            }
            else
            {
                UtilityMethods.GiveCustomPlayerInfo(Ply, "Sprinter\n(Custom Class)", -1, "C50000");
            }

            Ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=red>Sprinter</color>!\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            Ply.SendConsoleMessage("Name: Sprinter\n\nDescription: You're small, you're fast and by extension you're deadly! Use your speed and size to spread the infection as fast as possible!\n\nPassive Buffs: Small, 2x 207\nPassive Debuffs: 20 HP Damage per attack, 275 HP Max, Easy to spot\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}