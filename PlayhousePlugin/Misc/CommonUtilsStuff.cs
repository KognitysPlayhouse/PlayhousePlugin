using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using Exiled.API.Enums;
using Respawning;
using UnityEngine;

namespace PlayhousePlugin
{
	class CommonUtilsStuff
	{
		public static bool AutoNukeDetonated = false;
		public static IEnumerator<float> AutoNuke()
		{
			yield return Timing.WaitForSeconds(10);

			foreach (Room room in Room.List)
			{
				room.FlickerableLightController.WarheadLightOverride = true;
				room.TurnOffLights(2f);
			}
			
			Timing.CallDelayed(5, () =>
			{
				foreach (var room in Room.List)
				{
					room.ResetColor();
				}
			});

			Cassie.Message("pitch_0.3 .g5 .g5 pitch_0.95 alert alert . facility will attempt a site wide decontamination . All decontamination terminals must be enabled in 18 minutes pitch_0.3 .g5 .g5", true, false);
			foreach (Player Ply in Player.List)
			{
				Ply.ShowCenterDownHint("<color=red>The facility will try to decontaminate in 19 minutes.\nAll terminals must be active before 18 minutes!</color>", 10);
			}
			// 13 minutes delay
			yield return Timing.WaitForSeconds(780);
	
			// 13 Minutes into the round	
			foreach (Room room in Room.List)
			{
				room.FlickerableLightController.WarheadLightOverride = true;
				room.TurnOffLights(1f);
			}

			Timing.CallDelayed(3, () =>
			{
				foreach (var room in Room.List)
				{
					room.ResetColor();
				}
			});

			Map.Broadcast(6, "<color=red><b><i>Attempted Decontamination in 5 Minutes</i></b></color>");
			Cassie.Message("pitch_0.6 .g6 .g6 pitch_0.95 Danger Danger . Pitch_0.95 attempted facility decontamination will start in 5 minutes pitch_0.6 .g6 .g6", true, false);

			// 5 minutes delay
			yield return Timing.WaitForSeconds(300);
			
			// 18 Minutes into the round
			foreach (Room room in Room.List)
			{
				room.FlickerableLightController.WarheadLightOverride = true;
				room.TurnOffLights(1f);
			}

			Timing.CallDelayed(3, () =>
			{
				foreach (var room in Room.List)
				{
					room.ResetColor();
				}
			});

			if (ObjectivePointController.objectivesCapped != 6)
			{
				// Chaos Advantage
				ObjectivePointController.FailedObjectives = true;
				Map.Broadcast(6, "<color=red><b><i>Failed to prepare for decontamination. Site Wide Decontamination Protocol Failure.</i></b></color>");
				Cassie.Message("pitch_0.6 .g6 .g6 pitch_0.95 site wide decontamination protocol failure . information systems offline . radio systems offline . warhead system failure .  detonation in 7 minutes pitch_0.6 .g6 .g6", true, false);
				
				ObjectivePointController.RapidSpawnWaves = true;
				Respawn.ForceWave(SpawnableTeamType.ChaosInsurgency);
				RespawnManager.Singleton.NextKnownTeam = SpawnableTeamType.ChaosInsurgency;
				RespawnManager.Singleton._timeForNextSequence = 60;
				ObjectivePointController.Team = SpawnableTeamType.ChaosInsurgency;
			}
			else
			{
				// MTF Advantage
				ObjectivePointController.FailedObjectives = false;
				Map.Broadcast(6, "<color=red><b><i>Decontamination in 1 Minute!</i></b></color>");
				Cassie.Message("pitch_0.6 .g6 .g6 pitch_0.85 Danger Danger . facility decontamination in 1 minute . evacuate now . pitch_0.2 .g4 yd_2.5 .g4 yd_2.5 .g4", true, false);			
				
				ObjectivePointController.RapidSpawnWaves = true;
				Respawn.ForceWave(SpawnableTeamType.NineTailedFox);
				RespawnManager.Singleton.NextKnownTeam = SpawnableTeamType.NineTailedFox;
				RespawnManager.Singleton._timeForNextSequence = 60;
				ObjectivePointController.Team = SpawnableTeamType.NineTailedFox;
			}


			if (!ObjectivePointController.FailedObjectives) // MTF Decon
			{
				yield return Timing.WaitForSeconds(30);
				//Cassie.Message("30 Seconds", true, false);
				EventHandler.coroutines.Add(Timing.RunCoroutine(FadeToYellow()));
				Map.Broadcast(6, "<color=red><b><i>30 Seconds to decontamination</i></b></color>");
				
				yield return Timing.WaitForSeconds(10);
				//Cassie.Message("20 Seconds", true, false);
				Map.Broadcast(6, "<color=red><b><i>20 Seconds to decontamination</i></b></color>");
				
				yield return Timing.WaitForSeconds(10);
				//Cassie.Message("10 yd_1 9 yd_1 8 yd_1 7 yd_1 6 yd_1 5 yd_1 4 yd_1 3 yd_1 2 yd_1 1", true, false);
				Map.Broadcast(1, "<color=red><b><i>10 Seconds to decontamination</i></b></color>");
				Map.Broadcast(1, "<color=red><b><i>9 Seconds to decontamination</i></b></color>");
				Map.Broadcast(1, "<color=red><b><i>8 Seconds to decontamination</i></b></color>");
				Map.Broadcast(1, "<color=red><b><i>7 Seconds to decontamination</i></b></color>");
				Map.Broadcast(1, "<color=red><b><i>6 Seconds to decontamination</i></b></color>");
				Map.Broadcast(1, "<color=red><b><i>5 Seconds to decontamination</i></b></color>");
				Map.Broadcast(1, "<color=red><b><i>4 Seconds to decontamination</i></b></color>");
				Map.Broadcast(1, "<color=red><b><i>3 Seconds to decontamination</i></b></color>");
				Map.Broadcast(1, "<color=red><b><i>2 Seconds to decontamination</i></b></color>");
				Map.Broadcast(1, "<color=red><b><i>1 Seconds to decontamination</i></b></color>");
				Map.Broadcast(1, "<color=red><b><i>Decontamination has started</i></b></color>");
				
				yield return Timing.WaitForSeconds(10);
				EventHandler.coroutines.Add(Timing.RunCoroutine(KillPlayers()));
				ObjectivePointController.DisableElevators = true;
			}
			else // Autonuke 
			{
				yield return Timing.WaitForSeconds(60);
				yield return Timing.WaitForSeconds(360);
				if (!Warhead.IsDetonated)
				{
					if (Warhead.IsInProgress)
					{
						Warhead.IsLocked = true;
						AutoNukeDetonated = true;
						Map.Broadcast(10, "<color=red><b><i>Auto Nuke started it can't be turned off!\nESCAPE THE FACILITY!!</i></b></color>");
					}
					else
					{
						Warhead.Start();
						Warhead.IsLocked = true;
						AutoNukeDetonated = true;
						Map.Broadcast(10, "<color=red><b><i>Auto Nuke started it can't be turned off!\nESCAPE THE FACILITY!!</i></b></color>");
					}

				}
				foreach (var x in Door.List)
				{
					x.BreakDoor();
					yield return Timing.WaitForSeconds(0.25f);
				}
			}
		}

