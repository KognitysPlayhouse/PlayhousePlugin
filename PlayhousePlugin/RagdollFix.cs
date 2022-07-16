using Assets._Scripts.Dissonance;
using HarmonyLib;
using System.Linq;
using Exiled.Permissions.Extensions;
using UnityEngine;
using Exiled.API.Features;

namespace SCPUtils
{
	[HarmonyPatch(typeof(Ragdoll), nameof(Ragdoll.Refreeze))]
	public class RagdollFix
	{
		public static void Prefix(Ragdoll __instance)
		{
			Log.Info("Test");
			bool flag = false;
			Rigidbody[] componentsInChildren = __instance.GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody rigidbody in componentsInChildren)
			{
				foreach (Rigidbody rigidbody2 in __instance.LastRagdollPos)
				{
					if (!(rigidbody != rigidbody2) && Vector3.Distance(rigidbody.position, rigidbody2.position) > 0.11f)
					{
						flag = true;
					}
				}
			}
			__instance.LastRagdollPos.Clear();
			__instance.LastRagdollPos.AddRange(componentsInChildren);
			if (__instance.CurrentTime < (float)__instance.MaxRagdollTime || flag)
			{
				return;
			}
			CharacterJoint[] componentsInChildren2 = __instance.GetComponentsInChildren<CharacterJoint>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				//UnityEngine.Object.Destroy(componentsInChildren2[j]);
			}
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				//UnityEngine.Object.Destroy(componentsInChildren[k]);
			}
			__instance.CancelInvoke("Refreeze");
		}
	}
}