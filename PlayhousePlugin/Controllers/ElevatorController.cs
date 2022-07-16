using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using Exiled.API.Features;
using MapEditorReborn.API.Features;
using MEC;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;

namespace PlayhousePlugin.Controllers
{
    public class ElevatorController
    {
        private Vector3 RootPosition = new Vector3(-13f, 987.2f, -65.1f);
        private PrimitiveObjectToy BottomDoor;
        private PrimitiveObjectToy TopDoor;
        private PrimitiveObjectToy Platform;
        private bool goingBack = false;
        public static CoroutineHandle CoroutineHandle;

        public ElevatorController()
        {
            var Elevator = MapUtils.GetSchematicDataByName("Elevator");
            ObjectSpawner.SpawnSchematic("Elevator",
                RootPosition, Quaternion.Euler(new Vector3(0,-90,0)), Vector3.one, Elevator);

            for (int i = 0; i < 5; i++)
            {
                var SpotLight = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
                SpotLight.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
				
                SpotLight.GetComponent<LightSourceToy>().NetworkLightColor = new Color(200/255f, 128/255f,128/255f);
                SpotLight.GetComponent<LightSourceToy>().NetworkLightRange = 5;
                SpotLight.GetComponent<LightSourceToy>().transform.position =  RootPosition + Vector3.forward*2f + Vector3.up*4.5f*(i+1);
            }
            
            // Platform
            Platform = UnityEngine.Object.Instantiate(Utils.PrimitiveBaseObject);
            Platform.NetworkPrimitiveType = PrimitiveType.Cube;
            Platform.NetworkMaterialColor = new Color( 218/255f, 218/255f, 218/255f, 255/255f);
            Platform.NetworkMovementSmoothing = 60;
            Platform.transform.position = RootPosition + Vector3.up * 0.2f;
            Platform.transform.localScale = new Vector3(2.99f,0.4f,5.99f);
            Platform.transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
            
            NetworkServer.Spawn(Platform.gameObject);
            Platform.UpdatePositionServer();

            // Bottom Door
            BottomDoor = UnityEngine.Object.Instantiate(Utils.PrimitiveBaseObject);
            BottomDoor.NetworkPrimitiveType = PrimitiveType.Cube;
            BottomDoor.NetworkMaterialColor = new Color( 111/255f, 11/255f, 11/255f, 255/255f);
            BottomDoor.NetworkMovementSmoothing = 60;
            BottomDoor.transform.position = RootPosition + Vector3.up * 1.5f + Vector3.forward*1.6f;
            BottomDoor.transform.localScale = new Vector3(6f,3.2f,0.1f);
            BottomDoor.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
            
            NetworkServer.Spawn(BottomDoor.gameObject);
            BottomDoor.UpdatePositionServer();
            
            // Top Door
            TopDoor = UnityEngine.Object.Instantiate(Utils.PrimitiveBaseObject);
            TopDoor.NetworkPrimitiveType = PrimitiveType.Cube;
            TopDoor.NetworkMaterialColor = new Color( 111/255f, 11/255f, 11/255f, 255/255f);
            TopDoor.NetworkMovementSmoothing = 60;
            TopDoor.transform.position = RootPosition + Vector3.up * 16f + Vector3.forward*-1.6f;
            TopDoor.transform.localScale = new Vector3(6f,6f,0.1f);
            TopDoor.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
            
            NetworkServer.Spawn(TopDoor.gameObject);
            TopDoor.UpdatePositionServer();
            
            CoroutineHandle = Timing.RunCoroutine(Move());
        }

        public IEnumerator<float> Move()
        {
            yield return Timing.WaitForSeconds(1);
            while (true)
            {
                yield return Timing.WaitForOneFrame;
                Platform.transform.position = Vector3.MoveTowards(Platform.transform.position, this.goingBack ? RootPosition : RootPosition+Vector3.up*14, Time.deltaTime*2.25f);
                
                if (Vector3.Distance(Platform.transform.position, goingBack ? RootPosition : RootPosition+Vector3.up*14) <= 0f)
                {
                    if (!goingBack) // Going up
                    {
                        Timing.RunCoroutine(OpenTopDoor());
                        yield return Timing.WaitForSeconds(4); // Waiting to open the door
                        yield return Timing.WaitForSeconds(5); // Waiting for people to go out or in
                        
                        Timing.RunCoroutine(CloseTopDoor());
                        yield return Timing.WaitForSeconds(4); // Waiting to close the door
                    }
                    else
                    {
                        Timing.RunCoroutine(OpenBottomDoor());
                        yield return Timing.WaitForSeconds(4); // Waiting to open the door
                        yield return Timing.WaitForSeconds(5); // Waiting for people to go out or in

                        Timing.RunCoroutine(CloseBottomDoor());
                        yield return Timing.WaitForSeconds(4); // Waiting to close the door
                    }


                    goingBack = !goingBack;
                }
            }
        }

        public IEnumerator<float> CloseTopDoor()
        {
            for (int i = 0; i < 30; i++)
            {
                yield return Timing.WaitForSeconds(0.125f);
                TopDoor.transform.position += Vector3.up*0.2f;
            }
        }
        
        public IEnumerator<float> OpenTopDoor()
        {
            for (int i = 0; i < 30; i++)
            {
                yield return Timing.WaitForSeconds(0.125f);
                TopDoor.transform.position += Vector3.down*0.2f;
            }
        }
        
        public IEnumerator<float> CloseBottomDoor()
        {
            for (int i = 0; i < 30; i++)
            {
                yield return Timing.WaitForSeconds(0.125f);
                BottomDoor.transform.position += Vector3.up*0.1f;
            }
        }
        
        public IEnumerator<float> OpenBottomDoor()
        {
            for (int i = 0; i < 30; i++)
            {
                yield return Timing.WaitForSeconds(0.125f);
                BottomDoor.transform.position += Vector3.down*0.1f;
            }
        }
    }
}