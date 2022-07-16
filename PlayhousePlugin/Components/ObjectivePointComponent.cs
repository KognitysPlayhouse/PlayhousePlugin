using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs;
using Interactables.Interobjects.DoorUtils;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using PlayhousePlugin.CustomGameMode;
using UnityEngine;

namespace PlayhousePlugin.Components
{
    public class ObjectivePointComponent : MonoBehaviour
    {
        public event EventHandler<GameObject> ObjectiveCaptured;
        
        public Utils.ObjectiveStates _state = Utils.ObjectiveStates.Disabled;
        public float Radius = 2.5f;
        public bool AllowAllToCap = false;
        public RoleType RoleToNotify = RoleType.None;
        
        private float _decayRate = 0.5f;
        private float _capRate = 1.5f;
        private float _capturedAmount = 0;
        private float _captureRequirement = 200;
        
        private float _interval = 0.25f;
        private float _nextCycle = 0f;
        
        private Vector3 _position;
        private Vector3 _positionDisplay;

        private PrimitiveObject _display;
        private LightSourceToy _light;
        private bool _lightOn = false;

        private Dictionary<Utils.ObjectiveStates, Color> ColorsAndStates = new Dictionary<Utils.ObjectiveStates, Color>
        {
            {Utils.ObjectiveStates.Disabled, Color.red},
            {Utils.ObjectiveStates.Activating, Color.yellow},
            {Utils.ObjectiveStates.Contested, Color.gray},
            {Utils.ObjectiveStates.Decaying, new Color(191/255f, 181/255f, 203/255f)},
            {Utils.ObjectiveStates.Enabled, Color.green},
        };
        
        private Dictionary<Utils.ObjectiveStates, Color> LightColors = new Dictionary<Utils.ObjectiveStates, Color>
        {
            {Utils.ObjectiveStates.Disabled, Color.red},
            {Utils.ObjectiveStates.Activating, Color.yellow},
            {Utils.ObjectiveStates.Contested, Color.black},
            {Utils.ObjectiveStates.Decaying, Color.red},
            {Utils.ObjectiveStates.Enabled, Color.green},
        };
        
        private Pickup _activateButton;
        
        private Door _door;
        
        private void Awake()
        {
            Log.Info(Map.FindParentRoom(gameObject));
            Log.Info(gameObject.transform.rotation.eulerAngles);
            Log.Info(gameObject.transform.localPosition);

            // Misc Setup
            _nextCycle = Time.time;
            _position = gameObject.transform.position;
            _positionDisplay = _position + Vector3.up * 0.23f;
            
            // Registering Events
            Exiled.Events.Handlers.Player.PickingUpArmor += OnPickup;

            // Door beep sounds
            var doorPrefab = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x => x.name == "HCZ BreakableDoor");
            GameObject doorV = Instantiate(doorPrefab);

            NetworkServer.Spawn(doorV);
            _door = Door.Get(doorV.GetComponent<DoorVariant>());
            _door.Position = _position + Vector3.down;
            _door.Scale = Vector3.zero;
            _door.ChangeLock(DoorLockType.AdminCommand);
            _door.MaxHealth = float.MaxValue;
            _door.Health = float.MaxValue;

            // Panel setup
            var schematicData = MapUtils.GetSchematicDataByName("Terminal");
            var mapEditorObject = ObjectSpawner.SpawnSchematic("Terminal",
                _position, Quaternion.Euler(new Vector3(0, gameObject.transform.localEulerAngles.y, 0)), Vector3.one, schematicData);

            // Activate Button
            _activateButton = Item.Create(ItemType.ArmorLight).Spawn(_position +Vector3.down*0.14f);
            _activateButton.Scale = new Vector3(0.1f, 1f, 1f);
            _activateButton.Rotation = Quaternion.Euler(-90, -90 + gameObject.transform.localEulerAngles.y, 0);
            _activateButton.Weight = 0.01f;
            
            BreakoutBlitz.PickupsToNotClear.Add(_activateButton);

            var rigidBody1 = _activateButton.Base.gameObject.GetComponent<Rigidbody>();
            var collider1 = _activateButton.Base.gameObject.GetComponents<Collider>();

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
            
