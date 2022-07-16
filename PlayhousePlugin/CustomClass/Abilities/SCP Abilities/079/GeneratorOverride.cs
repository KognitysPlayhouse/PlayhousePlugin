using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class GeneratorOverride : CooldownAbilityBase
    {
        public override string Name { get; } = "Generator Override";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 60;

        public GeneratorOverride(Player ply)
        {
            Ply = ply;
        }
        
        public override string GenerateHud()
        {
            if(Ply.ReferenceHub.scp079PlayerScript.Lvl < 2)
                return $"Selected: {Name} (You need to be at least Tier 3 to use this ability)";
            
            return $"Selected: {Name} (Ready)";
        }
        
        public override bool UseCooldownAbility()
        {
            if (Ply.ReferenceHub.scp079PlayerScript.Lvl < 2)
            {
                Ply.ShowCenterDownHint("<color=yellow>You need to be at least Tier 3 to use this ability!</color>", 3);
                return false;
            }

            if (!(Ply.ReferenceHub.scp079PlayerScript.Mana >= 70))
            {
                Ply.ShowCenterDownHint($"<color=yellow>You need 70 AP to use this ability.</color>", 3);
                return false;
            }

            if (Ply.CurrentRoom.Zone == ZoneType.Surface)
            {
                Ply.ShowCenterDownHint($"<color=yellow>You cannot use this ability on surface.</color>", 3);
                return false;
            }
            
            Extensions.BlackoutZone(Ply.CurrentRoom.Zone, 30);
            Timing.RunCoroutine(Drainpower(30));
            Ply.ShowCenterDownHint($"<color=yellow>Blacked out zone!</color>", 3);
            
            switch (Ply.CurrentRoom.Zone)
            {
                case ZoneType.Entrance:
                    Cassie.Message(
                        $"SCP 0 7 9 power override detected at Entrance zone . . backup generators engaged . standby",
                        isNoisy: false);
                    break;
                case ZoneType.HeavyContainment:
                    Cassie.Message(
                        $"SCP 0 7 9 power override detected at Heavy Containment Zone . . backup generators engaged . standby",
                        isNoisy: false);
                    break;
                case ZoneType.LightContainment:
                    Cassie.Message(
                        $"SCP 0 7 9 power override detected at Light containment zone . . backup generators engaged . standby",
                        isNoisy: false);
                    break;
            }

            return true;
        }
        
        private IEnumerator<float> Drainpower(int secondsToDrain)
        {
            for (var i = 0; i < secondsToDrain * 60; i++)
            {
                Ply.ReferenceHub.scp079PlayerScript.Mana = 10;
                yield return Timing.WaitForOneFrame;
            }
        }
    }
}