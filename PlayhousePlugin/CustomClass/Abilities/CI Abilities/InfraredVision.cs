using CustomPlayerEffects;
using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class InfraredVision : CooldownAbilityBase
    {
        public override string Name { get; } = "Infrared Vision";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 30;

        public InfraredVision(Player ply)
        {
            Ply = ply;
        }
        public override bool UseCooldownAbility()
        {
            Ply.ShowCenterDownHint($"<color=yellow>Infrared Vision Activated</color>", 3);

            Ply.ReferenceHub.playerEffectsController.EnableEffect<Visuals939>(duration: 10);
            Ply.ReferenceHub.playerEffectsController.ChangeEffectIntensity<Visuals939>(3);
            return true;
        }
    }
}