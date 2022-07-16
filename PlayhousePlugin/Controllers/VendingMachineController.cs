using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables.Scp330;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using Mirror;
using PlayhousePlugin.CustomGameMode;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

namespace PlayhousePlugin.Controllers
{
    public class VendingMachineController : MonoBehaviour
    {
        public enum VendingMachineType
        {
            EntranceZone,
            LightContainmentZone
        }

        private Dictionary<int, bool[]> _digitsMap = new Dictionary<int, bool[]>
        {
            { 0, new[]{true, true, true, false, true, true, true} },
            { 1, new[]{false, true, false, false, true, false, false} },
            { 2, new[]{true, false, true, true, true, false, true} },
            { 3, new[]{true, true, false, true, true, false, true} },
            { 4, new[]{false, true, false, true, true, true, false} },
            { 5, new[]{true, true, false, true, false, true, true} },
            { 6, new[]{true, true, true, true, false, true, true} },
            { 7, new[]{false, true, false, false, true, false, true} },
            { 8, new[]{true, true, true, true, true, true, true} },
            { 9, new[]{true, true, false, true, true, true, true} }
        };

        private class DigitDisplay
        {
            public List<PrimitiveObject> Segments = new List<PrimitiveObject>();
        }

        private class VendingMachineItem
        {
            public int RequiredCoins;
            public ItemType ItemType;
            public string ItemName;
        }

        private readonly Vector3 _basePosition = new Vector3(-1.108f, 1.4145f, 0.469f);
        private Vector3 _refundPosition = new Vector3(-1.16f, 1.8215f, 0.469f);
        private float step = 0.125f;
        private int _coins = 0;
        private string _display = "0000";
        private float _timer;
        private List<PrimitiveObject> _digits = new List<PrimitiveObject>();
        private List<DigitDisplay> _digitDisplays = new List<DigitDisplay>();
        private List<ItemPickupBase> DeletedItems = new List<ItemPickupBase>();
        private List<LightSourceObject> _lightSources = new List<LightSourceObject>();

        private List<VendingMachineItem> _vendingMachineItems = new List<VendingMachineItem>
        {
            new VendingMachineItem { RequiredCoins = 1, ItemType = ItemType.Painkillers },
            new VendingMachineItem { RequiredCoins = 1, ItemType = ItemType.None, ItemName = "M&Ms" },
            new VendingMachineItem { RequiredCoins = 1, ItemType = ItemType.None, ItemName = "Layz"},
            new VendingMachineItem { RequiredCoins = 1, ItemType = ItemType.None, ItemName = "Conk"},
            new VendingMachineItem { RequiredCoins = 2, ItemType = ItemType.Medkit },
            new VendingMachineItem { RequiredCoins = 1, ItemType = ItemType.None, ItemName = "Bepis"},
            new VendingMachineItem { RequiredCoins = 2, ItemType = ItemType.Ammo9x19 },
            new VendingMachineItem { RequiredCoins = 4, ItemType = ItemType.None, ItemName = "MysteryBox"},
            new VendingMachineItem { RequiredCoins = 2, ItemType = ItemType.Ammo556x45 },
        };

        private List<ItemType> MysteryBoxLootEZ = new List<ItemType>()
        {
            ItemType.GrenadeFlash,
            ItemType.Adrenaline,
            ItemType.SCP2176,
            ItemType.SCP500,
            
            // EZ Specific
            ItemType.KeycardNTFOfficer,
            ItemType.ArmorHeavy,
            
        };

        private List<ItemType> MysteryBoxLootLCZ = new List<ItemType>()
        {
            ItemType.GrenadeFlash,
            ItemType.Adrenaline,
            ItemType.SCP2176,
            ItemType.SCP500,
            
            // LCZ Specific
            ItemType.GunRevolver,
            ItemType.KeycardZoneManager,
            ItemType.KeycardResearchCoordinator,
        };

        public VendingMachineType vendingMachineType { get; set; }
        public List<Pickup> Buttons { get; set; }
        public static List<VendingMachineController> VendingMachines = new List<VendingMachineController>();

