using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass
{
    public abstract class AbilityBase
    {
        public abstract string Name { get; }
        public abstract Player Ply { get; }
        public abstract bool Use();
        public virtual string GenerateHud()
        {
            return $"Selected: {Name} (Ready)";
        }
    }
}