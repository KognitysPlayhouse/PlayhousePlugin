using System;
using System.Linq;
using AdminToys;
using Exiled.API.Features;
using MapEditorReborn.API.Enums;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.API.Features;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;

namespace PlayhousePlugin.Controllers
{
    public class GateBReworkController
    {
        public static void Spawn()
        {
            var positionB = new Vector3(146.7f, 992.4f, -59.2f);
            var schematicData1 = MapUtils.GetSchematicDataByName("GateB");
            var mapEditorObject1 = ObjectSpawner.SpawnSchematic("GateB",
                positionB, Quaternion.identity, Vector3.one, schematicData1);
            
           var truck = ObjectSpawner.SpawnSchematic("Truck_Crash",
                positionB, Quaternion.identity, Vector3.one);

           foreach (var block in truck.AttachedBlocks)
           {
               if (block.name.Contains("Light"))
               {
                   var component = block.gameObject.AddComponent<Components.LightBlink>();
                   component.maxIntensityDecreaseMultiplier = 0.7f;
                   component.maxFlickerTimeRange = 0.3f;
                   component.offset = 0.1f;
               }
           }

            var positionWorkstation = new Vector3(116.1f, 987.2f, -66.8f);
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ObjectType.WorkStation.GetObjectByMode(), positionWorkstation,
                Quaternion.identity);
            NetworkServer.Spawn(gameObject);
            
            var Root = new Vector3(115f, 991.4f, -65.4f);
            for (int i = 0; i < 3; i++)
            {
                var SpotLight = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
                SpotLight.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
            
                SpotLight.GetComponent<LightSourceToy>().NetworkLightColor = new Color(254/255f, 255/255f, 232/255f);
                SpotLight.GetComponent<LightSourceToy>().NetworkLightRange = 15;
                SpotLight.GetComponent<LightSourceToy>().transform.position = new Vector3(Root.x-(i*15), Root.y, Root.z);
            }
           
            var ElevatorLight = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
            ElevatorLight.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
            
            ElevatorLight.GetComponent<LightSourceToy>().NetworkLightColor = new Color(204/255f, 205/255f, 182/255f);
            ElevatorLight.GetComponent<LightSourceToy>().NetworkLightRange = 10;
            ElevatorLight.GetComponent<LightSourceToy>().transform.position = new Vector3(87.4f, 996.4f, -47.8f);
            
            var SecondaryLight = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
            SecondaryLight.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
            
            SecondaryLight.GetComponent<LightSourceToy>().NetworkLightColor = new Color(254/255f, 255/255f, 232/255f);
            SecondaryLight.GetComponent<LightSourceToy>().NetworkLightRange = 20;
            SecondaryLight.GetComponent<LightSourceToy>().transform.position = new Vector3(76.2f, 994.3f, -48.6f);
            
            /*
            var RoomLight = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
            RoomLight.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
            
            RoomLight.GetComponent<LightSourceToy>().NetworkLightColor = new Color(254/255f, 255/255f, 232/255f);
            //RoomLight.GetComponent<LightSourceToy>().NetworkLightRange = 20;
            RoomLight.GetComponent<LightSourceToy>().transform.position = new Vector3(86.8f, 996.1f, -57.1f);
            */
            
            var positionC = new Vector3(164.5f, 992.4f, -59.2f);
            var schematicData2 = MapUtils.GetSchematicDataByName("FootBridge");
            ObjectSpawner.SpawnSchematic("FootBridge",
                positionC, Quaternion.identity, Vector3.one, schematicData2);
        }
    }
}