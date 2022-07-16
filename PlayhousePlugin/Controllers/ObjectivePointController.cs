using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Mirror;
using PlayhousePlugin.Components;
using Respawning;
using UnityEngine;

namespace PlayhousePlugin
{
    public static class ObjectivePointController
    {
        private static int NumOfObjectives = 6;
        public static int objectivesCapped = 0;
        public static List<GameObject> ObjectivePoints = new List<GameObject>();
        public static bool RapidSpawnWaves = false;
        public static SpawnableTeamType Team = SpawnableTeamType.None;
        public static bool FailedObjectives = false;
        public static bool DisableElevators = false;

        public static void SpawnObjectives()
        {
            List<Room> LocationsPicked = new List<Room>();
            
            Dictionary<Room, List<Utils.PosRot>> LocationsHeavy = new Dictionary<Room, List<Utils.PosRot>>();
            Dictionary<Room, List<Utils.PosRot>> LocationsEntrance = new Dictionary<Room, List<Utils.PosRot>>();

            foreach (var room in Room.List)
            {
                if (Utils.ObjectivePointLocations.ContainsKey(room.Type))
                {
                    if (room.Zone == ZoneType.Entrance)
                    {
                        if (!LocationsEntrance.ContainsKey(room))
                            LocationsEntrance.Add(room, Utils.ObjectivePointLocations[room.Type]);

                    }
                    else if (room.Zone == ZoneType.HeavyContainment)
                    {
                        if (!LocationsHeavy.ContainsKey(room))
                            LocationsHeavy.Add(room, Utils.ObjectivePointLocations[room.Type]);

                    }
                }
            }

            while (ObjectivePoints.Count != 2)
            {
                var value = LocationsEntrance.ElementAt(EventHandler.random.Next(0, LocationsEntrance.Count));
                if (LocationsPicked.Contains(value.Key))
                    continue;
                
                LocationsEntrance.Remove(value.Key);
                LocationsPicked.Add(value.Key);
                var g = new GameObject("objective");

                var posRot = value.Value.PickRandom();
                    
                g.transform.localPosition = value.Key.Transform.TransformPoint(posRot.Pos);
                g.transform.localRotation = value.Key.Transform.rotation * Quaternion.Euler(posRot.Rot);
                g.AddComponent<ObjectivePointComponent>();

                g.GetComponent<ObjectivePointComponent>().ObjectiveCaptured += OnObjectiveCaptured;
                ObjectivePoints.Add(g);
            }
            
            while (ObjectivePoints.Count != 6)
            {
                var value = LocationsHeavy.ElementAt(EventHandler.random.Next(0, LocationsHeavy.Count));
                if (LocationsPicked.Contains(value.Key))
                    continue;
                
                LocationsHeavy.Remove(value.Key);
                LocationsPicked.Add(value.Key);
                var g = new GameObject("objective");

                var posRot = value.Value.PickRandom();
                    
                g.transform.localPosition = value.Key.Transform.TransformPoint(posRot.Pos);
                g.transform.localRotation = value.Key.Transform.rotation * Quaternion.Euler(posRot.Rot);
                g.AddComponent<ObjectivePointComponent>();

                g.GetComponent<ObjectivePointComponent>().ObjectiveCaptured += OnObjectiveCaptured;
                ObjectivePoints.Add(g);
            }
        }

        public static void DestroyObjectives()
        {
            foreach (var Objective in ObjectivePoints)
            {
                Objective.GetComponent<ObjectivePointComponent>().ObjectiveCaptured -= OnObjectiveCaptured;
                NetworkServer.Destroy(Objective);
            }
            
            ObjectivePoints.Clear();
            objectivesCapped = 0;
            RapidSpawnWaves = false;
            FailedObjectives = false;
            DisableElevators = false;
            Team = SpawnableTeamType.None;
        }

        public static void OnObjectiveCaptured(object sender, GameObject g)
        {
            objectivesCapped += 1;
            if (objectivesCapped == 6)
            {
                Cassie.Message($"6 of 6 decontamination terminals online all terminals have been successfully engaged . completing decontamination sequence . Heavy and Entrance Zone will proceed with Decontamination at 18 minutes");
            }
            else
                Cassie.Message($"{objectivesCapped} of 6 decontamination terminals online");
        }
    }
}