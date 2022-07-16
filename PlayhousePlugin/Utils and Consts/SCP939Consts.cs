using Exiled.API.Features;
using System.Collections.Generic;

namespace PlayhousePlugin
{
	public class SCP939Consts
	{
		public static Dictionary<Player, int> killsBy939 = new Dictionary<Player, int> { };
		public static bool InDict(Player ply)
		{
			if (killsBy939.ContainsKey(ply))
				return true;
			else
				return false;
		}

		public class SCP93989
		{
			public static Dictionary<int, float> killsAndSize = new Dictionary<int, float>
			{
				{1, 0.835f},
				{2, 0.87f},
				{3, 0.905f},
				{4, 0.94f},
				{5, 0.975f},
				{6, 1.01f},
				{7, 1.045f},
				{8, 1.08f},
				{9, 1.115f},
				{10, 1.15f}
			};
			public static Dictionary<int, float> killsAndDamage = new Dictionary<int, float>
			{
				{1, 55},
				{2, 60},
				{3, 65},
				{4, 70},
				{5, 75},
				{6, 80},
				{7, 85},
				{8, 90},
				{9, 95},
				{10, 100}
			};
		}
		public class SCP93953
		{
			public static Dictionary<Player, bool> hasChargeKilled = new Dictionary<Player, bool> { }; // When 939-53 spawns, check if in list if not add them
			public static Dictionary<int, byte> killsAndCharge = new Dictionary<int, byte>
			{
				{1, 2},
				{2, 2},
				{3, 2},
				{4, 2},
				{5, 3},
				{6, 3},
				{7, 3},
				{8, 3},
				{9, 3},
				{10, 4}
			};
			public static Dictionary<int, float> killsAndSlowdown = new Dictionary<int, float>
			{
				{1, 1.8f},
				{2, 1.6f},
				{3, 1.4f},
				{4, 1.2f},
				{5, 1f},
				{6, 0.8f},
				{7, 0.6f},
				{8, 0.4f},
				{9, 0.2f},
				{10, 0f}
			};
		}
	}
}
