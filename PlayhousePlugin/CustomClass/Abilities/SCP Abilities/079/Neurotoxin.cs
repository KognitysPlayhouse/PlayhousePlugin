using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerStatsSystem;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class Neurotoxin : CooldownAbilityBase
    {
        public override string Name { get; } = "Neurotoxin";
        public override Player Ply { get; }
        public override double Cooldown { get; set; } = 20;
        public static List<Room> PoisonedRooms = new List<Room>();
        public Neurotoxin(Player ply)
        {
            Ply = ply;
        }
        
        public override string GenerateHud()
        {
	        if(Ply.ReferenceHub.scp079PlayerScript.Lvl < 4)
		        return $"Selected: {Name} (You need to be Tier 5 to use this ability)";
            
	        return $"Selected: {Name} (Ready)";
        }
        
        public override bool UseCooldownAbility()
        {
            if (Ply.ReferenceHub.scp079PlayerScript.Lvl < 4)
            {
                Ply.ShowCenterDownHint("<color=yellow>You need to be Tier 5 to use this ability!</color>", 3);
                return false;
            }

            if (!(Ply.ReferenceHub.scp079PlayerScript.Mana >= 120))
            {
                Ply.ShowCenterDownHint($"<color=yellow>You need 120 AP to use this ability.</color>", 3);
                return false;
            }

            if (Ply.CurrentRoom.Zone == ZoneType.Surface)
            {
                Ply.ShowCenterDownHint($"<color=yellow>This space is too large, poison is ineffective</color>", 3);
                return false;
            }
            
            if (PoisonedRooms.Contains(Ply.CurrentRoom))
            {
                Ply.ShowCenterDownHint($"<color=yellow>This room is already poisoned</color>", 3);
                return false;
            }
            
            Ply.ReferenceHub.scp079PlayerScript.Mana -= 120;
            Ply.ShowCenterDownHint($"<color=yellow>Poisoning Room</color>", 3);

            Timing.RunCoroutine(PoisonRoom(Ply.CurrentRoom));
            PoisonedRooms.Add(Ply.CurrentRoom);

            return true;
        }

        private IEnumerator<float> PoisonRoom(Room roomToPoison)
        {
        	List<Player> poisonedPlayers = new List<Player>();

        	for (int x = 0; x < 150; x++)
        	{
        		foreach (Player player in roomToPoison.Players)
        		{
        			if (player.Role.Team != Team.SCP && player.IsAlive)
        			{
        				if (player.Health > 1f)
        				{
        					player.Health -= 1f;
        					player.ShowCenterDownHint($"<color=yellow>You are breathing toxic gas from the air vents</color>",3);

        					if (!poisonedPlayers.Contains(player))
        					{
        						poisonedPlayers.Add(player);
        						Timing.CallDelayed(5f, () =>
        						{
        							poisonedPlayers.Remove(player);
        						});
        					}
        				}
        				else
        				{
        					poisonedPlayers.Remove(player);
        					Ply.ReferenceHub.scp079PlayerScript.RpcGainExp(ExpGainType.DirectKill, player.Role.Type);
                            player.Hurt(new CustomReasonDamageHandler("Neurotoxin Gas", float.MaxValue));
        				}
        			}
        		}

        		foreach (var player in poisonedPlayers.Where(player => player.CurrentRoom != roomToPoison))
                {
	                if (player.Health > 0.5f)
	                {
		                player.Health -= 0.5f;
		                player.ShowCenterDownHint($"<color=yellow>The toxic gas is fading away...</color>",3);
	                }
	                else
	                {
		                Ply.ReferenceHub.scp079PlayerScript.RpcGainExp(ExpGainType.DirectKill, player.Role.Type);
		                player.Hurt(new CustomReasonDamageHandler("Neurotoxin Gas", float.MaxValue));
	                }
                }

        		yield return Timing.WaitForSeconds(0.1f);
        	}
            PoisonedRooms.Remove(roomToPoison);
        }
    }
}