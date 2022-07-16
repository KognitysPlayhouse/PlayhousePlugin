using System;
using CommandSystem;
using Exiled.API.Features;
using MEC;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class LeaveQueue : ICommand
	{
		public string Command { get; } = "remove";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Will only work if you are in the queue in the deathmatch server!";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return false;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (!EventHandler.IsDeathMatchServer)
			{
				response = "You are not on the deathmatch server!";
				return false;
			}

			if (KingAndCompetitor.IsKing(p)) // If the player who left is a king
			{
				var index = KingAndCompetitor.GetIndex(p);
				var kc = KingAndCompetitor.GetAtIndex(index);

				if (kc.Competitor != null) // If there was actually a competitor
				{
					if (kc.Queue.Count >= 1) // Try to replace the competitor
					{
						kc.King = kc.Competitor; // Set the competitor as the new king
						kc.Competitor = kc.Queue[0];
						kc.Queue.Remove(kc.Queue[0]);

						Timing.CallDelayed(0.2f, () =>
						{
							Deathmatch.StartMatch(kc);
						});
						Log.Info("L1");							
					}
					else // No competitors were able to replace the King that left.
					{
						Timing.KillCoroutines(kc.StalemateChecker);
						Timing.KillCoroutines(kc.Unfreezer);
						Deathmatch.activeMatch[index] = false;
						
						kc.Competitor.Role.Type = RoleType.Spectator;
						kc.Competitor.Broadcast(5, "Your opponent has left and you have been put into spectator.");
						kc.King = kc.Competitor; // Set the competitor as the new king
						kc.Competitor = null;
						
						Log.Info("L2"); 
					}
				}
				else // Means there was no competitor, set everything to standby
				{
					kc.King = null;
					Deathmatch.activeMatch[index] = false;
				}
			}
			else if (KingAndCompetitor.IsCompetitor(p))  // If the player who left is a competitor
			{
				var index = KingAndCompetitor.GetIndex(p);
				var kc = KingAndCompetitor.GetAtIndex(index);

				if (kc.King != null) // If there was actually a king
				{
					if (kc.Queue.Count >= 1) // Try to replace the competitor
					{
						Timing.KillCoroutines(kc.StalemateChecker); // Kill the 
						Timing.KillCoroutines(kc.Unfreezer);
						kc.Competitor.Role.Type = RoleType.Spectator; // Set the competitor who left to spectator
						
						// Replacing the competitor now
						kc.Competitor = kc.Queue[0];
						kc.Queue.Remove(kc.Queue[0]);

						Timing.CallDelayed(0.2f, () =>
						{
							Deathmatch.StartMatch(kc);
						});
						Log.Info("L3");
					}
					else // No competitors were able to replace the one that left.
					{
						Timing.KillCoroutines(kc.StalemateChecker);
						Timing.KillCoroutines(kc.Unfreezer);
						Deathmatch.activeMatch[index] = false;
						
						kc.Competitor = null;
						kc.King.Role.Type = RoleType.Spectator;
						kc.King.Broadcast(5, "Your opponent has left and you have been put into spectator.");
						
						Log.Info("L4");							
					}
				}
			}
			
			KingAndCompetitor.RemoveFromQueue(p);
			p.Role.Type = RoleType.Spectator; 
			response = "Left queue!";
			return true;
		}
	}
}