		public static IEnumerator<float> FadeToYellow()
		{
			var ColorVal = new Color(325f / 255f, 325f / 255f, 0);
			float fadeAmount = 0;
			float fadeDuration = 0.01f;
			bool doorOpened = false;

			for (int i = 0; i < 300; i++)
			{
				fadeAmount += Time.deltaTime * fadeDuration;
				foreach (var room in Room.List)
				{
					room.Color = Color.Lerp(room.Color,ColorVal, fadeAmount);
					if (room.Zone != ZoneType.Surface && room.Zone != ZoneType.LightContainment)
					{
						if (!doorOpened)
						{
							foreach (var door in room.Doors)
							{
								door.IsOpen = true;
								door.ChangeLock(DoorLockType.Warhead);
							}
						}
					}
				}

				doorOpened = true;
				yield return Timing.WaitForSeconds(0.1f);
			}
		}

		public static IEnumerator<float> KillPlayers()
		{
			for (int i = 0; i < 60; i++)
			{
				yield return Timing.WaitForSeconds(1);
				foreach (var player in Player.List)
				{
					if (player.Role.Type == RoleType.Scp079) continue;
					if (player.ReferenceHub.transform.position.y < 800)
					{
						player.Hurt((int)player.MaxHealth*0.1f, "Decontamination");
						Log.Info("DECONTAMINATING");
					}

					if (player.Role.Type == RoleType.Scp106)
					{
						player.ReferenceHub.scp106PlayerScript.NetworkportalPosition = Vector3.zero;
					}
				}
			}
			
			foreach (var player in Player.List)
			{
				if (player.Role.Type == RoleType.Scp079) continue;
				if (player.ReferenceHub.transform.position.y < 800)
				{
					player.Hurt(int.MaxValue, "Please die already");
					Log.Info("DECONTAMINATING");
				}

				if (player.Role.Type == RoleType.Scp106)
				{
					player.ReferenceHub.scp106PlayerScript.NetworkportalPosition = Vector3.zero;
				}
			}
		}
	}
}
