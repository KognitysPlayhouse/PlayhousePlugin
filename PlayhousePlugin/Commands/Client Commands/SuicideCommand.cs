using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using PlayerStatsSystem;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Suicide : ICommand
	{
		public string Command { get; } = "kill";
		public string[] Aliases { get; } = { "suicide" };
		public string Description { get; } = "Kills you instantly";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return false;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;
			
			if (p.IsAlive && !EventHandler.IsDeathMatchServer && !p.IsGodModeEnabled)
			{
				if (EventHandler.SillySunday)
				{
					if (p.Role.Type == RoleType.Tutorial && p.RemoteAdminAccess)
					{
						UtilityMethods.FakeExplode(p);
					}
					else
					{
						if (!p.IsSpawnProtected && p.Role.Type != RoleType.Tutorial)
						{
							UtilityMethods.FakeExplode(p);
						}
					}

					RandomDeath(p);
				}
				else
				{
					RandomDeath(p);
				}
				
				if (!p.DoNotTrack)
				{
					if (Handler.killBindsUsed.ContainsKey(p))
						Handler.killBindsUsed[p] += 1;
					else
						Handler.killBindsUsed.Add(p, 1);
				}

				response = $"{p.Nickname} bid farewell cruel world";
				return true;
			}
			else
			{
				response = "You cannot use this command";
				return false;
			}
		}

		private static void RandomDeath(Player p)
		{
			CustomReasonDamageHandler damageHandler;
			
			switch (EventHandler.random.Next(18))
			{
				case 0:
					damageHandler = new CustomReasonDamageHandler("too much cringe", float.MaxValue, "");
					break;
				case 1:
					damageHandler = new CustomReasonDamageHandler("extremely high levels of cringe", float.MaxValue, "");
					break;
				case 2:
					damageHandler = new CustomReasonDamageHandler("heart attack", float.MaxValue, "");
					break;
				case 3:
					damageHandler = new CustomReasonDamageHandler("The FitnessGram™ Pacer Test is a multistage aerobic capacity test that progressively gets more difficult as it continues. The 20 meter pacer test will begin in 30 seconds. Line up at the start. The running speed starts slowly, but gets faster each minute after you hear this signal. [beep] A single lap should be completed each time you hear this sound. [ding] Remember to run in a straight line, and run as long as possible. The second time you fail to complete a lap before the sound, your test is over. The test will begin on the word start. On your mark, get ready, start.", float.MaxValue, "");
					break;
				case 4:
					damageHandler = new CustomReasonDamageHandler("twitter user", float.MaxValue, "");
					break;
				case 5:
					damageHandler = new CustomReasonDamageHandler("told a yo mama joke", float.MaxValue, "");
					break;
				case 6:
					damageHandler = new CustomReasonDamageHandler("didn't touch grass", float.MaxValue, "");
					break;
				case 7:
					damageHandler = new CustomReasonDamageHandler("lack of responsibility and self control", float.MaxValue, "");
					break;
				case 8:
					damageHandler = new CustomReasonDamageHandler("this is so sad", float.MaxValue, "");
					break;
				case 9:
					damageHandler = new CustomReasonDamageHandler("being a corpse", float.MaxValue, "");
					break;
				case 10:
					damageHandler = new CustomReasonDamageHandler("Belle Delphine only fans supporter", float.MaxValue, "");
					break;
				case 11:
					damageHandler = new CustomReasonDamageHandler("unironically played fortnite", float.MaxValue, "");
					break;
				case 12:
					damageHandler = new CustomReasonDamageHandler("The Bite of '87", float.MaxValue, "");
					break;
				case 13:
					damageHandler = new CustomReasonDamageHandler("Went to #scp-discussion", float.MaxValue, "");
					break;
				case 14:
					damageHandler = new CustomReasonDamageHandler("Stepped on a lego", float.MaxValue, "");
					break;
				case 15:
					damageHandler = new CustomReasonDamageHandler("doesn't like cats", float.MaxValue, "");
					break;
				case 16:
					damageHandler = new CustomReasonDamageHandler("skill issue", float.MaxValue, "");
					break;
				case 17:
					damageHandler = new CustomReasonDamageHandler("ඞ amogus ඞ", float.MaxValue, "");
					break;
				
				default:
					damageHandler = new CustomReasonDamageHandler("If you see this please ping kognity with a screenshot", float.MaxValue, "");
					break;
			}
			p.Hurt(damageHandler);
			
			var d = Donator.Donators.FirstOrDefault(x => x.UserId == p.RawUserId);
			if (d == null) return;
			if (d.DonatorNum >= 2)
				damageHandler.StartVelocity = p.ReferenceHub.playerMovementSync.PlayerVelocity * 5;
		}
	}
}