        public static void SpawnVendingMachines()
        {
            // Spawn Vending Machines
            // 2 in LCZ, 2 EZ
            
            List<Room> LocationsPicked = new List<Room>();
            
            Dictionary<Room, List<Utils.PosRot>> LocationsLight = new Dictionary<Room, List<Utils.PosRot>>();
            Dictionary<Room, List<Utils.PosRot>> LocationsEntrance = new Dictionary<Room, List<Utils.PosRot>>();

            foreach (var room in Room.List)
            {
                if (Utils.LCZVendingLocations.ContainsKey(room.Type))
                {
                    if (!LocationsLight.ContainsKey(room))
                        LocationsLight.Add(room, Utils.LCZVendingLocations[room.Type]);
                }
                else if (Utils.EZVendingLocations.ContainsKey(room.Type))
                {
                    if (!LocationsEntrance.ContainsKey(room))
                        LocationsEntrance.Add(room, Utils.EZVendingLocations[room.Type]);
                }
            }
            
            while(VendingMachines.Count != 2)
            {
                var value = LocationsEntrance.ElementAt(EventHandler.random.Next(0, LocationsEntrance.Count));
                if (LocationsPicked.Contains(value.Key))
                    continue;

                LocationsEntrance.Remove(value.Key);
                LocationsPicked.Add(value.Key);

                var posRot = value.Value.PickRandom();
                
                var vendingMachine = MapUtils.GetSchematicDataByName("Vending_Machine");
                var vendingMachineObj = ObjectSpawner.SpawnSchematic("Vending_Machine",
                    value.Key.Transform.TransformPoint(posRot.Pos),
                    value.Key.Transform.rotation * Quaternion.Euler(posRot.Rot), Vector3.one, vendingMachine);
                
                vendingMachineObj.gameObject.AddComponent<VendingMachineController>().Init(vendingMachineObj);
                VendingMachines.Add(vendingMachineObj.GetComponent<VendingMachineController>());
                Log.Info($"Spawned vending machine in {value.Key}");
            }
            
            while(VendingMachines.Count != 4)
            {
                var value = LocationsLight.ElementAt(EventHandler.random.Next(0, LocationsLight.Count));
                if (LocationsPicked.Contains(value.Key))
                    continue;
                
                LocationsLight.Remove(value.Key);
                LocationsPicked.Add(value.Key);

                var posRot = value.Value.PickRandom();
                    
                var vendingMachine = MapUtils.GetSchematicDataByName("Vending_Machine");
                var vendingMachineObj = ObjectSpawner.SpawnSchematic("Vending_Machine",
                    value.Key.Transform.TransformPoint(posRot.Pos),
                    value.Key.Transform.rotation * Quaternion.Euler(posRot.Rot), Vector3.one, vendingMachine);

                vendingMachineObj.gameObject.AddComponent<VendingMachineController>().Init(vendingMachineObj);
                VendingMachines.Add(vendingMachineObj.GetComponent<VendingMachineController>());
                Log.Info($"Spawned vending machine in {value.Key}");
            }
        }
        
        public void Init(SchematicObject obj)
        {
            Exiled.Events.Handlers.Player.PickingUpArmor += OnPickingUpArmor;
            Buttons = new List<Pickup>();
            vendingMachineType = Map.FindParentRoom(gameObject).Zone == ZoneType.Entrance
                ? VendingMachineType.EntranceZone
                : VendingMachineType.LightContainmentZone;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var Button = Item.Create(ItemType.ArmorLight).Spawn(
                        gameObject.transform.TransformPoint(_basePosition + new Vector3(step * -j, step * i, 0)));
                    Button.Scale = Vector3.one * 0.1f;
                    Button.Rotation = gameObject.transform.rotation * Quaternion.Euler(0,-90,0);

                    var rigidBody1 = Button.Base.gameObject.GetComponent<Rigidbody>();
                    var collider1 = Button.Base.gameObject.GetComponents<Collider>();

                    foreach (var thing in collider1)
                    {
                        thing.enabled = false;
                    }

                    if (rigidBody1 != null)
                    {
                        rigidBody1.useGravity = false;
                        rigidBody1.angularDrag = 10000000000;
                        rigidBody1.drag = 10000000000;
                        rigidBody1.detectCollisions = false;
                        rigidBody1.freezeRotation = true;
                        rigidBody1.isKinematic = false;
                        rigidBody1.mass = 10000000000;
                    }

                    Buttons.Add(Button);
                }
            }

