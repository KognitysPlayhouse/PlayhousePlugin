using System;
using UnityEngine;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using RemoteAdmin;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ItemSpawn : ICommand
    {
        public string Command { get; } = "item";
        public string[] Aliases { get; } = { "i" };
        public string Description { get; } = "Spawn item command";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            // Only players can run this command
            if (!(sender is PlayerCommandSender player))
            {
                response = "This command can only be run by a player.";
                return false;
            }
            
            if (arguments.Count < 1)
            {
                response = "USAGE: item <itemType> <count> <scaleX,scaleY,scaleZ>/<scaleX> <scaleY> <scaleZ>";
                return false;
            }
                    
            // Evaluate ItemType enum from string
            int count = 1;
            var scale = Vector3.one;

            if (!Enum.TryParse(arguments.At(0), true, out ItemType itemType))
            {
                response = $"Invalid value for item type: {arguments.At(1)}";
                return false;
            }

            if (arguments.Count >= 2)
            {
                if (!int.TryParse(arguments.At(1), out count))
                {
                    response = $"Invalid value for item count: {arguments.At(1)}";
                    return false;
                }
            }
                        
                    
            // Evaluate a Vector3 scale from string
            if (arguments.Count >= 3)
            {
                // check if argument at index 3 has a comma
                if (arguments.At(2).Contains(","))
                {
                    var split = arguments.At(2).Split(',');

                    if (!float.TryParse(split[0], out float x))
                    {
                        response = $"Invalid value for item scale: {split[0]}";
                        return false;
                    }

                    if (!float.TryParse(split[1], out float y))
                    {
                        response = $"Invalid value for item scale: {split[1]}";
                        return false;
                    }
                            

                    if (!float.TryParse(split[2], out float z))
                    {
                        response = $"Invalid value for item scale: {split[2]}";
                        return false;
                    }
                            
                    scale = new Vector3(x, y , z);
                }
                else
                {
                    // if arguments count isn't 5 make response invalid
                    if (arguments.Count != 5)
                    {
                        response = "USAGE: item <itemType> <count> <scaleX,scaleY,scaleZ>/<scaleX> <scaleY> <scaleZ>";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(2), out float x))
                    {
                        response = $"Invalid value for item scale: {arguments.At(2)}";
                        return false;
                    }
                            
                    if (!float.TryParse(arguments.At(3), out float y))
                    {
                        response = $"Invalid value for item scale: {arguments.At(3)}";
                        return false;
                    }
                            
                    if (!float.TryParse(arguments.At(4), out float z))
                    {
                        response = $"Invalid value for item scale: {arguments.At(4)}";
                        return false;
                    }
                            
                    scale = new Vector3(x, y , z);
                }
            }

            // Spawn count number of items of type itemType with scale scale
            for (var i = 0; i < count; i++)
            {
                var item = Item.Create(itemType).Spawn(Player.Get(((PlayerCommandSender) sender).ReferenceHub).Position);
                if(scale != Vector3.one)
                    item.Scale = scale;
            }

            response = "Spawned Item(s)";
            return true;
        }
    }
}