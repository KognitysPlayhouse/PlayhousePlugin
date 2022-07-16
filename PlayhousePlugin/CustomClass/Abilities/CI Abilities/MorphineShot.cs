using CustomPlayerEffects;
using Exiled.API.Features;
using InventorySystem.Items.Usables.Scp330;
using MEC;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class MorphineShot : CooldownAbilityBase
    {
        public override string Name { get; } = "Morphine Shot";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 45;

        public MorphineShot(Player ply)
        {
            Ply = ply;
        }
        
        public override bool UseCooldownAbility()
        {
            Ply.ShowCenterDownHint($"<color=yellow>Injecting Morphine</color>", 5);

            Ply.CurrentItem = null;
            EventHandler.Stunned.Add(Ply);
            Ply.ReferenceHub.playerEffectsController.EnableEffect<Ensnared>(5);
            Ply.ReferenceHub.playerEffectsController.EnableEffect<Amnesia>(5);
            Ply.ReferenceHub.playerEffectsController.EnableEffect<Concussed>(2);
            Timing.CallDelayed(2, () =>
            {
                Ply.ReferenceHub.playerEffectsController.EnableEffect<Blinded>(duration: 3);
            });
							
            Scp330Bag.AddSimpleRegeneration(Ply.ReferenceHub, 20, 10f);
							
            Timing.CallDelayed(5, () =>
            {
                Ply.ReferenceHub.playerEffectsController.EnableEffect<Invigorated>(duration: 10);
                EventHandler.Stunned.Remove(Ply);
            });
            
            return true;
        }
    }
}