            var RefundButton = Item.Create(ItemType.ArmorLight).Spawn(gameObject.transform.TransformPoint(_refundPosition));
            RefundButton.Scale = new Vector3(0.1f, 0.1f, 0.2f);
            RefundButton.Rotation = gameObject.transform.rotation * Quaternion.Euler(0,-90,0);

            var rigidBody = RefundButton.Base.gameObject.GetComponent<Rigidbody>();
            var collider = RefundButton.Base.gameObject.GetComponents<Collider>();

            foreach (var thing in collider)
            {
                thing.enabled = false;
            }

            if (rigidBody != null)
            {
                rigidBody.useGravity = false;
                rigidBody.angularDrag = 10000000000;
                rigidBody.drag = 10000000000;
                rigidBody.detectCollisions = false;
                rigidBody.freezeRotation = true;
                rigidBody.isKinematic = false;
                rigidBody.mass = 10000000000;
            }

            Buttons.Add(RefundButton);

            foreach (var button in Buttons)
            {
                BreakoutBlitz.PickupsToNotClear.Add(button);
            }
            
            foreach (var block in obj.AttachedBlocks)
            {
                if (block.name.Contains("DIGIT") || block.name.Contains("DECIMAL_POINT"))
                {
                    if(!block.name.Contains("DECIMAL_POINT"))
                        _digits.Add(block.GetComponent<PrimitiveObject>());
                    else
                    {
                        block.GetComponent<PrimitiveObject>().Primitive.Base.NetworkMaterialColor = Color.red;
                    }
                }

                if (block.name.Contains("LightSource"))
                {
                    block.gameObject.AddComponent<Components.LightBlink>();
                    
                    _lightSources.Add(block.GetComponent<LightSourceObject>());
                    
                }
            }

            for (int i = 0; i < 4; i++)
                _digitDisplays.Add(new DigitDisplay());

            _digits = _digits.OrderBy(x=>x.name).ToList();

            int counter = 0;
            int displayCounter = 0;
            
            for (int i = 0; i < 28; i++)
            {
                if(counter == 7)
                {
                    counter = 0;
                    displayCounter++;
                }
                
                _digitDisplays[displayCounter].Segments.Add(_digits[i]);
                counter++;
            }
            
            foreach(var display in _digitDisplays)
            {
                for (int i = 0; i < 7; i++)
                {
                    display.Segments[i].Primitive.Base.NetworkMaterialColor = _digitsMap[0][i] ? Color.red : Color.black;
                }
            }

            _digitDisplays.Reverse();

