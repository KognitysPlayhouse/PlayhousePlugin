using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace PlayhousePlugin
{
	public class CustomItemSpawning
	{
		private static List<LieutenantKeycardSpawns> SpawnedLocations = new List<LieutenantKeycardSpawns>();
		private static readonly System.Random Random = new System.Random();

		private enum LieutenantKeycardSpawns
		{
			Room939L,
			Room939R,
			Room049,
			Room106,
			Nuke,
		}

		public static void Clear()
		{
			SpawnedLocations.Clear();
		}

		public static void SpawnItems()
		{
			foreach (Room room in Room.List)
			{
				// LCZ
				if (room.Type == RoomType.LczGlassBox)
				{
					Item.Create(ItemType.Radio).Spawn(room.Transform.Offset(-8f, 1, 10), new Quaternion(-0.5f, 1, -0.5f, 0));
					Item.Create(ItemType.KeycardJanitor).Spawn(room.Transform.Offset(-8f, 2, 10), new Quaternion(-0.5f, 1, -0.5f, 0));
					Item.Create(ItemType.Coin).Spawn(room.Transform.Offset(-8f, 1, 9.5f), new Quaternion(-0.5f, 1, -0.5f, 0));
					Item.Create(ItemType.Coin).Spawn(room.Transform.Offset(-8f, 1, 9.5f), new Quaternion(-0.5f, 1, -0.5f, 0));
				}
				if (room.Type == RoomType.Lcz914)
				{
					Item.Create(ItemType.KeycardJanitor).Spawn(room.Transform.Offset(-10f, 1, 0));
					Item.Create(ItemType.Coin).Spawn(room.Transform.Offset(-10f, 1f, 0f), Quaternion.identity);
				}
				if (room.Type == RoomType.LczCurve)
				{
					if(UtilityMethods.RandomChance(2))
						Item.Create(ItemType.Coin).Spawn(room.Transform.TransformPoint(0f, 1f, 0f), Quaternion.identity);	
				}
				if (room.Type == RoomType.LczTCross)
				{
					if(UtilityMethods.RandomChance(2))
						Item.Create(ItemType.Coin).Spawn(room.Transform.TransformPoint(2.2f, 1f, 7.8f), Quaternion.identity);	
				}
				if (room.Type == RoomType.LczCafe)
				{
					Item.Create(ItemType.Coin).Spawn(room.Transform.TransformPoint(10.8f, 2.2f, 1.4f), Quaternion.identity);
					if(UtilityMethods.RandomChance(2))
						Item.Create(ItemType.Coin).Spawn(room.Transform.TransformPoint(-3.3f, 2.1f, -1.7f), Quaternion.identity);	
				}
				
				// EZ
				if (room.Type == RoomType.EzUpstairsPcs)
				{
					Item.Create(ItemType.Coin).Spawn(room.Transform.TransformPoint(-1.4f, 2.3f, 0.6f), Quaternion.identity);
					Item.Create(ItemType.Coin).Spawn(room.Transform.TransformPoint(-5.3f, 2.3f, -0.7f), Quaternion.identity);
				}
				if (room.Type == RoomType.EzCafeteria)
				{
					Item.Create(ItemType.Coin).Spawn(room.Transform.TransformPoint(-1.8f, 8f, 5.1f), Quaternion.identity);
				}
				if (room.Type == RoomType.EzTCross)
				{
					Item.Create(ItemType.Coin).Spawn(room.Transform.TransformPoint(0.0f, 1f, -5.2f), Quaternion.identity);
				}
				if (room.Type == RoomType.EzPcs)
				{
					Item.Create(ItemType.Coin).Spawn(room.Transform.TransformPoint(-1.3f, 3.2f, 0.6f), Quaternion.identity);
				}
			}
			if (Server.Port == 9999) return;

			if (Player.List.Count() > 9)
			{
				SpawnItem();
			}

			if (Player.List.Count() > 19)
			{
				SpawnItem();
			}

			if (Player.List.Count() > 29)
			{
				SpawnItem();
			}
		}

		private static LieutenantKeycardSpawns PickRandomSpawnPoint()
		{
			List<LieutenantKeycardSpawns> values = Enum.GetValues(typeof(LieutenantKeycardSpawns)).ToArray<LieutenantKeycardSpawns>().ToList();
			foreach(LieutenantKeycardSpawns spawn in SpawnedLocations)
				values.Remove(spawn);
			return values[Random.Next(values.Count)];
		}

		private static Vector3 GetPositionFromSpawnPoint(LieutenantKeycardSpawns spawnpoint)
		{
			switch (spawnpoint)
			{
				case LieutenantKeycardSpawns.Room049:
					return RoleExtensions.GetRandomSpawnProperties(RoleType.Scp049).Item1;
				case LieutenantKeycardSpawns.Room106:
					return Room.List.Where(x => x.Type == RoomType.Hcz106).FirstOrDefault().Transform.Offset(-30f, 1f, 24f);
				case LieutenantKeycardSpawns.Room939L:
					return Room.List.Where(x => x.Type == RoomType.Hcz939).FirstOrDefault().Transform.Offset(-8f, -15.5f, -3f);
				case LieutenantKeycardSpawns.Room939R:
					return Room.List.Where(x => x.Type == RoomType.Hcz939).FirstOrDefault().Transform.Offset(-8f, -15.5f, 3f);
				case LieutenantKeycardSpawns.Nuke:
					return Room.List.Where(x => x.Type == RoomType.HczNuke).FirstOrDefault().Transform.Offset(17f, 403f, -6.5f);
				default:
					return Vector3.zero;
			}
		}

		private static void SpawnItem()
		{
			LieutenantKeycardSpawns SpawnPoint = PickRandomSpawnPoint();
			SpawnedLocations.Add(SpawnPoint);

			Item.Create(ItemType.KeycardNTFLieutenant).Spawn(GetPositionFromSpawnPoint(SpawnPoint));
			Log.Info(SpawnPoint);
		}
	}
}