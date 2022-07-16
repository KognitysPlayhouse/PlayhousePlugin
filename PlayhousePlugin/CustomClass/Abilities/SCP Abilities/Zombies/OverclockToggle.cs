using CustomPlayerEffects;
using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass.SCP_Abilities
{
    public class OverclockToggle : NonCooldownAbilityBase
    {
        public override string Name { get; } = "Overclock Toggle";
        public override Player Ply { get; }
        public bool Enabled { get; set; } = false;
        public OverclockToggle(Player ply)
        {
            Ply = ply;
        }
        public override bool UseAbility()
        {
            if (Enabled)
            {
                // Disable ability
                Ply.ReferenceHub.playerEffectsController.DisableEffect<Hemorrhage>();
                Ply.ReferenceHub.playerEffectsController.DisableEffect<Scp207>();
                Ply.ShowCenterDownHint("<color=yellow>Overclock is now OFF</color>",2);

                Enabled = false;
                return true;
            }
            else
            {
                // Enable ability
                Ply.ReferenceHub.playerEffectsController.EnableEffect<Hemorrhage>();
                Ply.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();
                Ply.ReferenceHub.playerEffectsController.ChangeEffectIntensity<Scp207>(2);
                Ply.ShowCenterDownHint("<color=yellow>Overclock is now ON</color>",2);
                Enabled = true;
                return true;
            }
        }
    }
}