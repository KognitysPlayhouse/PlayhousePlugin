using System;
using CommandSystem;
using Exiled.API.Features;
using MEC;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SundayEventsCommand : ICommand
    {
        public string Command { get; } = "sundayevents";
        public string[] Aliases { get; } = {"se", "sevents"};
        public string Description { get; } = "Command to control silly sunday events";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender))
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
            var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

            if (!EventHandler.SillySunday)
            {
                response = "Silly Sunday is not active";
                return false;
            }

            switch (arguments.At(0))
            {
                case "nerfwar1":
                    Timing.RunCoroutine(SillySundayEventHandler.NerfWar1());
                    response = "Starting Nerf War 1";
                    return true;

                case "nerfwar2":
                    Timing.RunCoroutine(SillySundayEventHandler.NerfWar2());
                    response = "Starting Nerf War 2";
                    return true;

                case "nerfwar3":
                    Timing.RunCoroutine(SillySundayEventHandler.NerfWar3());
                    response = "Starting Nerf War 3";
                    return true;

                case "nerfwar4":
                    Timing.RunCoroutine(SillySundayEventHandler.NerfWar4());
                    response = "Starting Nerf War 4";
                    return true;

                case "05rescue":
                    SillySundayEventHandler.ohfiverescuemode = true;
                    Timing.RunCoroutine(SillySundayEventHandler.OhFiveRescue());
                    response = "Starting 05 rescue!";
                    return true;

                case "instantrevive":
                    SillySundayEventHandler.instantRevive = true;
                    Timing.RunCoroutine(SillySundayEventHandler.InstantRevive());
                    response = "Starting Instant Revive!";
                    return true;

                case "randomrevive":
                    SillySundayEventHandler.randomrevive = true;
                    Timing.RunCoroutine(SillySundayEventHandler.RandomRevive());
                    response = "Starting Random Revive!";
                    return true;

                case "sugarrush":
                    SillySundayEventHandler.sugarrush = true;
                    Timing.RunCoroutine(SillySundayEventHandler.SugarRush());
                    response = "Starting Sugar Rush!";
                    return true;

                case "slaughterhouse":
                    SillySundayEventHandler.slaughterhouse = true;
                    Timing.RunCoroutine(SillySundayEventHandler.SlaughterHouse());
                    response = "Starting Slaugherhouse!";
                    return true;
                
                case "173":
                case "173infection":
                    if (SillySundayInfectionController.InfectionEnabled &&
                        SillySundayInfectionController.InfectedRole == RoleType.Scp173)
                    {
                        SillySundayInfectionController.InfectedRole = RoleType.None;
                        SillySundayInfectionController.InfectionEnabled = false;
                        response = "Disabled 173 infection";
                        return true;
                    }
                    else
                    {
                        SillySundayInfectionController.InfectedRole = RoleType.Scp173;
                        SillySundayInfectionController.InfectionEnabled = true;
                        response = "Enabled 173 infection";
                        return true;
                    }

                case "096":
                case "096infection":
                    if (SillySundayInfectionController.InfectionEnabled &&
                        SillySundayInfectionController.InfectedRole == RoleType.Scp096)
                    {
                        SillySundayInfectionController.InfectedRole = RoleType.None;
                        SillySundayInfectionController.InfectionEnabled = false;
                        response = "Disabled 096 infection";
                        return true;
                    }
                    else
                    {
                        SillySundayInfectionController.InfectedRole = RoleType.Scp096;
                        SillySundayInfectionController.InfectionEnabled = true;
                        response = "Enabled 096 infection";
                        return true;
                    }
                
                case "049":
                case "049infection":
                    if (SillySundayInfectionController.InfectionEnabled &&
                        SillySundayInfectionController.InfectedRole == RoleType.Scp049)
                    {
                        SillySundayInfectionController.InfectedRole = RoleType.None;
                        SillySundayInfectionController.InfectionEnabled = false;
                        response = "Disabled 049 infection";
                        return true;
                    }
                    else
                    {
                        SillySundayInfectionController.InfectedRole = RoleType.Scp049;
                        SillySundayInfectionController.InfectionEnabled = true;
                        response = "Enabled 049 infection";
                        return true;
                    }
                
                case "939":
                case "939infection":
                    if (SillySundayInfectionController.InfectionEnabled &&
                        SillySundayInfectionController.InfectedRole == RoleType.Scp93989)
                    {
                        SillySundayInfectionController.InfectedRole = RoleType.None;
                        SillySundayInfectionController.InfectionEnabled = false;
                        response = "Disabled 939 infection";
                        return true;
                    }
                    else
                    {
                        SillySundayInfectionController.InfectedRole = RoleType.Scp93989;
                        SillySundayInfectionController.InfectionEnabled = true;
                        response = "Enabled 939 infection";
                        return true;
                    }
                
                case "106":
                case "106infection":
                    if (SillySundayInfectionController.InfectionEnabled &&
                        SillySundayInfectionController.InfectedRole == RoleType.Scp106)
                    {
                        SillySundayInfectionController.InfectedRole = RoleType.None;
                        SillySundayInfectionController.InfectionEnabled = false;
                        response = "Disabled 106 infection";
                        return true;
                    }
                    else
                    {
                        SillySundayInfectionController.InfectedRole = RoleType.Scp106;
                        SillySundayInfectionController.InfectionEnabled = true;
                        response = "Enabled 106 infection";
                        return true;
                    }
                
                default:
                    response = "No event found with that name";
                    return false;
            }
        }
    }
}