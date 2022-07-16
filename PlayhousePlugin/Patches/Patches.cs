using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Exiled.API.Features;
using HarmonyLib;
using Hints;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using Mirror;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Keycards;
using NorthwoodLib.Pools;
using PlayableScps;
using PlayerStatsSystem;
using PlayhousePlugin.CustomClass.SCP;
using PlayhousePlugin.CustomClass.SCP_Abilities;
using Scp096 = PlayableScps.Scp096;


namespace PlayhousePlugin
{
	// SCP Speaking
	[HarmonyPatch(typeof(Radio), nameof(Radio.UserCode_CmdSyncTransmissionStatus))]
	public static class SpeechPatch
	{
		public static bool Prefix(Radio __instance, bool b)
		{
			if (Player.Get(__instance._hub).IsScp)
				__instance._dissonanceSetup.MimicAs939 = b;
			return true;
		}
	}

	
	[HarmonyPatch(typeof(Scp096), nameof(Scp096.UpdateVision))]
	public static class Scp096DelayRemove
	{
		public static bool Prefix(Scp096 __instance)
		{
			Vector3 vector = __instance.Hub.transform.TransformPoint(Scp096._headOffset);
			foreach (KeyValuePair<GameObject, global::ReferenceHub> keyValuePair in global::ReferenceHub.GetAllHubs())
			{
				ReferenceHub value = keyValuePair.Value;
				CharacterClassManager characterClassManager = value.characterClassManager;
				if (characterClassManager.CurClass != RoleType.Spectator &&
				    characterClassManager.CurClass != RoleType.Tutorial &&
				    !(value == __instance.Hub) &&
				    !characterClassManager.IsAnyScp() &&
				    Vector3.Dot((value.PlayerCameraReference.position - vector).normalized, __instance.Hub.PlayerCameraReference.forward) >= 0.1f)
				{
					VisionInformation visionInformation = VisionInformation.GetVisionInformation(value, vector, -0.1f, 60f, true, true, __instance.Hub.localCurrentRoomEffects, 0);
					float distance = Vector3.Distance(value.playerMovementSync.GetRealPosition(),
						__instance.Hub.playerMovementSync.GetRealPosition());

					float delay;
					if (distance <= 3)
						delay = 3;
					else
						delay = 9 / distance; 
					
					if (visionInformation.IsLooking)
					{
						if (!__instance.Calming)
						{
							__instance.AddTarget(value.gameObject);
						}
						if (__instance.CanEnrage && value.gameObject != null)
						{
							__instance.PreWindup(delay);
						}
					}
				}
			}

			return false;
		}
	}
	
