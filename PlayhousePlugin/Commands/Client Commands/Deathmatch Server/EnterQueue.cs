using System;
using CommandSystem;
using Exiled.API.Features;
using MEC;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class EnterQueue : ICommand
	{
		public string Command { get; } = "add";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Will only work if you are not on the queue in the deathmatch server!";

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

			if (arguments.Count < 1)
			{
				response = arguments.Count == 0 ? "Here is a list of all the arenas" : "You need to provide an arena to join";
				Timing.CallDelayed(0.1f, () =>
				{
					p.SendConsoleMessage("Arenas:\n1 - 939's Chamber\n2 - EZ Small Ring\n3 - 914\n4 - EZ Large Ring\n\nExample: \".add 1\"", "yellow");
				});
				return false;
			}

			if(int.TryParse(arguments.At(0), out int result))
			{
				if (!(1 <= result && result <= 4))
				{
					response = "You must choose a valid arena number!";
					return false;
				}
				Log.Info($"U-1 {result}");
				var kcChoice = KingAndCompetitor.GetAtIndex(result-1);
				
				if (KingAndCompetitor.IsKing(p)) // If the player who left is a king
				{
					Log.Info("U1");
					var index = KingAndCompetitor.GetIndex(p);
					var kc = KingAndCompetitor.GetAtIndex(index);

					if (kc == kcChoice)
					{
						Log.Info("U2");
						response = "You are already at this arena!";
						return false;
					}

					if (kc.Competitor != null) // If there was actually a competitor
					{
						Log.Info("U3");
						if (kc.Queue.Count >= 1) // Try to replace the competitor
						{
							Log.Info("U4");
							Timing.KillCoroutines(kc.StalemateChecker);
							Timing.KillCoroutines(kc.Unfreezer);
							kc.King.Role.Type = RoleType.Spectator;
							kc.Competitor.Role.Type = RoleType.Spectator;
							
							kc.King = kc.Competitor;
							kc.Competitor = kc.Queue[0];
							kc.Queue.Remove(kc.Competitor);
							
							kc.King.ShowCenterHint("You have been set as the new King as the previous King has left the arena", 4);

							Timing.CallDelayed(0.2f, () =>
							{
								Deathmatch.StartMatch(kc);
							});
							Log.Info("H1");							
						}
						else // No competitors were able to replace the King that left.
						{
							Log.Info("U5");
							Timing.KillCoroutines(kc.StalemateChecker);
							Timing.KillCoroutines(kc.Unfreezer);
							Deathmatch.activeMatch[index] = false;
							
							kc.Competitor.Role.Type = RoleType.Spectator;
							kc.Competitor.Broadcast(5, "Your opponent has left and you have been put into spectator.");
							kc.King = kc.Competitor; // Set the competitor as the new king
							kc.Competitor = null;
							p.Role.Type = RoleType.Spectator;
							
							Log.Info("H2");
						}
					}
					else // Means there was no competitor, set everything to standby
					{
						Log.Info("U6");
						kc.King = null;
						Deathmatch.activeMatch[index] = false;
						Log.Info("H3");
					}
				}
				else if (KingAndCompetitor.IsCompetitor(p))  // If the player who left is a competitor
				{
					Log.Info("U7");
					var index = KingAndCompetitor.GetIndex(p);
					var kc = KingAndCompetitor.GetAtIndex(index);
					
					if (kc == kcChoice)
					{
						Log.Info("U8");
						response = "You are already at this arena!";
						return false;
					}

					if (kc.King != null) // If there was actually a king
					{
						Log.Info("U9");
						if (kc.Queue.Count >= 1) // Try to replace the competitor
						{
							Log.Info("U10");
							Timing.KillCoroutines(kc.StalemateChecker); // Kill the coroutine
							Timing.KillCoroutines(kc.Unfreezer);
							kc.Competitor.Role.Type = RoleType.Spectator; // Set the competitor who left to spectator
							
							// Replacing the competitor now
							kc.Competitor = kc.Queue[0];
							kc.Queue.Remove(kc.Competitor);
							
							kc.Competitor.ShowCenterHint("Your opponent has left you and has been replaced.", 4);
							
							Timing.CallDelayed(0.2f, () =>
							{
								Deathmatch.StartMatch(kc);
							});
							Log.Info("H4");
						}
						else // No competitors were able to replace the one that left.
						{
							Log.Info("U11");
							Timing.KillCoroutines(kc.StalemateChecker);
							Timing.KillCoroutines(kc.Unfreezer);
							Deathmatch.activeMatch[index] = false;
							
							kc.Competitor.Role.Type = RoleType.Spectator;
							kc.Competitor = null;
							kc.King.Role.Type = RoleType.Spectator;
							kc.King.Broadcast(5, "Your opponent has left and you have been put into spectator.");
							
							Log.Info("H5");							
						}
					}
				}
				Log.Info("U12");
				if (Deathmatch.activeMatch[result-1])
				{
					Log.Info("U13");
					KingAndCompetitor.AddToQueue(p, result-1);
					response = $"Added to queue of arena {result}!";
					return true;
				}

				if (kcChoice.King == null)
				{
					Log.Info("U14");
					KingAndCompetitor.RemoveFromQueue(p);
					kcChoice.King = p;
					response = $"Added to arena {result}!";
					return true;
				}
				
				if (kcChoice.Competitor == null)
				{
					Log.Info("U15");
					KingAndCompetitor.RemoveFromQueue(p);
					kcChoice.Competitor = p;
					Deathmatch.AttemptToStartMatch(kcChoice);
					response = $"Added to arena {result}!";
					return true;
				}
			}
			
			
			response = "That wasn't a valid arena number. Example of usage \".add 1\"";
			return false;
		}
	}
}