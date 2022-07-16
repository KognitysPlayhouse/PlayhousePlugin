using System;
using CommandSystem;
using Exiled.API.Features;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ViewQueue : ICommand
    {
        public string Command { get; } = "viewq";
        public string[] Aliases { get; } = null;
        public string Description { get; } = "Debug command";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

            for (var index = 0; index < KingAndCompetitor.KingAndCompetitors.Count; index++)
            {
                var t = KingAndCompetitor.KingAndCompetitors[index];
                Log.Info($"King: {t.King?.Nickname} Competitor: {t.Competitor?.Nickname}");
                foreach (var e in t.Queue)
                {
                    Log.Info($"Queue Arena {index+1} {e.Nickname}");
                }
                Log.Info("\n");
                
            }

            response = $"done";
            return true;
        }
    }
}