using Exiled.API.Features;
using MEC;
using PlayhousePlugin.CustomClass.Abilities;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class NTFSergeant : CustomClassBase
    {
        public override Player Ply { get; set; }
        public override string Name { get; } = "NTF Sergeant";
        public override int AbilitiesNum { get; } = 0;
        public override CoroutineHandle[] PassiveAbilities { get; }
        public override AbilityBase[] ActiveAbilities { get; }
        
        public override void Escape()
        {
            Ply.Role.Type = RoleType.ChaosRifleman;
            Ply.CustomClassManager().DisposeCustomClass();
        }

        public override void Replace(Player ply)
        {
            Dispose();
            ply.Role.Type = Ply.Role.Type;
            Vector3 pos = ply.Position;
            Timing.CallDelayed(0.1f, () =>
            {
                ply.Position = pos;
            });
        }

        public NTFSergeant(Player ply)
        {
            Ply = ply;
            PassiveAbilities = new CoroutineHandle[] { };
            ActiveAbilities = new AbilityBase[] { };
            ply.CustomClassManager().AbilityIndex = 0;
            
            ply.Broadcast(10, "<size=40><b><i>You have spawned as a <color=navy>NTF Sergeant</color>\nThis is a custom class with special abilities. To learn how to use them, press your console key (~ by default) or join our Discord at discord.gg/kognity</i></b></size>");
            ply.SendConsoleMessage("Name: NTF Sergeant\n\nDescription: Direct your squad to contain anomalies and activate containment protocols.\n\nTo get the binding commands for the abilities type \".commands\"", "yellow");
        }
    }
}