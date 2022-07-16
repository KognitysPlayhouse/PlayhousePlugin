using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Permissions.Extensions;
using MEC;
using Mirror;
using RemoteAdmin;
using UnityEngine;

namespace PlayhousePlugin.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Template : ICommand
	{
		public string Command { get; } = "template";
		public string[] Aliases { get; } = null;
		public string Description { get; } = "template";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			var p = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
			var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

			response = "";
			return true;
		}
	}
}