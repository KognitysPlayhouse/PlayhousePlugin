using System;
using System.Diagnostics;

namespace PlayhousePlugin.CustomClass
{
    public abstract class CooldownAbilityBase : AbilityBase
    {
        public abstract double Cooldown { get; set; }
        public Stopwatch _sw = Stopwatch.StartNew();
        public virtual bool IsCooldown()
        {
            if (_sw.Elapsed.TotalSeconds < Cooldown)
                return true;

            return false;
        }

        public override string GenerateHud()
        {
            if(Ply.IsCuffed)
                return $"Selected: {Name} (Not Ready, You are detained.)";
            
            if(IsCooldown())
                return $"Selected: {Name} ({(Math.Round(Cooldown - _sw.Elapsed.TotalSeconds) < 1 ? 1 : Math.Round(Cooldown - _sw.Elapsed.TotalSeconds))}s)";

            return $"Selected: {Name} (Ready)";
        }

        public override bool Use()
        {
            if (Ply.IsCuffed)
            {
                Ply.ShowCenterDownHint($"<color=yellow>You are detained!</color>", 2);
                return false;
            }

            if (IsCooldown())
            {
                Ply.ShowCenterDownHint($"<color=yellow>You must wait {(Math.Round(Cooldown - _sw.Elapsed.TotalSeconds) < 1 ? 1 : Math.Round(Cooldown - _sw.Elapsed.TotalSeconds))} seconds until you can use your ability again!</color>", 2);
                return false;
            }

            if (UseCooldownAbility())
            {
                _sw.Restart();
                return true;
            }
            
            return false;
        }
        
        public abstract bool UseCooldownAbility();
    }
}