using Exiled.API.Features;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.SCP_Abilities
{
    public class Sneeze : CooldownAbilityBase
    {
        public override string Name { get; } = "Infectious Sneeze";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 30;
        public override bool UseCooldownAbility()
        {
            foreach (Player ply in Player.List)
            {
                if (Vector3.Distance(ply.Position, Ply.Position) <= 4)
                {
                    if (!ply.IsScp && ply != Ply)
                        UtilityMethods.InfectPlayer(ply);
                }
            }

            Ply.ShowCenterDownHint($"<color=yellow>ACHOO!</color>",3);
            return true;
        }

        public Sneeze(Player ply)
        {
            Ply = ply;
        }
    }
}