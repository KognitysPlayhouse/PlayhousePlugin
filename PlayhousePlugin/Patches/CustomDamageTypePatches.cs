namespace PlayhousePlugin
{
	class CustomDamageTypePatches
	{
		/*
		[HarmonyPatch(typeof(DamageTypes), nameof(DamageTypes.FromWeaponId))]
		class DamageTypesFromWeaponIdPatch
		{
			public static bool Prefix(int weaponId, ref DamageTypes.DamageType __result)
			{
				var types = (KeyValuePair<DamageTypes.DamageType, int>[])typeof(DamageTypes).GetField("Types", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
				foreach (KeyValuePair<DamageTypes.DamageType, int> keyValuePair in types)
				{
					if (keyValuePair.Key.weaponId == weaponId)
					{
						__result = keyValuePair.Key;
						return false;
					}
				}
				__result = DamageTypes.None;
				return false;
			}
		}

		[HarmonyPatch(typeof(DamageTypes), nameof(DamageTypes.FromIndex))]
		class DamageTypesFromIndexPatch
		{
			public static bool Prefix(int id, ref DamageTypes.DamageType __result)
			{
				if (id < 0 || id >= DamageTypes.Types.Length)
				{
					__result = DamageTypes.None;
					return false;
				}
				__result = DamageTypes.Types[id].Key;
				return false;
			}
		}

		[HarmonyPatch(typeof(DamageTypes), nameof(DamageTypes.ToIndex))]
		class DamageTypesToIndexPatch
		{
			public static bool Prefix(DamageTypes.DamageType damageType, ref int __result)
			{
				var types = (KeyValuePair<DamageTypes.DamageType, int>[])typeof(DamageTypes).GetField("Types", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
				for (int i = 0; i < types.Length; i++)
				{
					if (types[i].Key == damageType)
					{
						__result = i;
						return false;
					}
				}
				__result = -1;
				return false;
			}
		}*/
	}
}
