using Exiled.API.Features;
using PlayerStatsSystem;

namespace PlayhousePlugin.CustomClass.SCP_Abilities
{
    public class DrugDose : CooldownAbilityBase
    {
        public override string Name { get; } = "Overdose";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 30;
        public DrugDose(Player ply)
        {
            Ply = ply;
        }
        public override bool UseCooldownAbility()
        {
            Ply.ReferenceHub.playerStats.GetModule<AhpStat>().ServerAddProcess(75f).DecayRate = 0f;
            return true;
        }
    }
}