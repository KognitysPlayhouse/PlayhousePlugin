using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class Stalk : NonCooldownAbilityBase
    {
        public override string Name { get; } = "Stalk";
        public override Player Ply { get; }

        public Stalk(Player ply)
        {
            Ply = ply;
        }

        public override string GenerateHud()
        {
            if(Ply.ReferenceHub.scp079PlayerScript.Lvl < 1)
                return $"Selected: {Name} (You need to be at least Tier 2 to use this ability)";
            
            return $"Selected: {Name} (Ready)";
        }

        public override bool UseAbility()
        {
            if (Ply.ReferenceHub.scp079PlayerScript.Lvl < 1)
            {
                Ply.ShowCenterDownHint("<color=yellow>You need to be at least Tier 2 to use this ability!</color>", 3);
                return false;
            }

            Player playerToTP = Player.List.Where(r => r.Role.Team == Team.CDP || r.Role.Team == Team.MTF || r.Role.Team == Team.RSC).ToList().PickRandom();

            if (playerToTP.CurrentRoom.Type == RoomType.Pocket)
            {
                Ply.ShowCenterDownHint("<color=yellow>This player is in the Pocket Dimension</color>", 3);
                return false;
            }

            Camera cam = Camera.List.Where(r => r.Room == playerToTP.CurrentRoom).ToList()[0];

            float cost = Ply.ReferenceHub.scp079PlayerScript.CalculateCameraSwitchCost(playerToTP.CurrentRoom.Position) / ((int)Ply.ReferenceHub.scp079PlayerScript.Lvl + 1);
            if (Ply.ReferenceHub.scp079PlayerScript.Mana >= cost)
            {
                Ply.ReferenceHub.scp079PlayerScript.RpcSwitchCamera(cam.Base.cameraId, true);
                Ply.ReferenceHub.scp079PlayerScript.Mana -= cost;
                return true;
            }

            Ply.ShowCenterDownHint($"<color=yellow>You need {Math.Round(cost - Ply.ReferenceHub.scp079PlayerScript.Mana)} more AP.</color>",3);
            return false;
        }
    }
}