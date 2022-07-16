using Exiled.API.Features;
using MEC;

namespace PlayhousePlugin.CustomClass
{
    public abstract class CustomClassBase
    {
        public abstract Player Ply { get; set; }
        public abstract string Name { get; }
        public abstract int AbilitiesNum { get; }
        public abstract CoroutineHandle[] PassiveAbilities { get; }
        public abstract AbilityBase[] ActiveAbilities { get; }
        public virtual void Dispose()
        {
            foreach (var passiveAbility in PassiveAbilities)
                Timing.KillCoroutines(passiveAbility);
            
            if (Ply.ReferenceHub.nicknameSync.Network_customPlayerInfoString != "")
            {
                Ply.ReferenceHub.nicknameSync.Network_customPlayerInfoString = "";
            }

            Ply.CustomClassManager().AbilityIndex = -1;
        }

        public virtual void Escape()
        {
            
        }

        public abstract void Replace(Player ply);
    }
}