	[HarmonyPatch(typeof(InventorySystem.Items.Keycards.KeycardPickup), nameof(KeycardPickup.ProcessCollision))]
	public class KeycardCollisionPatch
	{
		public static bool Prefix(KeycardPickup __instance, Collision collision)
		{
			if (!NetworkServer.active)
			{
				return false;
			}
			RegularDoorButton regularDoorButton;
			if (!collision.collider.TryGetComponent<RegularDoorButton>(out regularDoorButton))
			{
				return false;
			}
			DoorVariant doorVariant;
			if ((doorVariant = (regularDoorButton.Target as DoorVariant)) != null && Door.Get(doorVariant).Nametag.Contains("106"))
			{
				if(Generator.List.Count(x=>x.IsEngaged) < 2)
					return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.ServerPerformShot))]
	public class SingleBulletHitregPatch
	{
		public static bool Prefix(SingleBulletHitreg __instance, Ray ray)
		{
			FirearmBaseStats stats = __instance.Firearm.BaseStats;

			Vector3 randomVector = (new Vector3(Random.value, Random.value, Random.value) - Vector3.one / 2).normalized * Random.value;
			float angle = stats.GetInaccuracy(__instance.Firearm, __instance.Firearm.AdsModule.ServerAds, __instance.Hub.playerMovementSync.PlayerVelocity.magnitude, __instance.Hub.playerMovementSync.Grounded);

			if (__instance._usesRecoilPattern)
			{
				__instance._recoilPattern.ApplyShot(1f / __instance.Firearm.ActionModule.CyclicRate);
				angle += __instance._recoilPattern.GetInaccuracy();
			}

			ray.direction = Quaternion.Euler(angle * randomVector) * ray.direction;

			if (Physics.Raycast(ray, out RaycastHit hit, stats.MaxDistance(), LayerMask.GetMask("Default", "Hitbox", "Glass", "CCTV", "Door", "Locker")))
			{
				if (hit.collider.TryGetComponent(out IDestructible target) && __instance.CheckInaccurateFriendlyFire(target))
				{
					__instance.RestorePlayerPosition();

					float dmg = stats.DamageAtDistance(__instance.Firearm, hit.distance);

					if (target is HitboxIdentity identity)
					{
						if (identity.TargetHub.characterClassManager.NetworkCurClass == RoleType.Scp106)
						{
							var p = Player.Get(identity.TargetHub);
							var shooter = Player.Get(__instance.Hub);
							SCP106CustomClass customClass = (SCP106CustomClass) p.CustomClassManager().CustomClass;
							Vanish ability = (Vanish) customClass.ActiveAbilities[0];

							if (ability.IsVanish)
							{
								// If they are NOT in the list they can DO damage
								if (!p.TargetGhostsHashSet.Contains(shooter.Id) && !shooter.TargetGhostsHashSet.Contains(p.Id))
								{
									if (target.Damage(dmg, new FirearmDamageHandler(__instance.Firearm, dmg), hit.point))
									{
										Hitmarker.SendHitmarker(__instance.Conn, 1);
										__instance.ShowHitIndicator(target.NetworkId, dmg, ray.origin);
										__instance.PlaceBloodDecal(ray, hit, target);
									}
								}
								else
								{
									return false;
								}
							}
							else
							{
								if (target.Damage(dmg, new FirearmDamageHandler(__instance.Firearm, dmg), hit.point))
								{
									Hitmarker.SendHitmarker(__instance.Conn, 1);
									__instance.ShowHitIndicator(target.NetworkId, dmg, ray.origin);
									__instance.PlaceBloodDecal(ray, hit, target);
								}
							}
						}
						else
						{
							if (target.Damage(dmg, new FirearmDamageHandler(__instance.Firearm, dmg), hit.point))
							{
								Hitmarker.SendHitmarker(__instance.Conn, 1);
								__instance.ShowHitIndicator(target.NetworkId, dmg, ray.origin);
								__instance.PlaceBloodDecal(ray, hit, target);
							}
						}
					}
					else
					{
						if (target.Damage(dmg, new FirearmDamageHandler(__instance.Firearm, dmg), hit.point))
						{
							Hitmarker.SendHitmarker(__instance.Conn, 1);
							__instance.ShowHitIndicator(target.NetworkId, dmg, ray.origin);
							__instance.PlaceBloodDecal(ray, hit, target);
						}
					}
				}
				else
				{
					__instance.PlaceBulletholeDecal(ray, hit);
				}
			}

			return false;
		}
	}

