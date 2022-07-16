using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using MEC;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class SCPSwap : ICommand
	{
		public string Command { get; } = "scpswap";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "Swap roles with another SCP in the round";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (!Round.IsStarted)
			{
				response = "<color=red>The round has not started yet</color>";
				return true;
			}

			if (p.Role.Team != Team.SCP)
			{
				response = "<color=red>You're not an SCP, why did you think that would work?</color>";
				return true;
			}


			if (!EventHandler.allowSwaps)
			{
				response = "<color=red>SCP swap period has expired.</color>";
				return true;
			}

			switch (arguments.Count)
			{
				case 1:
					switch (arguments.At(0).ToLower())
					{
						case "yes":
							Player swap = Handler.OngoingReqs.FirstOrDefault(x => x.Value == p).Key;
							if (swap != null)
							{
								PerformSwap(swap, p);
								Timing.KillCoroutines(Handler.ReqCoroutines[swap]);
								Handler.ReqCoroutines.Remove(swap);

								response = "<color=green>Swap successful!</color>";
								return true;
							}

							response = "<color=red>You do not have a swap request.</color>";
							return true;

						case "no":
							swap = Handler.OngoingReqs.FirstOrDefault(x => x.Value == p).Key;
							if (swap != null)
							{
								swap.ReferenceHub.characterClassManager.TargetConsolePrint(swap.ReferenceHub.scp079PlayerScript.connectionToClient, "Your swap request has been denied.", "red");
								Timing.KillCoroutines(Handler.ReqCoroutines[swap]);
								Handler.ReqCoroutines.Remove(swap);
								Handler.OngoingReqs.Remove(swap);


								response = "<color=red>Swap request denied.</color>";
								return true;
							}
							response = "<color=red>You do not have a swap request.</color>";
							return true;

						case "cancel":
							if (Handler.OngoingReqs.ContainsKey(p))
							{
								Player dest = Handler.OngoingReqs[p];
								dest.ReferenceHub.characterClassManager.TargetConsolePrint(dest.ReferenceHub.scp079PlayerScript.connectionToClient, "Your swap request has been cancelled.", "red");
								Timing.KillCoroutines(Handler.ReqCoroutines[p]);
								Handler.ReqCoroutines.Remove(p);
								Handler.OngoingReqs.Remove(p);

								response = "<color=yellow>You have cancelled your swap request</color>";
								return true;
							}
							response = "<color=red>You do not have a swap request.</color>";
							return true;

						default:
							if (p.Role.Type == RoleType.Scp0492)
							{
								response = "<color=red>Stop crying and doing cheeky shit, play the fucking video game.</color>";
								return true;
							}

							if (!valid.ContainsKey(arguments.At(0)))
							{
								response = "<color=red>Invalid SCP</color>";
								return true;
							}

							if (Handler.OngoingReqs.ContainsKey(p))
							{
								response = "<color=red>You already have a request pending.</color>";
								return true;
							}

							RoleType role = valid[arguments.At(0)];
							if(role == RoleType.Scp0492)
							{
								response = "<color=red>That SCP is blacklisted</color>";
								return true;
							}


							swap = Player.List.FirstOrDefault(x => role == RoleType.Scp93953 ? x.Role.Type == role || x.Role.Type == RoleType.Scp93989 : x.Role.Type == role);

							if (swap != null)
							{
								Handler.ReqCoroutines.Add(p, Timing.RunCoroutine(SendRequest(p, swap)));
								response = "<color=green>Swap request sent!</color>";
								return true;
							}
							response = "<color=red>No Players found to swap with</color>";
							return true;
					}

				default:
					response = "<color=red>USAGE: SCPSWAP [SCP NUMBER]</color>";
					return true;
			}
		}

		private Dictionary<string, RoleType> valid = new Dictionary<string, RoleType>()
		{
			{"173", RoleType.Scp173},
			{"peanut", RoleType.Scp173},
			{"939", RoleType.Scp93953},
			{"dog", RoleType.Scp93953},
			{"079", RoleType.Scp079},
			{"computer", RoleType.Scp079},
			{"106", RoleType.Scp106},
			{"larry", RoleType.Scp106},
			{"096", RoleType.Scp096},
			{"shyguy", RoleType.Scp096},
			{"049", RoleType.Scp049},
			{"doctor", RoleType.Scp049},
		};

		private IEnumerator<float> SendRequest(Player source, Player dest)
		{
			PlayhousePlugin.PlayhousePluginRef.Handler.OngoingReqs.Add(source, dest);
			dest.Broadcast(5, "<i>You have an SCP Swap request!\nCheck your console by pressing [`] or [~]</i>");
			dest.ReferenceHub.characterClassManager.TargetConsolePrint(dest.ReferenceHub.scp079PlayerScript.connectionToClient, $"You have received a swap request from {source.ReferenceHub.nicknameSync.Network_myNickSync} who is SCP-{valid.FirstOrDefault(x => x.Value == source.Role.Type).Key}. Would you like to swap with them? Type \".scpswap yes\" to accept or \".scpswap no\" to decline.", "yellow");
			yield return Timing.WaitForSeconds(120);
			TimeoutRequest(source);
		}

		private void TimeoutRequest(Player source)
		{
			if (PlayhousePlugin.PlayhousePluginRef.Handler.OngoingReqs.ContainsKey(source))
			{
				Player dest = PlayhousePlugin.PlayhousePluginRef.Handler.OngoingReqs[source];
				source.ReferenceHub.characterClassManager.TargetConsolePrint(source.ReferenceHub.scp079PlayerScript.connectionToClient, "The player did not respond to your request.", "red");
				dest.ReferenceHub.characterClassManager.TargetConsolePrint(dest.ReferenceHub.scp079PlayerScript.connectionToClient, "Your swap request has timed out.", "red");
				PlayhousePlugin.PlayhousePluginRef.Handler.OngoingReqs.Remove(source);
			}
		}

		private void PerformSwap(Player source, Player dest)
		{
			if(source.Role.Type == RoleType.Spectator || dest.Role.Type == RoleType.Spectator)
			{
				source.ReferenceHub.characterClassManager.TargetConsolePrint(source.ReferenceHub.scp079PlayerScript.connectionToClient, "Swap Cancelled, spectator detected", "red");
				dest.ReferenceHub.characterClassManager.TargetConsolePrint(source.ReferenceHub.scp079PlayerScript.connectionToClient, "Swap Cancelled, spectator detected", "red");

				PlayhousePlugin.PlayhousePluginRef.Handler.OngoingReqs.Remove(source);
				return;
			}
			source.ReferenceHub.characterClassManager.TargetConsolePrint(source.ReferenceHub.scp079PlayerScript.connectionToClient, "Swap successful!", "green");

			RoleType sRole = source.Role.Type;
			RoleType dRole = dest.Role.Type;

			float sHealth = source.Health;
			float dHealth = dest.Health;

			source.Role.Type = dRole;
			dest.Role.Type = sRole;

			Timing.CallDelayed(0.5f, () =>
			{ 				
				source.Health = dHealth;
				dest.Health = sHealth;
			});

			PlayhousePlugin.PlayhousePluginRef.Handler.OngoingReqs.Remove(source);
		}
	}
}