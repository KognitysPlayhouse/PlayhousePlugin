using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using Mirror;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
	public static class PassiveAbilities
	{
		public static IEnumerator<float> ToxicZone(Player Zombie)
		{
			Vector3 position = Zombie.Position;
			yield return Timing.WaitForSeconds(0.5f);

			var circle = UnityEngine.Object.Instantiate(Utils.PrimitiveBaseObject);
			circle.NetworkPrimitiveType = PrimitiveType.Cylinder;
			circle.NetworkMaterialColor = new Color(1, 0, 0, 0.3f);
			circle.NetworkMovementSmoothing = 60;
			circle.transform.localScale = new Vector3(-4, -0.1f, -4);
			circle.transform.position = position + Vector3.down * 1.4f;
			
			NetworkServer.Spawn(circle.gameObject);
			circle.UpdatePositionServer();
			
			for (int x = 0; x < 50; x++)
			{
				foreach (Player ply in Player.List)
				{
					if (Vector3.Distance(ply.Position, position) <= 4)
					{
						if (!ply.IsScp && ply != Zombie)
						{
							if (ply.Health <= 1)
							{
								ply.Kill("Zombie Infection");
							}
							else
							{
								ply.Health -= 1;
							}

							if (!ply.IsInfected())
								UtilityMethods.InfectPlayer(ply);

						}
					}
				}
				yield return Timing.WaitForSeconds(0.2f);
			}
			
			NetworkServer.Destroy(circle.gameObject);
		}
	}
}