	[HarmonyPatch(typeof(BuckshotHitreg), nameof(BuckshotHitreg.ApplyHits))]
	public class ApplyHitsPatch
	{
		public static bool Prefix(BuckshotHitreg __instance, ref float __result, IDestructible target, List<BuckshotHitreg.ShotgunHit> hits)
		{
			float totalDamage = 0;

			foreach (BuckshotHitreg.ShotgunHit hit in hits)
			{
				float dmg = hit.Damage;

				if (target is HitboxIdentity identity)
				{
					if (identity.TargetHub.characterClassManager.NetworkCurClass == RoleType.Scp106)
					{
						var p = Player.Get(identity.TargetHub);
						var shooter = Player.Get(__instance.Hub);
						SCP106CustomClass customClass = (SCP106CustomClass) p.CustomClassManager().CustomClass;
						Vanish ability = (Vanish) customClass.ActiveAbilities[0];

						if (ability.IsVanish)
						{
							// If they are NOT in the list they can DO damage
							if (!p.TargetGhostsHashSet.Contains(shooter.Id) && !shooter.TargetGhostsHashSet.Contains(p.Id))
							{
								if (!target.Damage(dmg, new FirearmDamageHandler(__instance.Firearm, dmg, false), hit.RcResult.point))
									continue;

								__instance.PlaceBloodDecal(hit.RcRay, hit.RcResult, target);
								totalDamage += dmg;
							}
							else
							{
								continue;
							}
						}
						else
						{
							if (!target.Damage(dmg, new FirearmDamageHandler(__instance.Firearm, dmg, false), hit.RcResult.point))
								continue;

							__instance.PlaceBloodDecal(hit.RcRay, hit.RcResult, target);
							totalDamage += dmg;
						}
					}
					else
					{
						if (!target.Damage(dmg, new FirearmDamageHandler(__instance.Firearm, dmg, false), hit.RcResult.point))
							continue;

						__instance.PlaceBloodDecal(hit.RcRay, hit.RcResult, target);
						totalDamage += dmg;
					}
				}
				else
				{
					if (!target.Damage(dmg, new FirearmDamageHandler(__instance.Firearm, dmg, false), hit.RcResult.point))
						continue;

					__instance.PlaceBloodDecal(hit.RcRay, hit.RcResult, target);
					totalDamage += dmg;
				}
			}

			__instance.ShowHitIndicator(target.NetworkId, totalDamage, __instance.Hub.transform.position);
			__result =  totalDamage;

			return false;
		}
	}

