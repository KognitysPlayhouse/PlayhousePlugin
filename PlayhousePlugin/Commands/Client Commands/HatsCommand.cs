using System;
using System.Collections.Generic;
using System.IO;
using CommandSystem;
using Exiled.API.Features;
using MEC;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class HatsCommand : ICommand
	{
		public string Command { get; } = "hats";
		public string[] Aliases { get; } = { "hat" };
		public string Description { get; } = "A cosmetic that allows you to have a hat that follows you around!";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return false;
			}

			if (!EventHandler.IsDevServer)
			{
				response = "This command cannot be run!";
				return false;
			}
			
			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);

			// TODO: checks about whether or not they can use the premium hats and stuff

			if (arguments.Count == 0)
			{
				response = "Here are the all the hats!";
				p.SendConsoleMessage(File.ReadAllText("/home/ubuntu/.config/EXILED/Configs/Hats/HatsTextNew.txt"), "yellow");
				return true;
			}

			if (arguments.At(0) == "0" || arguments.At(0) == "unequip" || arguments.At(0) == "remove")
			{
				UtilityMethods.CheckExistingSpawnedHatAndKill(p.UserId);
				response = "Removed hat!";
				return true;
			}
			
			if (NameRefs.ContainsKey(arguments.At(0).ToLower()))
			{
				UtilityMethods.CheckExistingSpawnedHatAndKill(p.UserId);
				Hat.SpawnHat(p, NameRefs[arguments.At(0).ToLower()]);
				response = "Spawned hat!";
				return true;
			}

			response = "That is not a valid hat number! USAGE: \".hats NUMBER\"";
			return true;
		}

		public Dictionary<string, string> NameRefs = new Dictionary<string, string>()
		{
			{"1a", "Egg"},
			{"1b", "Frog"},
			{"1c", "Halo"},
			{"1d", "Horns"},
		};
	}
}