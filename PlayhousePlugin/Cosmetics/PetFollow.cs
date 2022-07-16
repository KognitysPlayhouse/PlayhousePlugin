using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayhousePlugin
{
	public class PetFollow
	{
		public static Dictionary<ItemType, List<int>> ItemsAndAngles = new Dictionary<ItemType, List<int>> {
			// Cards
			{ ItemType.KeycardNTFLieutenant, new List<int> {0, 90, 0 } },
			{ ItemType.KeycardNTFCommander, new List<int> {0, 90, 0 } },
			{ ItemType.KeycardContainmentEngineer, new List<int> {0, 90, 0 } },
			{ ItemType.KeycardFacilityManager, new List<int> {0, 90, 0 } },
			{ ItemType.KeycardO5, new List<int> {0, 90, 0 } },

			// SCP Items
			{ ItemType.SCP018, new List<int> {90, 200, 80 } },
			{ ItemType.SCP207, new List<int> {-90, 90, 0 } },
			{ ItemType.SCP268, new List<int> {260, 0, 90 } },

			// Misc
			{ ItemType.GrenadeFlash, new List<int> {0, 90, 0 } },
			{ ItemType.Medkit, new List<int> {90, 0, 0 } },
			{ ItemType.Adrenaline, new List<int> {0, 0, 0 } },
			{ ItemType.Coin, new List<int> {0, 90, 0 } },
			{ ItemType.Painkillers, new List<int> {-90, 0, 0 } },
			{ ItemType.MicroHID, new List<int> {-90, 0, 0 } },
			{ ItemType.Radio, new List<int> {0, 90, -90 } },
		};

		public static List<Pickup> Pets = new List<Pickup>();
		public static Dictionary<string, CoroutineHandle> Coroutines = new Dictionary<string, CoroutineHandle>();
		public static Dictionary<string, Pickup> IDsAndPickups = new Dictionary<string, Pickup>();

		public static IEnumerator<float> FollowPlayer(Player Ply, Pickup item)
		{
			yield return Timing.WaitForSeconds(0.25f);
			if (item.Type == ItemType.MicroHID)
			{
				item.Scale = new Vector3(0.5f, 0.5f, 0.5f);
			}

			List<int> Angles = ItemsAndAngles[item.Type];

			Pets.Add(item);
			IDsAndPickups.Add(Ply.UserId, item);

			int errorCounter = 0;
			bool cooldown = false;
			Vector3 pastPosition = Ply.Position;
			bool afk = false;
			int afkCounter = 0;
			float unitCircle = 0;

			var rigidbody = item.Base.gameObject.GetComponent<Rigidbody>();
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;

			float distanceX = EventHandler.random.Next(-150, 150) / 100;
			float distanceZ = EventHandler.random.Next(-150, 150) / 100;

			var pickup = item.Base;
			var pickupInfo = pickup.NetworkInfo;
			var pickupType = pickup.GetType();

			var fakePickupInfo = pickup.NetworkInfo;
			fakePickupInfo.Position = Vector3.zero;
			fakePickupInfo.Rotation = new LowPrecisionQuaternion(Quaternion.identity);

			while (Ply.IsAlive)
			{
				yield return Timing.WaitForSeconds(0.25f);
				try
				{
					pickupInfo.Rotation = new LowPrecisionQuaternion(Quaternion.Euler(Angles[0], Ply.CameraTransform.rotation.eulerAngles.y + Angles[1], Angles[2]));
					foreach (Player player in Player.List)
					{
						if (afkCounter > 50)
						{
							afk = true;
							if (Ply.CurrentRoom.Type == RoomType.Lcz914)
							{
								if (player.Role.Team == Team.SCP || Ply.ReferenceHub.playerEffectsController.GetEffect<Invisible>().IsEnabled)
								{
									MirrorExtensions.SendFakeSyncVar(player, pickup.netIdentity, pickupType, "NetworkInfo", fakePickupInfo);
								}
								else
								{
									pickupInfo.Position = new Vector3(
										Ply.CurrentRoom.Position.x + distanceX - Convert.ToSingle(Math.Cos(unitCircle) - 0.4),
										Ply.CurrentRoom.Position.y + 1.3f + Convert.ToSingle(Math.Cos(unitCircle)),
										Ply.CurrentRoom.Position.z + distanceZ - Convert.ToSingle(Math.Sin(unitCircle) - 0.4));

									MirrorExtensions.SendFakeSyncVar(player, pickup.netIdentity, pickupType, "NetworkInfo", pickupInfo);
								}
							}
							else
							{
								if (player.Role.Team == Team.SCP || Ply.ReferenceHub.playerEffectsController.GetEffect<Invisible>().IsEnabled)
								{
									MirrorExtensions.SendFakeSyncVar(player, pickup.netIdentity, pickupType, "NetworkInfo", fakePickupInfo);
								}
								else
								{
									pickupInfo.Position = new Vector3(Ply.Position.x - Convert.ToSingle(Math.Cos(unitCircle) - 0.1),
										Ply.Position.y - Convert.ToSingle(Math.Cos(unitCircle) + 0.1),
										Ply.Position.z - Convert.ToSingle(Math.Sin(unitCircle) - 0.1));

									MirrorExtensions.SendFakeSyncVar(player, pickup.netIdentity, pickupType, "NetworkInfo", pickupInfo);
								}
							}

							unitCircle += 1;
						}
						else
						{
							if (Ply.CurrentRoom.Type == RoomType.Lcz914)
							{
								if (player.Role.Team == Team.SCP || Ply.ReferenceHub.playerEffectsController.GetEffect<Invisible>().IsEnabled)
								{
									MirrorExtensions.SendFakeSyncVar(player, pickup.netIdentity, pickupType, "NetworkInfo", fakePickupInfo);
								}
								else
								{
									pickupInfo.Position = new Vector3(Ply.CurrentRoom.Position.x + distanceX,
										Ply.CurrentRoom.Position.y + 0.5f,
										Ply.CurrentRoom.Position.z + distanceZ);

									MirrorExtensions.SendFakeSyncVar(player, pickup.netIdentity, pickupType, "NetworkInfo", pickupInfo);
								}
							}
							else
							{
								if (player.Role.Team == Team.SCP || Ply.ReferenceHub.playerEffectsController.GetEffect<Invisible>().IsEnabled)
								{
									MirrorExtensions.SendFakeSyncVar(player, pickup.netIdentity, pickupType, "NetworkInfo", fakePickupInfo);
								}
								else
								{
									pickupInfo.Position = Ply.Position + Ply.CameraTransform.forward + Ply.CameraTransform.right;

									MirrorExtensions.SendFakeSyncVar(player, pickup.netIdentity, pickupType, "NetworkInfo", pickupInfo);
								}
							}
						}
					}
					if (cooldown)
					{
						cooldown = false;
						errorCounter = 0;
					}
					if (Vector3.Distance(pastPosition, Ply.Position) > 0.1)
					{
						pastPosition = Ply.Position;
						afk = false;
						afkCounter = 0;
						unitCircle = 0;
					}
					else
					{
						afkCounter += 1;
					}

				}
				catch
				{
					errorCounter++;
					cooldown = true;
					if (errorCounter > 5)
					{
						Ply.Broadcast(6, "<i>Psst, your hat has lost you! You need to requip it again!</i>");
						KillPet(Ply, item);
						break;
					}
				}
				if (afk)
				{
					yield return Timing.WaitForSeconds(0.25f);
				}
				if (cooldown)
				{
					yield return Timing.WaitForSeconds(2f);
				}
			}
		}

		public static void KillPet(Player Ply)
		{
			var item = IDsAndPickups[Ply.UserId];
			Pets.Remove(item);

			try
			{
				item.Destroy();
			}
			catch { }

			Timing.KillCoroutines(Coroutines[Ply.UserId]);
			Coroutines.Remove(Ply.UserId);
			IDsAndPickups.Remove(Ply.UserId);
		}

		public static void KillPet(Player Ply, Pickup item)
		{
			item.Destroy();

			Pets.Remove(item);
			Timing.KillCoroutines(Coroutines[Ply.UserId]);
			Coroutines.Remove(Ply.UserId);
			IDsAndPickups.Remove(Ply.UserId);
		}
	}
}
