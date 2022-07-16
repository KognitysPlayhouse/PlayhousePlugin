namespace PlayhousePlugin
{
	public static class Cock
	{
		/*
		public static IEnumerator<float> FollowPlayer(Player Ply, Pickup ball1, Pickup ball2, Pickup cock)
		{
			List<int> Angle207 = new List<int> { 90, 0, 0 };

			Rigidbody rigidBody = ball1.gameObject.GetComponent<Rigidbody>();
			Collider[] collider = ball1.gameObject.GetComponents<Collider>();

			Rigidbody rigidBody1 = ball2.gameObject.GetComponent<Rigidbody>();
			Collider[] collider1 = ball2.gameObject.GetComponents<Collider>();

			Rigidbody rigidBody2 = cock.gameObject.GetComponent<Rigidbody>();
			Collider[] collider2 = cock.gameObject.GetComponents<Collider>();

			int errorCounter = 0;
			bool cooldown = false;

			Vector3 pastPosition = Ply.Position;
			bool afk = false;
			int afkCounter = 0;
			float unitCircle = 0;

			foreach (Collider thing in collider)
			{
				thing.enabled = false;
			}

			if (rigidBody != null)
			{
				rigidBody.useGravity = false;
				rigidBody.detectCollisions = false;
			}

			foreach (Collider thing in collider1)
			{
				thing.enabled = false;
			}

			if (rigidBody1 != null)
			{
				rigidBody1.useGravity = false;
				rigidBody1.detectCollisions = false;
			}

			foreach (Collider thing in collider2)
			{
				thing.enabled = false;
			}

			if (rigidBody2 != null)
			{
				rigidBody2.useGravity = false;
				rigidBody2.detectCollisions = false;
			}


			float distanceX = EventHandler.random.Next(-150, 150) / 100;
			float distanceZ = EventHandler.random.Next(-150, 150) / 100;

			while (Ply.IsAlive)
			{
				yield return Timing.WaitForSeconds(0.5f);
				try
				{
					foreach (Player player in Player.List)
					{
						if (afkCounter > 25)
						{
							afk = true;
							if (Ply.CurrentRoom.Type == RoomType.Lcz914)
							{
								if (player.Role.Team == Team.SCP || Ply.ReferenceHub.playerEffectsController.GetEffect<Invisible>().IsEnabled)
								{
									player.SendCustomSync(ball1.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
									player.SendCustomSync(ball2.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
									player.SendCustomSync(cock.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
								}
								else
								{
									player.SendCustomSync(ball1.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(new Vector3(Ply.CurrentRoom.Position.x + distanceX - Convert.ToSingle(Math.Cos(unitCircle) - 0.4), Ply.CurrentRoom.Position.y + Convert.ToSingle(1.3) + Convert.ToSingle(Math.Cos(unitCircle)), Ply.CurrentRoom.Position.z + distanceZ - Convert.ToSingle(Math.Sin(unitCircle) - 0.4)));
										//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
									});
									player.SendCustomSync(ball2.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(new Vector3(Ply.CurrentRoom.Position.x + distanceX - Convert.ToSingle(Math.Cos(unitCircle) - 0.4), Ply.CurrentRoom.Position.y + Convert.ToSingle(1.3) + Convert.ToSingle(Math.Cos(unitCircle)), Ply.CurrentRoom.Position.z + distanceZ - Convert.ToSingle(Math.Sin(unitCircle) - 0.4)));
										//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
									});
									player.SendCustomSync(cock.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(new Vector3(Ply.CurrentRoom.Position.x + distanceX - Convert.ToSingle(Math.Cos(unitCircle) - 0.4), Ply.CurrentRoom.Position.y + Convert.ToSingle(1.3) + Convert.ToSingle(Math.Cos(unitCircle)), Ply.CurrentRoom.Position.z + distanceZ - Convert.ToSingle(Math.Sin(unitCircle) - 0.4)));
										//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
									});
								}
							}
							else
							{
								if (player.Role.Team == Team.SCP || Ply.ReferenceHub.playerEffectsController.GetEffect<Invisible>().IsEnabled)
								{
									player.SendCustomSync(ball1.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
									player.SendCustomSync(ball2.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
									player.SendCustomSync(cock.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
								}
								else
								{
									player.SendCustomSync(ball1.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(new Vector3(Ply.Position.x - Convert.ToSingle(Math.Cos(unitCircle) - 0.1), Ply.Position.y - Convert.ToSingle(Math.Cos(unitCircle) + 0.1), Ply.Position.z - Convert.ToSingle(Math.Sin(unitCircle) - 0.1)));
										//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
									});
									player.SendCustomSync(ball2.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(new Vector3(Ply.Position.x - Convert.ToSingle(Math.Cos(unitCircle) - 0.1), Ply.Position.y - Convert.ToSingle(Math.Cos(unitCircle) + 0.1), Ply.Position.z - Convert.ToSingle(Math.Sin(unitCircle) - 0.1)));
										//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
									});
									player.SendCustomSync(cock.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(new Vector3(Ply.Position.x - Convert.ToSingle(Math.Cos(unitCircle) - 0.1), Ply.Position.y - Convert.ToSingle(Math.Cos(unitCircle) + 0.1), Ply.Position.z - Convert.ToSingle(Math.Sin(unitCircle) - 0.1)));
										//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
									});
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
									player.SendCustomSync(ball1.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
									player.SendCustomSync(ball2.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
									player.SendCustomSync(cock.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
								}
								else
								{
									player.SendCustomSync(ball1.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(new Vector3(Ply.CurrentRoom.Position.x + distanceX, Ply.CurrentRoom.Position.y + Convert.ToSingle(0.5), Ply.CurrentRoom.Position.z + distanceZ));
										//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
									});
									player.SendCustomSync(ball2.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(new Vector3(Ply.CurrentRoom.Position.x + distanceX, Ply.CurrentRoom.Position.y + Convert.ToSingle(0.5), Ply.CurrentRoom.Position.z + distanceZ));
										//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
									});
									player.SendCustomSync(cock.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(new Vector3(Ply.CurrentRoom.Position.x + distanceX, Ply.CurrentRoom.Position.y + Convert.ToSingle(0.5), Ply.CurrentRoom.Position.z + distanceZ));
										//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
									});
								}
							}
							else
							{
								if (player.Role.Team == Team.SCP || Ply.ReferenceHub.playerEffectsController.GetEffect<Invisible>().IsEnabled)
								{
									player.SendCustomSync(ball1.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
									player.SendCustomSync(ball2.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
									player.SendCustomSync(cock.netIdentity, typeof(Pickup), null, (writer) =>
									{
										writer.WriteUInt64(8L);
										writer.WriteVector3(Vector3.zero);
									});
								}
								else
								{
									if (Ply.Role.Type.Is939())
									{
										player.SendCustomSync(ball1.netIdentity, typeof(Pickup), null, (writer) =>
										{
											writer.WriteUInt64(8L);
											writer.WriteVector3(
												Ply.Position +
												new Vector3(-Ply.ReferenceHub.PlayerCameraReference.forward.x * 0.9f, 0, -Ply.ReferenceHub.PlayerCameraReference.forward.z * 0.9f) +
												Vector3.down * 0.7f +
												new Vector3(-Ply.ReferenceHub.PlayerCameraReference.right.x / 15f, 0, -Ply.ReferenceHub.PlayerCameraReference.right.z / 15f));


											//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
										});
										player.SendCustomSync(ball2.netIdentity, typeof(Pickup), null, (writer) =>
										{
											writer.WriteUInt64(8L);
											writer.WriteVector3(
												Ply.Position +
												new Vector3(-Ply.ReferenceHub.PlayerCameraReference.forward.x * 0.9f, 0, -Ply.ReferenceHub.PlayerCameraReference.forward.z * 0.9f) +
												Vector3.down * 0.7f +
												new Vector3(Ply.ReferenceHub.PlayerCameraReference.right.x / 15f, 0, Ply.ReferenceHub.PlayerCameraReference.right.z / 15f));
											//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
										});
										player.SendCustomSync(cock.netIdentity, typeof(Pickup), null, (writer) =>
										{
											writer.WriteUInt64(8L);
											writer.WriteVector3(
												Ply.Position +
												new Vector3(Ply.ReferenceHub.PlayerCameraReference.forward.x * -0.7f, 0, Ply.ReferenceHub.PlayerCameraReference.forward.z * -0.7f) +
												Vector3.down * 0.6f);


											//cock.Rb.transform.localEulerAngles = new Vector3(Angle207[0], Ply.GameObject.transform.localEulerAngles.y + Angle207[1], Angle207[2]);
											cock.Rb.transform.localEulerAngles = new Vector3(Angle207[0], Angle207[1], Angle207[2] - Ply.GameObject.transform.localEulerAngles.y);
										});
									}
									else
									{
										player.SendCustomSync(ball1.netIdentity, typeof(Pickup), null, (writer) =>
										{
											writer.WriteUInt64(8L);
											writer.WriteVector3(
												Ply.Position +
												new Vector3(Ply.ReferenceHub.PlayerCameraReference.forward.x / 6f, 0, Ply.ReferenceHub.PlayerCameraReference.forward.z / 6f) +
												Vector3.down * 0.25f +
												new Vector3(-Ply.ReferenceHub.PlayerCameraReference.right.x / 15f, 0, -Ply.ReferenceHub.PlayerCameraReference.right.z / 15f));


										//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
										});
										player.SendCustomSync(ball2.netIdentity, typeof(Pickup), null, (writer) =>
										{
											writer.WriteUInt64(8L);
											writer.WriteVector3(
												Ply.Position +
												new Vector3(Ply.ReferenceHub.PlayerCameraReference.forward.x / 6f, 0, Ply.ReferenceHub.PlayerCameraReference.forward.z / 6f) +
												Vector3.down * 0.25f +
												new Vector3(Ply.ReferenceHub.PlayerCameraReference.right.x / 15f, 0, Ply.ReferenceHub.PlayerCameraReference.right.z / 15f));
										//item.Rb.transform.localEulerAngles = new Vector3(Angels[0], Ply.GameObject.transform.localEulerAngles.y + Angels[1], Angels[2]);
										});
										player.SendCustomSync(cock.netIdentity, typeof(Pickup), null, (writer) =>
										{
											writer.WriteUInt64(8L);
											writer.WriteVector3(
												Ply.Position +
												new Vector3(Ply.ReferenceHub.PlayerCameraReference.forward.x / 2.5f, 0, Ply.ReferenceHub.PlayerCameraReference.forward.z / 2.5f) +
												Vector3.down * 0.1f);


										//cock.Rb.transform.localEulerAngles = new Vector3(Angle207[0], Ply.GameObject.transform.localEulerAngles.y + Angle207[1], Angle207[2]);
										cock.Rb.transform.localEulerAngles = new Vector3(Angle207[0], Angle207[1], Angle207[2] - Ply.GameObject.transform.localEulerAngles.y);
										});
									}
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
						Ply.Broadcast(6, "<i>Psst, your pet has lost you! You need to requip him again!</i>");
						NetworkServer.Destroy(ball1.gameObject);
						NetworkServer.Destroy(ball2.gameObject);
						NetworkServer.Destroy(cock.gameObject);
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
		}*/
	}
}