	[HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBulletholeDecal))]
	public class PlaceBulletHolePatch
	{
		public static bool Prefix(StandardHitregBase __instance, Ray ray, RaycastHit hit)
		{
			if (EventHandler.IsDeathMatchServer || Commands.DisableBulletHoles.DisableBulletHolesBool)
				return false;
			return true;
		}
	}
	
	[HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
	public static class OverrideHintPatch
	{
		public static bool Prefix(Hint hint)
		{
			if(hint.GetType() == typeof(TranslationHint))
				return false;

			if(hint._effects != null && hint._effects.Length > 0)
				return false;

			return true;
		}
	}
	
	/// <summary>
	/// Patches <see cref="StandardHitregBase.ShowHitIndicator"/> to fix crash when shooting near dummy.
	/// </summary>
	[HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.ShowHitIndicator))]
	internal static class DummyShowHitIndicatorFix
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

			const int offset = 1;

			int index = newInstructions.FindIndex(inst => inst.opcode == OpCodes.Ldloc_0) + offset;

			Label okLabel = generator.DefineLabel();

			// if(referenceHub.networkIdentity.connectionToClient == null)
			// {
			//   return;
			// }
			newInstructions.InsertRange(index, new[]
			{
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ReferenceHub), nameof(ReferenceHub.networkIdentity))),
				new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.connectionToClient))),
				new CodeInstruction(OpCodes.Brtrue_S, okLabel),
				new CodeInstruction(OpCodes.Ret),
				new CodeInstruction(OpCodes.Ldloc_0).WithLabels(okLabel),
			});

			for (int z = 0; z < newInstructions.Count; z++)
				yield return newInstructions[z];

			ListPool<CodeInstruction>.Shared.Return(newInstructions);
		}
	}
	
	/// <summary>
	/// Patches <see cref="FirearmExtensions.ServerSendAudioMessage"/> to fix crash when shooting near dummy.
	/// </summary>
	[HarmonyPatch(typeof(FirearmExtensions), nameof(FirearmExtensions.ServerSendAudioMessage))]
	internal static class DummyAudioMessageFix
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

			const int offset = 3;
			const int continueOffset = 2;

			int baseIndex = newInstructions.FindLastIndex(inst => inst.opcode == OpCodes.Callvirt && ((MethodInfo)inst.operand) == AccessTools.PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner)));

			Label continueLabel = (Label)newInstructions[baseIndex + continueOffset].operand;

			// if(referenceHub.networkIdentity.connectrionToClient == null)
			// {
			//   continue;
			// }
			newInstructions.InsertRange(baseIndex + offset, new[]
			{
				new CodeInstruction(OpCodes.Ldloc_S, 5),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ReferenceHub), nameof(ReferenceHub.networkIdentity))),
				new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.connectionToClient))),
				new CodeInstruction(OpCodes.Brfalse_S, continueLabel),
			});

			for (int z = 0; z < newInstructions.Count; z++)
				yield return newInstructions[z];

			ListPool<CodeInstruction>.Shared.Return(newInstructions);
		}
	}

	/*
	[HarmonyPatch(typeof(CollisionDetectionPickup), nameof(CollisionDetectionPickup.OnCollisionEnter))]
	internal class StickyGrenade
	{
		private static void Prefix(EffectGrenade __instance)
		{
			if (EventHandler.IsDevServer)
			{
				Log.Info((Pickup.Get(__instance).Type));
				var rigidbody = __instance.gameObject.GetComponent<Rigidbody>();
				rigidbody.isKinematic = false;
				rigidbody.useGravity = false;
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
				rigidbody.freezeRotation = true;
				rigidbody.mass = 100000;
			}
		}
	}*/
	
	/*
// Stickybomb logic
[HarmonyPatch(typeof(Grenade), nameof(Grenade.OnCollisionEnter))]
public class StickyGrenade
{	
	public static void Prefix(Grenade __instance)
	{
		if (PlayhousePlugin.PlayhousePluginRef.Handler.TempStickies.ContainsKey(__instance))
		{ 
			var rigidbody = __instance.gameObject.GetComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			rigidbody.useGravity = false;
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			rigidbody.freezeRotation = true;
			rigidbody.mass = 100000;

			// Get the player and change the sticky array to the one currently
			Player player = Player.Get(__instance.throwerGameObject);


			var item = Exiled.API.Extensions.Item.Spawn(ItemType.SCP207, 0, __instance.transform.localPosition, rotation:__instance.transform.localRotation);
			GameObject gameObject = item.gameObject;
			gameObject.transform.localScale = new Vector3(3f, 0.5f, 3f);
			NetworkServer.UnSpawn(gameObject);
			NetworkServer.Spawn(item.gameObject);
			PlayhousePlugin.PlayhousePluginRef.Handler.StickyPositions[player][PlayhousePlugin.PlayhousePluginRef.Handler.TempStickies[__instance]] = item;

			var rigidbody2 = item.gameObject.GetComponent<Rigidbody>();
			rigidbody2.isKinematic = false;
			rigidbody2.useGravity = false;
			rigidbody2.velocity = Vector3.zero;
			rigidbody2.angularVelocity = Vector3.zero;
			rigidbody2.freezeRotation = true;
			rigidbody2.drag = 99999999999999;
			rigidbody2.mass = 100000;

			//item.Networkposition = __instance.transform.position;
			//item.Networkrotation = __instance.transform.rotation;

			NetworkServer.Destroy(__instance.gameObject);
			PlayhousePlugin.PlayhousePluginRef.Handler.TempStickies.Remove(__instance);
			return;
		}
	}
}*/

	/*
	[HarmonyPatch(typeof(Ragdoll), nameof(Ragdoll.Refreeze))]
	public class RagdollFix
	{
		public static void Prefix(Ragdoll __instance)
		{
			// We'll always have the rigidbodies, might delete this patch later but it's not that big of a deal right now.
			__instance.CancelInvoke("Refreeze");
			return;
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
				UnityEngine.Object.Destroy(componentsInChildren2[j]);
			}
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				UnityEngine.Object.Destroy(componentsInChildren[k]);
			}
			__instance.CancelInvoke("Refreeze");
		}
	}*/

	/*
	// Hemorrhage Damage Type
	[HarmonyPatch(typeof(CustomPlayerEffects.Hemorrhage), nameof(CustomPlayerEffects.Hemorrhage.PublicUpdate))]
	class HemorrhagePublicUpdatePatch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var newInstructions = instructions.ToList();

			newInstructions.Last(i => i.opcode == OpCodes.Ldsfld).operand = AccessTools.Field(typeof(PlayhousePlugin), nameof(PlayhousePlugin.Hemorrhage));

			foreach (var code in newInstructions)
				yield return code;
		}
	}*/
}