using System;
using Exiled.API.Features;
using System.Collections.Generic;
using Exiled.API.Enums;
using System.Linq;
using Interactables.Interobjects.DoorUtils;
using Hints;
using PlayhousePlugin.Components;
using PlayhousePlugin.CustomClass;
using UnityEngine;

namespace PlayhousePlugin
{
	public static class Extensions
	{
		/// <summary>
		/// Returns true if item is found otherwise returns false.
		/// </summary>
		/// <param name="TargetItem">The item to search for.</param>
		/// <returns>bool</returns>
		public static bool HasItem(this Player Ply, ItemType TargetItem)
		{
			foreach (var item in Ply.Inventory.UserInventory.Items)
			{
				if(item.Value.ItemTypeId == TargetItem)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Turns off the lights of the rooms in the specified zone type
		/// </summary>
		/// <param name="ZoneType">The Zone to blackout.</param>
		/// <returns>void</returns>
		public static void BlackoutZone(ZoneType Zone, float duration)
		{
			foreach(Room room in Room.List.Where(r => r.Zone == Zone))
			{
				room.TurnOffLights(duration);
			}
		}

		public static bool IsInfected(this Player ply)
		{
			return PlayhousePlugin.PlayhousePluginRef.Handler.InfectedPlayers.Contains(ply);
		}

		public static string GetGroupName(this UserGroup group)
			=> ServerStatic.GetPermissionsHandler().GetAllGroups().Where(p => p.Value == group).Select(p => p.Key)
				.FirstOrDefault();

		public static bool IsRainbowTagUser(this Player hub)
		{
			string group = ServerStatic.GetPermissionsHandler().GetUserGroup(hub.UserId)
				.GetGroupName();

			return !string.IsNullOrEmpty(group) && PlayhousePlugin.PlayhousePluginRef.Config.ActiveGroups.Contains(group);
		}

		public static RoleType GetRole(this Ragdoll ragdoll)
		{
			return ragdoll.NetworkInfo.RoleType;
		}

		public static bool IsCheckpoint(this DoorVariant door)
		{
			door.TryGetComponent(out DoorNametagExtension doorNameExt);
			if(doorNameExt != null)
			{
				if (doorNameExt.GetName.Contains("CHECKPOINT"))
					return true;
			}
			return false;
		}

		public static CustomClassManager CustomClassManager(this Player ply)
		{
			if (CustomClass.CustomClassManager.Players.ContainsKey(ply))
				return CustomClass.CustomClassManager.Players[ply];

			if (ply.GameObject.TryGetComponent(out CustomClassManager component))
			{
				CustomClass.CustomClassManager.Players.Add(ply, component);
				return component;
			}

			var obj = ply.GameObject.AddComponent<CustomClassManager>();
			CustomClass.CustomClassManager.Players.Add(ply, obj);
			return obj;
		}

		public static string GetCurrentAbilitySelectionName(this Player ply, out string Name)
		{
			var c = ply.CustomClassManager();
			if (c.CustomClass == null || c.CustomClass.AbilitiesNum == 0)
			{
				Name = null;
				return null;
			}
			Name = c.CustomClass.ActiveAbilities[c.AbilityIndex].Name;
			return Name;
		}
		
		public static bool HasCurrentAbilitySelection(this Player ply)
		{
			var c = ply.CustomClassManager();
			if (c.CustomClass == null || c.CustomClass.AbilitiesNum == 0)
				return false;
			return true;
		}

		public static Vector3 Offset(this Transform location, float forward, float up, float right) =>
			location.position + (location.forward * forward) + (location.up * up) + (location.right * right);

		public static T PickRandom<T>(this List<T> List)
		{
			return List[EventHandler.random.Next(List.Count)];
		}

		public static T PickRandom<T>(this IReadOnlyCollection<T> List)
		{
			return List.ElementAt(EventHandler.random.Next(List.Count));
		}

		public static void SendTextHint(this Player player, string text, float time)
		{
			player.ReferenceHub.hints.Show(new TextHint(text, new HintParameter[] { new StringHintParameter(string.Empty) }, new HintEffect[] { HintEffectPresets.TrailingPulseAlpha(0.5f, 1f, 0.5f, 2f, 0f, 2) }, time));
		}

		public static void SendTextHintNotEffect(this Player player, string text, float time)
		{
			player.ReferenceHub.hints.Show(new TextHint(text, new HintParameter[] { new StringHintParameter(string.Empty) }, null, time));
		}

		public static void ShowCenterHint(this Player player, string text, ulong time = 1)
		{
			player.GameObject.GetComponent<PlayhousePluginComponent>().AddHudCenterText(text, time);
		}
		
		public static void ShowCenterUpHint(this Player player, string text, ulong time = 1)
		{
			player.GameObject.GetComponent<PlayhousePluginComponent>().AddHudCenterUpText(text, time);
		}
		
		public static void ShowCenterDownHint(this Player player, string text, ulong time = 1)
		{
			player.GameObject.GetComponent<PlayhousePluginComponent>().AddHudCenterDownText(text, time);
		}

		public static int GetHealthAmountPercent(this Player player)
		{
			return (int)Math.Round(player.Health/player.MaxHealth, 0);
		}
		
		public static int GetAHPAmountPercent(this Player player)
		{
			return (int)(100f - (Mathf.Clamp01(1f - player.ArtificialHealth / (float)player.MaxArtificialHealth) * 100f));
		}

	}
}
