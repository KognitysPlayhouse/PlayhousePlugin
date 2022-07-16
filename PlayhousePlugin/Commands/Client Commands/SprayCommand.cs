using System;
using System.IO;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class SprayCommand : ICommand
	{
		public string Command { get; } = "spray";
		public string[] Aliases { get; } = new string[] { "sprays" };
		public string Description { get; } = "A donator perk that allows you to spray images onto surfaces.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "This command can only be ran by a player!";
				return true;
			}

			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			if (arguments.Count == 0)
			{
				response = "Here are the all the sprays";
				p.SendConsoleMessage(File.ReadAllText("/home/ubuntu/.config/EXILED/Configs/Sprays/SpraysText.txt"), "yellow");
				return true;
			}

			Donator.GetDonator(p, out Donator donator);
			
			switch (arguments.At(0))
            {
	            case "1":
		            if (EventHandler.PlayerSpraysFree.ContainsKey(p))
		            {
			            if (EventHandler.PlayerSpraysFree[p] >= 2 && p.RawUserId != "kognity")
			            {
				            response = "You've used your allocated sprays this round!";
				            return false;
			            }			            
		            }
		            else
			            EventHandler.PlayerSpraysFree.Add(p, 0);
		            
		            Sprays.SprayPattern(p, Sprays.ArrowUp());
		            EventHandler.PlayerSpraysFree[p] += 1;

		            response = "Amogus";
		            return true;
	            
	            case "2":
		            if (EventHandler.PlayerSpraysFree.ContainsKey(p))
		            {
			            if (EventHandler.PlayerSpraysFree[p] >= 2 && p.RawUserId != "kognity")
			            {
				            response = "You've used your allocated sprays this round!";
				            return false;
			            }			            
		            }
		            else
			            EventHandler.PlayerSpraysFree.Add(p, 0);
		            
		            Sprays.SprayPattern(p, Sprays.ArrowDown());
		            EventHandler.PlayerSpraysFree[p] += 1;

		            response = "Amogus";
		            return true;
	            
	            case "3":
		            if (EventHandler.PlayerSpraysFree.ContainsKey(p))
		            {
			            if (EventHandler.PlayerSpraysFree[p] >= 2 && p.RawUserId != "kognity")
			            {
				            response = "You've used your allocated sprays this round!";
				            return false;
			            }			            
		            }
		            else
			            EventHandler.PlayerSpraysFree.Add(p, 0);
		            
		            Sprays.SprayPattern(p, Sprays.ArrowLeft());
		            EventHandler.PlayerSpraysFree[p] += 1;

		            response = "Amogus";
		            return true;
	            
	            case "4":
		            if (EventHandler.PlayerSpraysFree.ContainsKey(p))
		            {
			            if (EventHandler.PlayerSpraysFree[p] >= 2 && p.RawUserId != "kognity")
			            {
				            response = "You've used your allocated sprays this round!";
				            return false;
			            }			            
		            }
		            else
			            EventHandler.PlayerSpraysFree.Add(p, 0);
		            
		            Sprays.SprayPattern(p, Sprays.ArrowRight());
		            EventHandler.PlayerSpraysFree[p] += 1;

		            response = "Amogus";
		            return true;
	            
	            // Donator stuff -------------------------------------------------------------------------------------------------------
	            
            	case "1a":
	                if (donator == null || donator.DonatorNum < 1)
	                {
		                response = "This is a donator restricted command!";
		                return false;
	                }

	                if (EventHandler.PlayerSprays.ContainsKey(p))
	                {
		                if (EventHandler.PlayerSprays[p] == donator.DonatorNum && p.RawUserId != "kognity")
		                {
			                response = "You've used your allocated sprays this round!";
			                return false;
		                }			            
	                }
	                else
		                EventHandler.PlayerSprays.Add(p, 0);
	                
            		Sprays.SprayPattern(p, Sprays.Amogus());
            		EventHandler.PlayerSprays[p] += 1;

            		response = "Amogus";
            		return true;

            	case "2a":
	                if (donator == null || donator.DonatorNum < 1)
	                {
		                response = "This is a donator restricted command!";
		                return false;
	                }

	                if (EventHandler.PlayerSprays.ContainsKey(p))
	                {
		                if (EventHandler.PlayerSprays[p] == donator.DonatorNum && p.RawUserId != "kognity")
		                {
			                response = "You've used your allocated sprays this round!";
			                return false;
		                }			            
	                }
	                else
		                EventHandler.PlayerSprays.Add(p, 0);
	                
            		Sprays.SprayPattern(p, Sprays.FoundationLogo());
            		EventHandler.PlayerSprays[p] += 1;

            		response = "Amogus";
            		return true;
            	
            	case "3a":
	                if (donator == null || donator.DonatorNum < 1)
	                {
		                response = "This is a donator restricted command!";
		                return false;
	                }

	                if (EventHandler.PlayerSprays.ContainsKey(p))
	                {
		                if (EventHandler.PlayerSprays[p] == donator.DonatorNum && p.RawUserId != "kognity")
		                {
			                response = "You've used your allocated sprays this round!";
			                return false;
		                }			            
	                }
	                else
		                EventHandler.PlayerSprays.Add(p, 0);
	                
            		Sprays.SprayPattern(p, Sprays.TrollFace());
            		EventHandler.PlayerSprays[p] += 1;

            		response = "Amogus";
            		return true;
            	
            	case "4a":
	                if (donator == null || donator.DonatorNum < 1)
	                {
		                response = "This is a donator restricted command!";
		                return false;
	                }

	                if (EventHandler.PlayerSprays.ContainsKey(p))
	                {
		                if (EventHandler.PlayerSprays[p] == donator.DonatorNum && p.RawUserId != "kognity")
		                {
			                response = "You've used your allocated sprays this round!";
			                return false;
		                }			            
	                }
	                else
		                EventHandler.PlayerSprays.Add(p, 0);
	                
            		Sprays.SprayPattern(p, Sprays.KLPPog());
            		EventHandler.PlayerSprays[p] += 1;

            		response = "Amogus";
            		return true;

            	case "5a":
	                if (donator == null || donator.DonatorNum < 1)
	                {
		                response = "This is a donator restricted command!";
		                return false;
	                }

	                if (EventHandler.PlayerSprays.ContainsKey(p))
	                {
		                if (EventHandler.PlayerSprays[p] == donator.DonatorNum && p.RawUserId != "kognity")
		                {
			                response = "You've used your allocated sprays this round!";
			                return false;
		                }			            
	                }
	                else
		                EventHandler.PlayerSprays.Add(p, 0);
	                
            		Sprays.SprayPattern(p, Sprays.KLPLauv2());
            		EventHandler.PlayerSprays[p] += 1;

            		response = "Amogus";
            		return true;

	            case "hubert":
            		if (p.RawUserId == "kognity")
            		{
            			Sprays.SprayPattern(p, Sprays.Hubert());

                        response = "Amogus";
            			return true;
            		}
            		response = "That is not a valid spray number! USAGE: \".spray NUMBER\"";
            		return true;

            	default:
            		response = "That is not a valid spray number! USAGE: \".spray NUMBER\"";
            		return true;
            }
		}
	}
}