            for (int i = 0; i < EventHandler.random.Next(3); i++)
            {
                Item.Create(ItemType.Coin).Spawn(gameObject.transform.TransformPoint(0, 0.3f, 0.5f));
            }
        }

        private void OnDestroy()
        {
            Exiled.Events.Handlers.Player.PickingUpArmor -= OnPickingUpArmor;
            foreach (var button in Buttons)
                button.Destroy();
        }
        
        public static void DestroyVendingMachines()
        {
            foreach (var vending in VendingMachines)
            {
                NetworkServer.Destroy(vending.gameObject);
            }
            
            VendingMachines.Clear();
        }

        private void OnPickingUpArmor(PickingUpArmorEventArgs ev)
        {
            if(Buttons.Contains(ev.Pickup))
            {
                ev.IsAllowed = false;
                ev.Pickup.Locked = false;
                ev.Pickup.InUse = false;

                if (ev.Pickup == Buttons.Last())
                {
                    for (int i = 0; i < _coins; i++)
                    {
                        Item.Create(ItemType.Coin).Spawn(gameObject.transform.TransformPoint(0, 0.3f, 0.5f));
                    }
                    
                    _coins = 0;
                    _display = "0000";
                    UpdateDisplay();
                    return;
                }
                
                // Get index of button in Buttons list
                var index = Buttons.IndexOf(ev.Pickup);

                if (_coins >= _vendingMachineItems[index].RequiredCoins)
                {
                    if (_coins > _vendingMachineItems[index].RequiredCoins)
                    {
                        _coins -= _vendingMachineItems[index].RequiredCoins;
                        
                        _display = $"{_coins*25:D4}";
                        UpdateDisplay();
                    }
                    else
                    {
                        _coins = 0;
                        _display = "0000";
                        UpdateDisplay();
                    }

                    if (_vendingMachineItems[index].ItemType == ItemType.None)
                    {
                        switch (_vendingMachineItems[index].ItemName)
                        {
                            case "M&Ms":
                                Scp330Bag.AddSimpleRegeneration(ev.Player.ReferenceHub, 5, 10);
                                ev.Player.ShowCenterDownHint("You drank the M&Ms (+Regen)", 3);
                                break;
                            
                            case "Conk":
                                Scp330Bag.AddSimpleRegeneration(ev.Player.ReferenceHub, 5, 10);
                                ev.Player.ShowCenterDownHint("You drank the Conk (+Regen)", 3);
                                break;
                            
                            case "Bepis":
                                Scp330Bag.AddSimpleRegeneration(ev.Player.ReferenceHub, 5, 10);
                                ev.Player.ShowCenterDownHint("You drank the Bepis (+Regen)", 3);
                                break;
                            
                            case "Layz":
                                Scp330Bag.AddSimpleRegeneration(ev.Player.ReferenceHub, 5, 10);
                                ev.Player.ShowCenterDownHint("You ate the chips (+Regen)", 3);
                                break;
                            
                            case "MysteryBox":
                                if (vendingMachineType == VendingMachineType.EntranceZone)
                                {
                                    Item.Create(MysteryBoxLootEZ.PickRandom()).Spawn(
                                        gameObject.transform.TransformPoint(0, 0.3f, 0.5f));
                                }
                                else
                                {
                                    var item = MysteryBoxLootLCZ.PickRandom();

                                    if (item.IsWeapon())
                                    {
                                        var f = (Firearm) Firearm.Create(item);
                                        f.Ammo = 0;
                                        f.Spawn(gameObject.transform.TransformPoint(0, 0.3f, 0.5f));
                                    }
                                    else
                                    {
                                        Item.Create(item).Spawn(gameObject.transform.TransformPoint(0, 0.3f, 0.5f));
                                    }
                                }

                                break;
                        }
                    }
                    else
                    {
                        Item.Create(_vendingMachineItems[index].ItemType).Spawn(gameObject.transform.TransformPoint(0, 0.3f, 0.5f));
                        if (_vendingMachineItems[index].ItemType.IsAmmo())
                        {
                            Item.Create(_vendingMachineItems[index].ItemType).Spawn(gameObject.transform.TransformPoint(0, 0.3f, 0.5f));
                        }
                    }

                    ev.Player.ShowCenterDownHint("Thank you for your purchase!", 3);
                }
                else
                {
                    // Not enough coins and display how many are needed
                    ev.Player.ShowCenterDownHint("You need " + (_vendingMachineItems[index].RequiredCoins - _coins) + " more coins to purchase this item!", 3);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_timer < 0.5)
            {
                _timer += Time.deltaTime;
                return;
            }
            
            UpdateVendingMachine();
        }

        private void UpdateVendingMachine()
        {
            var colliders = Physics.OverlapSphere(gameObject.transform.TransformPoint(_basePosition) + Vector3.up, 1.1f,
                LayerMask.GetMask("Pickup"));
            foreach (var col in colliders)
            {
                if (col.transform.root.gameObject.TryGetComponent(out ItemPickupBase pickup))
                {
                    if (DeletedItems.Contains(pickup))
                        continue;

                    if (pickup.NetworkInfo.ItemId == ItemType.Coin)
                    {
                        _display = $"{int.Parse(_display) + 25:D4}";
                        UpdateDisplay();
                        Log.Info("Added coin, current display: " + _display);

                        _coins++;
                        pickup.DestroySelf();
                    }
                }
            }

            DeletedItems.Clear();
            _timer = 0;
        }

        private void UpdateDisplay()
        {
            for (int i = 0; i < 4; i++)
            {
                for (var j = 0; j < _digitDisplays[i].Segments.Count; j++)
                {
                    var display = _digitDisplays[i].Segments[j];
                    display.Primitive.Base.NetworkMaterialColor =
                        _digitsMap[int.Parse(_display[i].ToString())][j] ? Color.red : Color.black;
                }
            }
        }
    }
}