            // Display Primitive
            foreach (var block in mapEditorObject.AttachedBlocks)
            {
                if (block.name.Contains("Screen"))
                {
                    _display = block.GetComponent<PrimitiveObject>();
                    _display.Primitive.Base.NetworkMaterialColor = Color.red;
                }
            }

            _light = Instantiate(Utils.LightSourceBaseObject);
            _light.GetComponent<LightSourceToy>().NetworkLightColor = Color.red;
            _light.GetComponent<LightSourceToy>().NetworkLightRange = 3;
            _light.GetComponent<LightSourceToy>().transform.position =  _positionDisplay + gameObject.transform.forward*0.45f + Vector3.up*0.1f;
            NetworkServer.Spawn(_light.gameObject);
        }

        private void OnDestroy()
        {
            NetworkServer.UnSpawn(_display.gameObject);
            _activateButton.Destroy();
            NetworkServer.Destroy(_door.Base.gameObject);
            Exiled.Events.Handlers.Player.PickingUpArmor -= OnPickup;
        }

        private void FixedUpdate()
        {
            if (Time.time >= _nextCycle)
            {
                _nextCycle += _interval;
                
                if (!_lightOn)
                {
                    _light.NetworkLightColor = LightColors[_state];

                    _lightOn = !_lightOn;
                }
                else
                {
                    _lightOn = !_lightOn;
                    _light.NetworkLightColor = Color.black;
                }

                if (ObjectivePointController.FailedObjectives && _state == Utils.ObjectiveStates.Activating && !AllowAllToCap)
                {
                    _capturedAmount = 0;
                    _activateButton.Locked = true;
                    _activateButton.InUse = true;
                    
                    _state = Utils.ObjectiveStates.Disabled;
                    _display.Primitive.Base.NetworkMaterialColor = Color.black;
                }
                
                if (_state == Utils.ObjectiveStates.Enabled || _state == Utils.ObjectiveStates.Disabled)
                    return;
                
                var playersCapturing = 0;
                var playersDecaying = 0;

                List<Player> countedPlayers = new List<Player>();
                
                foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, _position) <= Radius))
                {
                    if (ply != null)
                    {
                        if (countedPlayers.Contains(ply))
                            continue;
                        else
                            countedPlayers.Add(ply);

                        if (AllowAllToCap)
                        {
                            if (ply.IsHuman)
                                playersCapturing += 1;
                            else
                                playersDecaying += 1;
                        }
                        else
                        {
                            if (ply.Role.Team == Team.MTF)
                            {
                                playersCapturing += 1;
                            }
                            else if (ply.Role.Team == Team.RSC || (ply.Role.Team == Team.CDP && ply.IsCuffed) ||
                                     ply.Role.Team == Team.RIP || ply.Role.Team == Team.TUT)
                            {
                            }
                            else
                            {
                                playersDecaying += 1;
                            }
                        }
                    }
                }

                if (playersCapturing != 0 && playersDecaying == 0) // Only friendlies is on the point, capture 
                {
                    if (_state != Utils.ObjectiveStates.Activating)
                    {
                        _state = Utils.ObjectiveStates.Activating;
                        _display.Primitive.Base.NetworkMaterialColor = ColorsAndStates[_state];
                    }
                    _capturedAmount += (float)Math.Log(playersCapturing*playersCapturing) + 1 * _capRate;
                    
                    foreach (var player in countedPlayers)
                        player.ShowCenterUpHint($"Captured {Math.Round(_capturedAmount/_captureRequirement*100, 0)}%");

                    if (RoleToNotify != RoleType.None)
                    {
                        foreach (Player ply in Player.List.Where(x => x.Role.Type == RoleToNotify))
                        {
                            ply.ShowCenterDownHint("<color=red>! Your objective point is being captured !</color>");
                        }
                    }
                }
                else if (playersCapturing != 0 && playersDecaying != 0) // Friendlies and Enemies are on the point, Contested pause capture
                {
                    if (_state != Utils.ObjectiveStates.Contested)
                    {
                        _state = Utils.ObjectiveStates.Contested;
                        _display.Primitive.Base.NetworkMaterialColor = ColorsAndStates[_state];
                    }

                    foreach (var player in countedPlayers)
                    {
                        player.ShowCenterUpHint($"Contested Objective Point");                        
                    }
                } 
                else if (playersCapturing == 0 && playersDecaying != 0) // Only an enemy is on the point, just decay
                {
                    _capturedAmount -= _decayRate;

                    if (_state != Utils.ObjectiveStates.Decaying)
                    {
                        _state = Utils.ObjectiveStates.Decaying;
                        _display.Primitive.Base.NetworkMaterialColor = ColorsAndStates[_state];
                    }
                    
                    foreach (var player in countedPlayers)
                    {
                        player.ShowCenterUpHint($"Reversing capture {_capturedAmount/_captureRequirement*100}%");                        
                    }
                }
                else // No one is on the point, just decay
                {
                    _capturedAmount -= _decayRate;

                    if (_state != Utils.ObjectiveStates.Decaying)
                    {
                        _state = Utils.ObjectiveStates.Decaying;
                        _display.Primitive.Base.NetworkMaterialColor = ColorsAndStates[_state];
                    }
                }

                // Check if the objective has been captured
                if (_capturedAmount >= _captureRequirement)
                {
                    Log.Info("Cap complete");

                    if (AllowAllToCap)
                    {
                        _activateButton.Locked = true;
                        _activateButton.InUse = true;
                    }
                    else
                    {
                        _activateButton.Locked = false;
                        _activateButton.InUse = false;
                    }

                    _state = Utils.ObjectiveStates.Enabled;
                    _display.Primitive.Base.NetworkMaterialColor = ColorsAndStates[_state];
                    OnObjectiveCaptured();
                }
                
                // check if the objective needs to be disabled
                if (_capturedAmount <= 0)
                {
                    Log.Info("Disabled objective");

                    _capturedAmount = 0;
                    _activateButton.Locked = false;
                    _activateButton.InUse = false;
                    
                    _state = Utils.ObjectiveStates.Disabled;
                    _display.Primitive.Base.NetworkMaterialColor = ColorsAndStates[_state];
                }
            }
            
            if (_state == Utils.ObjectiveStates.Activating)
                _door.PlaySound(DoorBeepType.InteractionAllowed);
        }

        private void OnPickup(PickingUpArmorEventArgs ev)
        {
            if (ev.Pickup == _activateButton)
            {
                ev.IsAllowed = false;

                if (AllowAllToCap)
                {
                    if (_state == Utils.ObjectiveStates.Disabled && Generator.List.Count(x=>x.IsEngaged) >= 1)
                    {
                        _state = Utils.ObjectiveStates.Activating;
                        _display.Primitive.Base.NetworkMaterialColor = ColorsAndStates[_state];
                    
                        ev.Pickup.Locked = true;
                        ev.Pickup.InUse = true;
                    }
                    else
                    {
                        ev.Player.ShowCenterDownHint("<color=red>1 079 Generator is required to power this objective point!</color>", 3);
                    
                        ev.Pickup.Locked = false;
                        ev.Pickup.InUse = false;
                    }   
                }
                else
                {
                    if (_state == Utils.ObjectiveStates.Disabled && ev.Player.Role.Team == Team.MTF && Generator.List.Count(x=>x.IsEngaged) >= 1)
                    {
                        _state = Utils.ObjectiveStates.Activating;
                        _display.Primitive.Base.NetworkMaterialColor = ColorsAndStates[_state];
                    
                        ev.Pickup.Locked = true;
                        ev.Pickup.InUse = true;
                    }
                    else
                    {
                        if (_state == Utils.ObjectiveStates.Enabled)
                        {
                            ev.Player.ShowCenterDownHint($"<color=green>{ObjectivePointController.objectivesCapped} out of 6 Generators enabled.</color>", 3);
                            ev.Pickup.Locked = false;
                            ev.Pickup.InUse = false;
                            return;
                        }
                        
                        if (Generator.List.Count(x=>x.IsEngaged) < 1)
                            ev.Player.ShowCenterDownHint("<color=red>1 079 Generator is required to power this objective point!</color>", 3);
                        else
                            ev.Player.ShowCenterDownHint("<color=red>You cannot activate these objectives!</color>", 3);
                    
                        ev.Pickup.Locked = false;
                        ev.Pickup.InUse = false;
                    }   
                }
            }
        }

        protected virtual void OnObjectiveCaptured()
        {
            if (ObjectivePointController.RapidSpawnWaves) return;
            ObjectiveCaptured?.Invoke(this, gameObject); 
        }
    }
}