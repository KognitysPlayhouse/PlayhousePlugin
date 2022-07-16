using Exiled.API.Features;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class WarheadJam : CooldownAbilityBase
    {
        public override string Name { get; } = "Warhead Jam";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 60;

        public WarheadJam(Player ply)
        {
            Ply = ply;
        }
        
        public override string GenerateHud()
        {
	        if(Ply.ReferenceHub.scp079PlayerScript.Lvl < 3)
		        return $"Selected: {Name} (You need to be at least Tier 4 to use this ability)";
            
	        return $"Selected: {Name} (Ready)";
        }
        
        public override bool UseCooldownAbility()
        {
           if (Warhead.DetonationTimer <= 10)
           {
	           Ply.ShowCenterDownHint($"<color=yellow>Detonation is inevitable...</color>", 3);
	           return false;
           }

           if (Ply.ReferenceHub.scp079PlayerScript.Lvl < 3)
           {
	           Ply.ShowCenterDownHint("<color=yellow>You need to be at least Tier 4 to use this ability!</color>", 3);
	           return false;
           }

           if (!(Ply.ReferenceHub.scp079PlayerScript.Mana >= 90))
           {
	           Ply.ShowCenterDownHint($"<color=yellow>You need 90 AP to use this ability.</color>", 3);
	           return false;
           }

           if (!Warhead.IsInProgress)
           {
	           Ply.ShowCenterDownHint($"<color=yellow>The Warhead is not active</color>", 3);
	           return false;
           }

           Ply.ReferenceHub.scp079PlayerScript.Mana -= 90;
           Ply.ShowCenterDownHint($"<color=yellow>Added 15 seconds to nuke!</color>", 3);
           Warhead.DetonationTimer += 15;
           Map.Broadcast(6, "<i>079 has delayed the warhead detonation timer!</i>");

           return true;
        }
    }
}