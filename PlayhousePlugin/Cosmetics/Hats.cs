using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdminToys;
using Exiled.API.Extensions;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MEC;
using Mirror;
using UnityEngine;

namespace PlayhousePlugin
{
	public class HatOwner: IEquatable<HatOwner>
	{
		public string UserID { get; set; }
		public SchematicObject Hat { get; set; }
		public CoroutineHandle CoroutineHandle { get; set; }

		public override string ToString()
		{
			return $"{UserID}";
		}

		public bool Equals(HatOwner other)
		{
			return UserID == other.UserID;
		}

		public override bool Equals(object other)
		{
			return other is HatOwner dt && this.Equals(dt);
		}

		public static bool operator ==(HatOwner left, HatOwner right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(HatOwner left, HatOwner right)
		{
			return !Equals(left, right);
		}
		
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (UserID != null ? UserID.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Hat != null ? Hat.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (CoroutineHandle != null ? CoroutineHandle.GetHashCode() : 0);
				return hashCode;
			}
		}
	}

	public class Hat
	{
		public static List<HatOwner> HatOwners = new List<HatOwner>();
		
		public static void KillHat(string UserId)
		{
			if (HatOwners != null)
			{
				HatOwner first = null;
				foreach (var x in HatOwners)
				{
					if (x.UserID == UserId)
					{
						first = x;
						break;
					}
				}

				if (first == null) return;
				Timing.KillCoroutines(first.CoroutineHandle);
				first.Hat.Destroy();
			}
		}
		
		public static void KillHat(Player ply)
		{
			if (HatOwners != null)
			{
				HatOwner first = null;
				foreach (var x in HatOwners)
				{
					if (x.UserID == ply.UserId)
					{
						first = x;
						break;
					}
				}

				if (first == null) return;
				first.Hat.Destroy();
				Timing.KillCoroutines(first.CoroutineHandle);

				HatOwners.Remove(first);
			}
		}

		public static void KillAllhats()
		{
			foreach (var hatOwner in HatOwners)
			{
				Timing.KillCoroutines(hatOwner.CoroutineHandle);
				hatOwner.Hat.Destroy();
			}
		}

		public static void SpawnHat(Player Ply, string hatName)
		{
			var hat = MapUtils.GetSchematicDataByName(hatName);
			var hatObject = ObjectSpawner.SpawnSchematic(hatName,
				Ply.CameraTransform.position + Vector3.up*1.4f,
				Quaternion.Euler(0,Ply.CameraTransform.rotation.eulerAngles.y, 0), Vector3.one, hat);

			var coroutine = Timing.RunCoroutine(HatFollow(Ply, hatObject));
			
			HatOwners.Add(new HatOwner()
				{
					UserID = Ply.UserId,
					CoroutineHandle = coroutine,
					Hat = hatObject
				}
			);
		}
		
		public static IEnumerator<float> HatFollow(Player Ply, SchematicObject obj)
        {
	        Dictionary<GameObject, Vector3> DefaultScales = new Dictionary<GameObject, Vector3>();
            
            foreach (var block in obj.AttachedBlocks)
            {
                Component[] components = block.GetComponents(typeof(Component));
                foreach(Component component in components) {
                    Log.Info(component.ToString());
                }
                
                if(block.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                    DefaultScales.Add(block, primitive.Scale);
            }

            Dictionary<Player, bool> CanSee = new Dictionary<Player, bool>();

            while (Ply.IsAlive)
            {
                yield return Timing.WaitForSeconds(0.05f);
                
                obj.transform.position = Ply.CameraTransform.position;
                obj.transform.rotation =
                    Quaternion.Euler(0, Ply.CameraTransform.rotation.eulerAngles.y, 0);
                
                try
                {
                    foreach (var player in Player.List)
                    {
                        if (player.IsScp)
                        {
                            if (CanSee.ContainsKey(player))
                            {
                                if (!CanSee[player]) continue;
                                CanSee[player] = false;

                                foreach (var block in obj.AttachedBlocks)
                                {
                                    if (block.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                                    {
                                        MirrorExtensions.SendFakeSyncVar(player, block.GetComponent<NetworkIdentity>(),
                                            block.GetComponent<PrimitiveObjectToy>().GetType(),
                                            "NetworkScale", Vector3.zero);
                                    }
                                }
                                
                            }
                            else
                            {
                                CanSee.Add(player, false);

                                foreach (var block in obj.AttachedBlocks)
                                {
                                    if (block.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                                    {
                                        MirrorExtensions.SendFakeSyncVar(player, block.GetComponent<NetworkIdentity>(),
                                            block.GetComponent<PrimitiveObjectToy>().GetType(),
                                            "NetworkScale", Vector3.zero);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (CanSee.ContainsKey(player))
                            {
                                if(CanSee[player]) continue;
                                CanSee[player] = true;
                                
                                foreach (var block in obj.AttachedBlocks)
                                {
                                    if (block.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                                    {
                                        MirrorExtensions.SendFakeSyncVar(player, block.GetComponent<NetworkIdentity>(),
                                            block.GetComponent<PrimitiveObjectToy>().GetType(),
                                            "NetworkScale", DefaultScales[block]);
                                    }
                                }
                            }
                            else
                            {
                                CanSee.Add(player, true);

                                foreach (var block in obj.AttachedBlocks)
                                {
                                    if (block.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                                    {
                                        MirrorExtensions.SendFakeSyncVar(player, block.GetComponent<NetworkIdentity>(),
                                            block.GetComponent<PrimitiveObjectToy>().GetType(),
                                            "NetworkScale", DefaultScales[block]);
                                    }
                                }
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Info(e);
                }
            }
            KillHat(Ply);
        }
	}
}
