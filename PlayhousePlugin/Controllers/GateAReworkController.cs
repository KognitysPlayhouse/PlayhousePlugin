using System;
using System.Linq;
using AdminToys;
using Exiled.API.Features;
using MapEditorReborn.API.Features;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;

namespace PlayhousePlugin.Controllers
{
    public class GateAReworkController
    {
        public static void Spawn()
        {
            //new ElevatorController();

            var door = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "EZ BreakableDoor"),
                new Vector3(41.492f, 987.2f, -64.254f), Quaternion.identity);
            NetworkServer.Spawn(door);

            BoxController.SpawnBoxes();
            var Elevator = MapUtils.GetSchematicDataByName("ElevatorAnimated");
            ObjectSpawner.SpawnSchematic("ElevatorAnimated",
                new Vector3(-13f, 987.2f, -65.1f), Quaternion.Euler(new Vector3(0,-90,0)), Vector3.one, Elevator);
            
            var position1 = new Vector3(-10.8f, 987.5f, -47.5f);
            
            var schematicData2 = MapUtils.GetSchematicDataByName("Stairs");
            ObjectSpawner.SpawnSchematic("Stairs",
                position1, Quaternion.identity, Vector3.one, schematicData2);

            var schematicData3 = MapUtils.GetSchematicDataByName("Stairs2");
            ObjectSpawner.SpawnSchematic("Stairs2",
                new Vector3(46.4f,987.2f,-57.15f), Quaternion.identity, Vector3.one, schematicData3);

            var GateAElevatorLight1 = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
            GateAElevatorLight1.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
            
            GateAElevatorLight1.GetComponent<LightSourceToy>().NetworkLightColor = new Color(125/255f, 125/255f, 125/255f);
            GateAElevatorLight1.GetComponent<LightSourceToy>().NetworkLightRange = 15;
            GateAElevatorLight1.GetComponent<LightSourceToy>().transform.position = new Vector3(-1.1f, 1005.5f, -37.2f);
            
            var GateAElevatorLight2 = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
            GateAElevatorLight2.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
            
            GateAElevatorLight2.GetComponent<LightSourceToy>().NetworkLightColor = new Color(125/255f, 125/255f, 125/255f);
            GateAElevatorLight2.GetComponent<LightSourceToy>().NetworkLightRange = 15;
            GateAElevatorLight2.GetComponent<LightSourceToy>().transform.position = new Vector3(-1.1f, 1005.5f, -29.5f);
            
            
            var BalconyLight = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
            BalconyLight.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
            
            BalconyLight.GetComponent<LightSourceToy>().NetworkLightColor = new Color(150/255f, 150/255f, 150/255f);
            BalconyLight.GetComponent<LightSourceToy>().NetworkLightRange = 20;
            BalconyLight.GetComponent<LightSourceToy>().transform.position = new Vector3(46.5f, 1007f, -51f);
            
            var TowerLight = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
            TowerLight.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
            
            TowerLight.GetComponent<LightSourceToy>().NetworkLightColor = new Color(150/255f, 150/255f, 150/255f);
            TowerLight.GetComponent<LightSourceToy>().NetworkLightRange = 25;
            TowerLight.GetComponent<LightSourceToy>().transform.position = new Vector3(-15, 1007f, -44f);
            
            var TowerLight1 = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
            TowerLight1.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
            
            TowerLight1.GetComponent<LightSourceToy>().NetworkLightColor = new Color(150/255f, 150/255f, 150/255f);
            TowerLight1.GetComponent<LightSourceToy>().NetworkLightRange = 25;
            TowerLight1.GetComponent<LightSourceToy>().transform.position = new Vector3(51.8f, 1007f, -72.3f);
            
        }
    }
}