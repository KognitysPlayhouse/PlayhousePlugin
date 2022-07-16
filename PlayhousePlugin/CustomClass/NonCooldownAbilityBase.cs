using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass
{
    public abstract class NonCooldownAbilityBase : AbilityBase
    {
        public override string Name { get; }
        public override Player Ply { get; }
        public override string GenerateHud()
        {
            if(Ply.IsCuffed)
                return $"Selected: {Name} (Not Ready, You are detained.)";
            
            return $"Selected: {Name} (Ready)";
        }

        public override bool Use()
        {
            if (Ply.IsCuffed)
            {
                Ply.ShowCenterDownHint($"<color=yellow>You are detained!</color>", 2);
                return false;
            }

            if (UseAbility())
            {
                return true;
            }

            return false;
        }
        
        public abstract bool UseAbility();
    }
}