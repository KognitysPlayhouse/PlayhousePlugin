using System;
using CommandSystem;
using Exiled.API.Features;
using MEC;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CustomClassZombieSpawn : ICommand
	{
		public string Command { get; } = "customclass";
		public string[] Aliases { get; } = {"cc"};
		public string Description { get; } = "Command to spawn custom classes (Mostly for debug)";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count == 0 || arguments.Count == 1)
			{
				response = "This command requires 2 arguments to run PlayerID/Name and CustomClassName";
				return true;
			}
			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;
			p.ClearBroadcasts();

			var player = Player.Get(arguments.At(0));

			if (player == null)
			{
				response = $"Player not found: {arguments.At(0)}";
				return true;
			}

			switch (arguments.At(1).ToLower())
			{
				case "medic":
					CustomClass.Ntf.MakeNtfMedic(player);
					break;

				case "heavy":
					CustomClass.Ntf.MakeNtfHeavy(player);
					break;

				case "demoman":
				case "demo":
					CustomClass.Ntf.MakeNtfDemo(player);
					break;

				case "engi":
				case "engineer":
					CustomClass.Ntf.MakeNtfEngineer(player);
					break;

				case "scout":
					CustomClass.Ntf.MakeNtfScout(player);
					break;

				case "containment":
				case "containmentspecialist":
					CustomClass.Ntf.MakeNtfContainmentSpecialist(player);
					break;

				case "chaosdemoman":
				case "demolitionsexpert":
				case "chaosdemo":
					CustomClass.CI.MakeChaosDemo(player);
					break;

				case "bulldozer":
					CustomClass.CI.MakeChaosBulldozer(player);
					break;

				case "chaoshunter":
				case "hunter":
					CustomClass.CI.MakeChaosHunter(player);
					break;
				
				case "chaosmachinist":
				case "machinist":
					CustomClass.CI.MakeChaosMachinist(player);
					break;
				
				case "chaosheretic":
				case "heretic":
					CustomClass.CI.MakeChaosHeretic(player);
					break;

				case "chaosexterminator":
				case "exterminator":
					CustomClass.CI.MakeChaosPoisonCarrier(player);
					break;

				case "classdchad":
				case "chad":
					CustomClass.CDP.MakeClassDChad(player);
					break;

				case "classdjanitor":
				case "janitor":
					CustomClass.CDP.MakeClassDJanitor(player);
					break;

				case "guardmanager":
					CustomClass.FGD.MakeGuardManager(player);
					break;

				case "seniorguard":
					CustomClass.FGD.MakeSeniorGuard(player);
					break;

				case "major":
				case "majorscientist":
					CustomClass.RSC.MakeMajorScientist(player);
					break;

				case "boomer":
					CustomClass.SCP.SCP0492.BoomerZombie(player);
					break;

				case "medicalstudent":
				case "medical":
				case "zmedic":
				case "zombiemedic":
					CustomClass.SCP.SCP0492.MedicalStudentZombie(player);
					break;

				case "overclocker":
					CustomClass.SCP.SCP0492.Overclocker(player);
					break;

				case "sprinter":
					CustomClass.SCP.SCP0492.SpeedyZombie(player);
					break;

				case "overdoser":
					CustomClass.SCP.SCP0492.Overdoser(player);
					break;
			}

			response = "Spawned!";
			return true;
		}
	}
}