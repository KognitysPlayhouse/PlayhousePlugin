using System;
using CommandSystem;
using PlayhousePlugin.CustomGameMode;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class RoundStats : ICommand
    {
        public string Command { get; } = "roundstats";
        public string[] Aliases { get; } = { "stats" };
        public string Description { get; } = "Round stats for Breakout Blitz gamemode";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if(!EventHandler.IsEventServer)
            {
                response = "This command can only be used on the event server.";
                return false;
            }
            
            response = $"Game Stats:\nRound Time: {EventHandler.Stopwatch.Elapsed.Minutes}:{EventHandler.Stopwatch.Elapsed.Seconds}\nSCP Kills: {BreakoutBlitz.SCPKills}/{BreakoutBlitz.RequiredSCPKills}\nClass D Escapes: {BreakoutBlitz.ClassDEscapes}/{BreakoutBlitz.RequiredClassDEscapes}\nScientist Escapes: {BreakoutBlitz.ScientistEscapes}/{BreakoutBlitz.RequiredScientistEscapes}\nTime till NTF/Chaos: {(600000 - EventHandler.Stopwatch.ElapsedMilliseconds < 0 ? 0 : 600000 - EventHandler.Stopwatch.ElapsedMilliseconds)/1000 :D}s";
            return true;
        }
    }
}