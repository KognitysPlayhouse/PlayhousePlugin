using System.Collections.Generic;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class HotBullets : CooldownAbilityBase
    {
        public override string Name { get; } = "Hot Bullets";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 30;
        public bool IsActive = false;
        public int TimeElapsed = 0;
        public HotBullets(Player ply)
        {
            Ply = ply;
        }

        public override string GenerateHud()
        {
            if(!IsActive)
                return base.GenerateHud();
	        
            return $"Selected: {Name} ({10-TimeElapsed} seconds of Hot Bullet remains)";
        }
        
        public override bool UseCooldownAbility()
        {
            IsActive = true;
            Timing.RunCoroutine(Buff());
            
            return true;
        }
        
        private IEnumerator<float> Buff()
        {
            TimeElapsed = 0;
            for (var i = 0; i < 10; i++)
            {
                TimeElapsed = i;
                yield return Timing.WaitForSeconds(1);
            }
            IsActive = false;
        }
    }
}