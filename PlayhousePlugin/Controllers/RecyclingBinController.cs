using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapEditorReborn.API.Features;
using Mirror;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace PlayhousePlugin.Controllers
{
    public class RecyclingBinInfo
    {
        public RoomType RoomType;
        public List<Vector3> Positions;
        public Room Room;
    }
    public class RecyclingBinController : MonoBehaviour
    {
        private float _timer = 0;
        List<ItemPickupBase> DeletedItems = new List<ItemPickupBase>();
        public static void SpawnRecyclingBins()
        {
            // 1 Bin in LCZ (And 1 fake one in 173's room)
            // 1 Bin in HCZ
            // 2 Bins in EZ ( 1 in gate)

            List<RecyclingBinInfo> AvailableRoomsLCZ =
                new List<RecyclingBinInfo>();
            
            List<RecyclingBinInfo> AvailableRoomsHCZ =
                new List<RecyclingBinInfo>();
            
            List<RecyclingBinInfo> AvailableRoomsEZ =
                new List<RecyclingBinInfo>();
            
            List<RecyclingBinInfo> Gates =
                new List<RecyclingBinInfo>();
            
            // Loop through all rooms in LCZ and if the room type is in the EZRecyclingBinLocations dictionary add it to the AvailableRoomsLCZ list
            foreach (var room in Room.List)
            {
                if (room.Zone == ZoneType.LightContainment)
                {
                    // Check if the room type is in the EZRecyclingBinLocations dictionary and if the room type isn't already in the AvailableRoomsEZ list
                    if (Utils.LCZRecyclingBinLocations.ContainsKey(room.Type) && !AvailableRoomsLCZ.Exists(x => x.RoomType == room.Type))
                    {
                        AvailableRoomsLCZ.Add(new RecyclingBinInfo
                        {
                            RoomType = room.Type,
                            Positions = Utils.LCZRecyclingBinLocations[room.Type],
                            Room = room
                        });
                    }
                }
                else if (room.Zone == ZoneType.HeavyContainment)
                {
                    // Check if the room type is in the HZRecyclingBinLocations dictionary and if the room type isn't already in the AvailableRoomsHCZ list
                    if (Utils.HCZRecyclingBinLocations.ContainsKey(room.Type) && !AvailableRoomsHCZ.Exists(x => x.RoomType == room.Type))
                    {
                        AvailableRoomsHCZ.Add(new RecyclingBinInfo
                        {
                            RoomType = room.Type,
                            Positions = Utils.HCZRecyclingBinLocations[room.Type],
                            Room = room
                        });
                    }
                }
                else if (room.Zone == ZoneType.Entrance)
                {
                    // Check if the room type is in the EZRecyclingBinLocations dictionary and if the room type isn't already in the AvailableRoomsEZ list
                    if (Utils.EZRecyclingBinLocations.ContainsKey(room.Type) && !AvailableRoomsEZ.Exists(x => x.RoomType == room.Type))
                    {
                        AvailableRoomsEZ.Add(new RecyclingBinInfo
                        {
                            RoomType = room.Type,
                            Positions = Utils.EZRecyclingBinLocations[room.Type],
                            Room = room
                        });
                    }

                    if (Utils.GateRecyclingBinLocations.ContainsKey(room.Type))
                    {
                        Gates.Add(new RecyclingBinInfo()
                        {
                            RoomType = room.Type,
                            Positions = Utils.GateRecyclingBinLocations[room.Type],
                            Room = room
                        });
                    }  
                }
            }
            
            // Pick 1 room in AvailableRoomsLCZ and spawn a recycling bin in it
            while (AvailableRoomsLCZ.Count != 2)
            {
                var randomRoom = AvailableRoomsLCZ.PickRandom();
                var randomPosition = randomRoom.Positions.PickRandom();

                AvailableRoomsLCZ.Remove(randomRoom);

                if (randomRoom.Room.Type == RoomType.Lcz330)
                {
                    var binObj = ObjectSpawner.SpawnSchematic("TrashWithLight",
                        randomRoom.Room.transform.TransformPoint(randomPosition),
                        Quaternion.Euler(0,
                            Quaternion.LookRotation((randomRoom.Room.Position -
                                                     randomRoom.Room.transform.TransformPoint(randomPosition))
                                .normalized).eulerAngles.y+180, 0), Vector3.one);
                    
                    binObj.gameObject.AddComponent<RecyclingBinController>();
                }
                else
                {
                    var binObj = ObjectSpawner.SpawnSchematic("TrashWithLight",
                        randomRoom.Room.transform.TransformPoint(randomPosition),
                        Quaternion.Euler(0,
                            Quaternion.LookRotation((randomRoom.Room.Position -
                                                     randomRoom.Room.transform.TransformPoint(randomPosition))
                                .normalized).eulerAngles.y, 0), Vector3.one);
                    
                    binObj.gameObject.AddComponent<RecyclingBinController>();
                }

                
            }
            
            // Pick 1 room in AvailableRoomsHCZ and spawn a recycling bin in it
            if (AvailableRoomsHCZ.Count > 0)
            {
                var randomRoom = AvailableRoomsHCZ.PickRandom();
                var randomPosition = randomRoom.Positions.PickRandom();
                
                var binObj = ObjectSpawner.SpawnSchematic("TrashWithLight",
                    randomRoom.Room.transform.TransformPoint(randomPosition),
                    Quaternion.Euler(0, Quaternion.LookRotation((randomRoom.Room.Position - randomRoom.Room.transform.TransformPoint(randomPosition)).normalized).eulerAngles.y, 0), Vector3.one);
                
                binObj.gameObject.AddComponent<RecyclingBinController>();
            }
            
            // Pick 1 room in AvailableRoomsEZ that isn't a gate and spawn a recycling bin in it and spawn a recycling bin in Gate A or Gate B
            if (AvailableRoomsEZ.Count > 0)
            {
                var randomRoom = AvailableRoomsEZ.PickRandom();
                var randomPosition = randomRoom.Positions.PickRandom();
                
                var binObj = ObjectSpawner.SpawnSchematic("TrashWithLight",
                    randomRoom.Room.transform.TransformPoint(randomPosition),
                    Quaternion.Euler(0, Quaternion.LookRotation((randomRoom.Room.Position - randomRoom.Room.transform.TransformPoint(randomPosition)).normalized).eulerAngles.y, 0), Vector3.one);
                    
                binObj.gameObject.AddComponent<RecyclingBinController>();

                if(UnityEngine.Random.Range(0, 1) == 0)
                {
                    var gate = Gates.FirstOrDefault(x => x.RoomType == RoomType.EzGateA);
                    var pos = gate.Positions.PickRandom();
                    
                    var gateBinObj = ObjectSpawner.SpawnSchematic("TrashWithLight",
                        gate.Room.transform.TransformPoint(pos),
                        Quaternion.Euler(0, Quaternion.LookRotation((gate.Room.Position - gate.Room.transform.TransformPoint(pos)).normalized).eulerAngles.y, 0), Vector3.one);
                    
                    gateBinObj.gameObject.AddComponent<RecyclingBinController>();
                }
                else
                {
                    var gate = Gates.FirstOrDefault(x => x.RoomType == RoomType.EzGateB);
                    var pos = gate.Positions.PickRandom();
                    
                    var gateBinObj = ObjectSpawner.SpawnSchematic("TrashWithLight",
                        gate.Room.transform.TransformPoint(pos),
                        Quaternion.Euler(0, Quaternion.LookRotation((gate.Room.Position - gate.Room.transform.TransformPoint(pos)).normalized).eulerAngles.y, 0), Vector3.one);
                    
                    gateBinObj.gameObject.AddComponent<RecyclingBinController>();
                }
            }
            
            // Spawning a fake bin in the 173's room
            var room173 = Room.List.FirstOrDefault(x => x.Type == RoomType.Lcz173);
            ObjectSpawner.SpawnSchematic("Trash",
                room173.transform.TransformPoint(new Vector3(8, 19.7f, 21.9f)),
                Quaternion.Euler(0,
                    Mathf.Round(Quaternion.LookRotation((room173.Position - room173.transform.TransformPoint(new Vector3(8, 19.7f, 21.9f))).normalized).eulerAngles.y/90)*90, 0),
                Vector3.one);
        }

        private void FixedUpdate()
        {
            if (_timer < 1)
            {
                _timer += Time.deltaTime;
                return;
            }
            
            var colliders = Physics.OverlapSphere(gameObject.transform.position, 1.1f, LayerMask.GetMask("Pickup"));
            foreach (var col in colliders)
            {
                if(col.transform.root.gameObject.TryGetComponent(out ItemPickupBase pickup))
                {
                    if(DeletedItems.Contains(pickup))
                        continue;
                    
                    if (!pickup.NetworkInfo.ItemId.IsAmmo())
                    {
                        Item.Create(ItemType.Coin).Spawn(gameObject.transform.position + gameObject.transform.forward * 2 + Vector3.up);
                        pickup.DestroySelf();
                    }
                    else
                    {
                        pickup.DestroySelf();
                    }
                    
                    DeletedItems.Add(pickup);
                }
            }
            
            DeletedItems.Clear();
            
            /*
            foreach (var pickup in Map.Pickups)
            {
                if (Vector3.Distance(gameObject.transform.position, pickup.Position) <= 1.1)
                {
                    itemsToDelete.Add(pickup);
                }
            }

            foreach (var pickup in itemsToDelete)
            {
                if (!pickup.Type.IsAmmo())
                {
                    (Item)Item.Create(ItemType.Coin).Spawn(gameObject.transform.position + gameObject.transform.forward * 2 + Vector3.up);
                    pickup.Destroy();
                }
                else
                {
                    pickup.Destroy();
                }
            }
            
            itemsToDelete.Clear();*/
            _timer = 0;
        }
    }
}