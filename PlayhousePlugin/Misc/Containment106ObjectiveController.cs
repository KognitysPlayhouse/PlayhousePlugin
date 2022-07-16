using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Mirror;
using PlayhousePlugin.Components;
using UnityEngine;

namespace PlayhousePlugin
{
    public static class Containment106ObjectiveController
    {
        public static GameObject ObjectivePoint;
        public static bool Allow106Containment = false;

        public static void SpawnObjectives()
        {
            var room = Room.List.Where(x => x.Type == RoomType.Hcz106).FirstOrDefault();
            
            var g = new GameObject("objective");

            g.transform.localPosition = room.Transform.TransformPoint(new Vector3(25.7f, 2, -13.2f));
            g.transform.localRotation = room.Transform.rotation * Quaternion.Euler(new Vector3(0,90,0));
            g.AddComponent<ObjectivePointComponent>();
            
            g.GetComponent<ObjectivePointComponent>().ObjectiveCaptured += OnObjectiveCaptured;
            g.GetComponent<ObjectivePointComponent>().Radius = 5;
            g.GetComponent<ObjectivePointComponent>().AllowAllToCap = true;
            g.GetComponent<ObjectivePointComponent>().RoleToNotify = RoleType.Scp106;
            ObjectivePoint = g;
            
        }

        public static void DestroyObjectives()
        {
            ObjectivePoint.GetComponent<ObjectivePointComponent>().ObjectiveCaptured -= OnObjectiveCaptured;
            NetworkServer.Destroy(ObjectivePoint);
            
            Allow106Containment = false;
        }

        public static void OnObjectiveCaptured(object sender, GameObject g)
        {
            Allow106Containment = true;
            Cassie.Message("SCP 1 0 6 recontainment procedure INITIATED  . waiting for manual Reactivation");
        }
    }
}