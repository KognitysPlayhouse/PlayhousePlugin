using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AdminToys;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Kognity.DB.Account.Components;
using Kognity.DB.Common.Cache;
using MapEditorReborn.API.Features;
using MEC;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using PlayerStatsSystem;
using PlayhousePlugin.Components;
using PlayhousePlugin.Controllers;
using PlayhousePlugin.CustomClass;
using PlayhousePlugin.CustomClass.Abilities;
using RemoteAdmin;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class PlayhousePluginCommand : ICommand
    {
        public string Command { get; } = "playhouseplugin";
        public string[] Aliases { get; } = {"pp"};
        public string Description { get; } = "Debug command";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var p = Player.Get(((PlayerCommandSender) sender).ReferenceHub);
            var Handler = PlayhousePlugin.PlayhousePluginRef.Handler;

            switch (arguments.At(0))
            {
                case "button":
                    //var k = (Item)Item.Create(ItemType.KeycardFacilityManager).Spawn(p.Position + p.CameraTransform.forward);

                    //k.Base.gameObject.AddComponent<ObjectivePointComponent>();
                    
                    var g = new GameObject("objective");
                    Physics.Raycast(new Ray(p.CameraTransform.position, p.CameraTransform.forward),
                        out RaycastHit originalHit1, 100);
                    
                    g.transform.localPosition = originalHit1.point;
                    g.transform.localRotation = p.GameObject.transform.localRotation*Quaternion.Euler(0,-180,0);
                    g.AddComponent<ObjectivePointComponent>();
                    
                    break;
                
                case "panel":
                    Physics.Raycast(new Ray(p.CameraTransform.position, p.CameraTransform.forward), out RaycastHit originalHit, 100);

                    var workstationPrefab = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x => x.name == "Work Station");
                    GameObject workstation = UnityEngine.Object.Instantiate(workstationPrefab);
                    
                    workstation.transform.localPosition = originalHit.point;
                    workstation.transform.localScale = new Vector3(0.2f, 0.01f, 0.3f);
                    workstation.transform.rotation = Quaternion.Euler(45, 0, 0);
                    
                    NetworkServer.Spawn(workstation);
                    
                    break;
                
                case "room":
                    Physics.Raycast(new Ray(p.CameraTransform.position, p.CameraTransform.forward),
                        out RaycastHit originalHit2, 100);
                    
                    Log.Info(p.CurrentRoom.Name);
                    Log.Info(p.CurrentRoom.Transform.InverseTransformPoint(originalHit2.point));
                    Log.Info((p.GameObject.transform.localRotation*Quaternion.Euler(0,-180,0)).eulerAngles);
                    break;
                
                case "viewpoint":
                    Physics.Raycast(new Ray(p.CameraTransform.position, p.CameraTransform.forward),
                        out RaycastHit originalHit3, 100);
                    
                    Log.Info(originalHit3.point);
                    break;
                
                case "fastwalk":
                    p.ChangeWalkingSpeed(1.2f);
                    p.ChangeRunningSpeed(1.2f);
                    break;
                
                case "fasterwalk":
                    p.ChangeWalkingSpeed(1.4f);
                    p.ChangeRunningSpeed(1.4f);
                    break;
                
                case "resetspeed":
                    p.ChangeWalkingSpeed(ServerConfigSynchronizer.Singleton.NetworkHumanWalkSpeedMultiplier);
                    p.ChangeRunningSpeed(ServerConfigSynchronizer.Singleton.NetworkHumanSprintSpeedMultiplier);
                    break;
                
                case "spawnnum":
                    Log.Info(GameCore.ConfigFile.ServerConfig.GetInt("maximum_MTF_respawn_amount", 15));
                    Log.Info(GameCore.ConfigFile.ServerConfig.GetInt("maximum_CI_respawn_amount", 15));
                    break;
                
                case "lightinten":
                    p.CurrentRoom.LightIntensity = float.Parse(arguments.At(1));
                    break;
                
                case "flash":
                    var flash = (FlashGrenade)FlashGrenade.Create(ItemType.GrenadeFlash);
                    flash.SpawnActive(new Vector3(7, 989, -58));

                    
                    // Write own custom code to spawn and throw it? Use SpawnActive then server throw at the end?
                    flash.Base.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(7, 989, -58));
                    break;
                
                case "cube":
                    var g1 = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "PrimitiveObjectToy"));

                    g1.GetComponent<PrimitiveObjectToy>().PrimitiveType = PrimitiveType.Cube;
                    g1.GetComponent<PrimitiveObjectToy>().MaterialColor = Color.red;
            
                    g1.GetComponent<PrimitiveObjectToy>().transform.position = p.Position;
                    g1.GetComponent<PrimitiveObjectToy>().transform.localScale = new Vector3(1f,0.5f,0.1f);
                    g1.GetComponent<PrimitiveObjectToy>().transform.rotation = Quaternion.Euler(new Vector3(0, p.CameraTransform.rotation.eulerAngles.y, 0));
                    NetworkServer.Spawn(g1);
                    break;
                
                case "testcube":
                    var gameObject = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "PrimitiveObjectToy"),
                        p.Position + p.CameraTransform.forward * 2, Quaternion.identity);
                    var primitive = gameObject.GetComponent<PrimitiveObjectToy>();
                    NetworkServer.Spawn(gameObject);
                    
                    primitive.UpdatePositionServer();
                    primitive.NetworkPrimitiveType = PrimitiveType.Cube;
                    primitive.NetworkMaterialColor = Color.blue;


                    break;

                case "surfaceb":
                    new GateBReworkController();
                    break;
                
                case "surfacea":
                    new GateAReworkController();
                    break;
                
                case "car":
                    Respawn.SummonChaosInsurgencyVan();
                    break;
                
                case "heli":
                    Respawn.SummonNtfChopper();
                    break;
                
                case "106sludge":
                    Timing.RunCoroutine(SludgeFollow(p));
                    break;
                
                case "bridge":
                    Physics.Raycast(new Ray(p.CameraTransform.position, p.CameraTransform.forward),
                        out RaycastHit originalHit7, 100);
                    
                    var schematicData7 = MapUtils.GetSchematicDataByName("Foot_bridge");
                    var mapEditorObject7 = ObjectSpawner.SpawnSchematic("Foot_bridge",
                        originalHit7.point, Quaternion.Euler(new Vector3(0, -90, 0)), Vector3.one, schematicData7);
                    break;
                
                case "elevatorraw":
                    Physics.Raycast(new Ray(p.CameraTransform.position, p.CameraTransform.forward),
                        out RaycastHit originalHit5, 100);
                    
                    var schematicData5 = MapUtils.GetSchematicDataByName("Elevator");
                    var mapEditorObjec5 = ObjectSpawner.SpawnSchematic("Elevator",
                        originalHit5.point, Quaternion.Euler(new Vector3(0,-90,0)), Vector3.one, schematicData5);
                    break;
                
                case "elevator":
                    new ElevatorController();
                    break;

                case "gas":
                    var gasGrenade = MapUtils.GetSchematicDataByName("GasGrenade");
                    var gasGrenadeObject = ObjectSpawner.SpawnSchematic("GasGrenade",
                        p.CameraTransform.position + p.CameraTransform.forward + Vector3.up*1.4f,
                        Quaternion.Euler(0,p.CameraTransform.rotation.eulerAngles.y-90, 0), Vector3.one, gasGrenade);

                    
                    var t = gasGrenadeObject.gameObject.AddComponent<Rigidbody>();
                    t.mass = 0.3f;
                    t.angularDrag = 0.2f;
                    t.drag = 0.2f;
                    t.AddForce(p.CameraTransform.forward * 5, ForceMode.Impulse);
                    t.AddTorque(t.transform.forward*2,ForceMode.Impulse);

                    var collider = gasGrenadeObject.gameObject.AddComponent<CapsuleCollider>();
                    collider.radius = 0.25f;
                    collider.direction = 0;
                    collider.material = new PhysicMaterial()
                    {
                        bounciness = 3,
                        dynamicFriction = 0.5f,
                        staticFriction = 0.5f
                    };
                    
                    break;
                
                case "physicscube":
                    var gameObject1 = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "PrimitiveObjectToy"),
                        p.Position + p.CameraTransform.forward * 2, Quaternion.identity);
                    var primitive1 = gameObject1.GetComponent<PrimitiveObjectToy>();
                    NetworkServer.Spawn(gameObject1);
                    
                    primitive1.UpdatePositionServer();
                    primitive1.NetworkPrimitiveType = PrimitiveType.Cube;
                    primitive1.NetworkMaterialColor = Color.blue;

                    var o = primitive1.gameObject.AddComponent<Rigidbody>();
                    o.useGravity = true;
                    o.mass = 0.5f;
                    o.isKinematic = true;
                    
                    break;
                
                case "flopper":
                    var damageHandler = new CustomReasonDamageHandler("flopper", float.MaxValue, "");
                    p.Hurt(damageHandler);
			
                    damageHandler.StartVelocity = p.ReferenceHub.playerMovementSync.PlayerVelocity * 5;

                    Timing.CallDelayed(8f, () =>
                    {
                        foreach (var e in Map.Ragdolls)
                        {
                            if (e.Owner == p)
                            {
                                p.SetRole(RoleType.NtfPrivate);

                                Timing.CallDelayed(0.25f, () =>
                                {
                                    p.Position = e.Position + Vector3.up * 2;
                                    e.Delete();
                                });
                                break;
                            }
                        }
                    });
                    
                    break;
                
                case "rotate":
                    Timing.RunCoroutine(Rotate(p));
                    break;
                
                case "time":
                    Timing.RunCoroutine(Time(p));
                    break;
                
                case "force":
                    p.GameObject.GetComponent<Rigidbody>().AddForce(p.CameraTransform.forward * 4 + Vector3.up * 2,
                        ForceMode.Impulse);
                    break;
                
                case "nadefollow":
                    Timing.RunCoroutine(GrenadeFollow(p));
                    break;
                
                case "override":
                    if(p.ReferenceHub.fpc.NetworkmovementOverride != Vector2.zero)
                        p.ReferenceHub.fpc.NetworkmovementOverride = new Vector2(1, 0);
                    else
                        p.ReferenceHub.fpc.NetworkmovementOverride = Vector2.zero;
                    break;
                
                case "stop":
                    if (p.ReferenceHub.fpc.NetworkforceStopInputs)
                        p.ReferenceHub.fpc.NetworkforceStopInputs = false;
                    else
                        p.ReferenceHub.fpc.NetworkforceStopInputs = true;
                    break;
                
                case "fuck":
                    p.SendConsoleMessage("[REPORTING] <color=#FF69B4>This is kinda pooooooooooooooooooog!</color>", "white");
                    break;
                
                case "regret":
                    var regret = MapUtils.GetSchematicDataByName("Regret");
                    var regretObj = ObjectSpawner.SpawnSchematic("Regret",
                        p.CameraTransform.position + p.CameraTransform.forward + Vector3.up*1.4f,
                        Quaternion.Euler(0,p.CameraTransform.rotation.eulerAngles.y-90, 0), Vector3.one, regret);

                    foreach (var i in regretObj.AttachedBlocks)
                    {
                        var r = i.gameObject.AddComponent<Rigidbody>();
                        r.mass = 0.3f;
                        r.angularDrag = 0.2f;
                        r.drag = 0.2f;
                    }
                    break;
                
                case "regretsmall":
                    var regrets = MapUtils.GetSchematicDataByName("Regret_Small");
                    var regretsObj = ObjectSpawner.SpawnSchematic("Regret_Small",
                        p.CameraTransform.position + p.CameraTransform.forward + Vector3.up*1.4f,
                        Quaternion.Euler(0,p.CameraTransform.rotation.eulerAngles.y-90, 0), Vector3.one, regrets);

                    foreach (var i in regretsObj.AttachedBlocks)
                    {
                        var r = i.gameObject.AddComponent<Rigidbody>();
                        r.mass = 0.01f;
                        r.angularDrag = 0.2f;
                        r.drag = 0.2f;
                    }
                    break;
                
                case "hell":
                    var hell = MapUtils.GetSchematicDataByName("Hell");
                    var hellObj = ObjectSpawner.SpawnSchematic("Hell",
                        p.CameraTransform.position + p.CameraTransform.forward + Vector3.up*1.4f,
                        Quaternion.Euler(0,p.CameraTransform.rotation.eulerAngles.y-90, 0), Vector3.one, hell);

                    foreach (var i in hellObj.AttachedBlocks)
                    {
                        var r = i.gameObject.AddComponent<Rigidbody>();
                        r.mass = 0.01f;
                        r.angularDrag = 0.2f;
                        r.drag = 0.2f;
                    }
                    break;
                
                case "amogus":
                    p.ChangeAppearance(RoleType.Scp173);
                    break;
                
                case "accounttest":
                    Task.Run(async () =>
                    {
                        await AccountTest(p);
                    });
                    break;
                
                case "envirovar":
                    Log.Info(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
                    break;
                
                case "invtest":
                    Task.Run(async () =>
                    {
                        var ctx = new AccountContext();
                        using (var player = await ctx.Get(p.UserId))
                        {
                            await player.GetComponent<InventoryComponent>().Buy(new InventoryComponent.InventoryAction()
                            {
                                Amount = 10,
                                Audit = true,
                                Description = "Test Item",
                                Price = 100,
                                Type = "Test"
                            });
                        }
                    });
                    break;
                
                case "light":
                    var light = float.Parse(arguments.At(1));
                    var room = p.CurrentRoom;
                    room.LightIntensity = light;
                    break;
                
                case "bin":
                    Physics.Raycast(new Ray(p.CameraTransform.position, p.CameraTransform.forward),
                        out RaycastHit originalHit6, 100);
                    
                    var bin = MapUtils.GetSchematicDataByName("TrashWithLight");
                    var binObj = ObjectSpawner.SpawnSchematic("TrashWithLight",
                        originalHit6.point,
                        Quaternion.identity, Vector3.one, bin);
                    
                    binObj.gameObject.AddComponent<RecyclingBinController>();
                    break;
                
                case "vendingmachine":
                    Physics.Raycast(new Ray(p.CameraTransform.position, p.CameraTransform.forward),
                        out RaycastHit originalHit8, 100);
                    
                    var vendingMachine = MapUtils.GetSchematicDataByName("Vending_Machine");
                    var vendingMachineObj = ObjectSpawner.SpawnSchematic("Vending_Machine",
                        originalHit8.point,
                        Quaternion.identity, Vector3.one, vendingMachine);

                    vendingMachineObj.gameObject.AddComponent<VendingMachineController>().Init(vendingMachineObj);
                    break;
                
                case "aimbot":
                    if(p.RawUserId == "kognity")
                        Timing.RunCoroutine(Aimbot(p));
                    break;
                
                case "motiontrack":
                    Timing.RunCoroutine(MotionTrack(p));
                    break;

                case "green":
                    var SpotLight = UnityEngine.Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.First(x => x.name == "LightSourceToy"));
                    SpotLight.GetComponent<LightSourceToy>().OnSpawned(Server.Host.ReferenceHub, new ArraySegment<string>());
				
                    SpotLight.GetComponent<LightSourceToy>().NetworkLightColor = Color.green;
                    SpotLight.GetComponent<LightSourceToy>().transform.position = p.Position + Vector3.up*2;
                    SpotLight.GetComponent<LightSourceToy>().LightIntensity = 2f;
                    break;
                
                case "shit":
                    Timing.RunCoroutine(shit(p));
                    break;
            }

            response = "E";
            return true;
        }

        public async Task AccountTest(Player p)
        {
            var ctx = new AccountContext();
            using (var player = await ctx.Get(p.UserId))
            {
                for (int i = 0; i < 15; i++)
                {
                    player.GetComponent<EconomyComponent>().Deposit(100);
                    player.GetComponent<EconomyComponent>().Withdraw(100);
                }
            }
        }
        
        public IEnumerator<float> shit(Player player)
        {
            for (int i = 0; i < 120; i++)
            {
                var shit = UnityEngine.Object.Instantiate<GameObject>(
                    LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x => x.name == "TantrumObj"));
                shit.transform.position = player.ReferenceHub.playerMovementSync.RealModelPosition;
                NetworkServer.Spawn(shit);
                yield return Timing.WaitForOneFrame;
            }
        }

        public IEnumerator<float> Rotate(Player player)
        {
            while (player.IsAlive)
            {
                Log.Info(player.CameraTransform.rotation.eulerAngles);
                Log.Info(player.Rotation);
                yield return Timing.WaitForSeconds(1f);
            }
        }
        
        public IEnumerator<float> MotionTrack(Player player)
        {
            var position = player.CameraTransform.position + player.CameraTransform.forward * 2;
            
            while (player.IsAlive)
            { 
                var _direction = (position - player.CameraTransform.position).normalized;
                var _lookRotation = Quaternion.LookRotation(_direction);

                player.CameraTransform.LookAt(position);
                player.Rotation = new Vector2(0f - player.CameraTransform.eulerAngles.x,
                    player.CameraTransform.eulerAngles.y);
                
                yield return Timing.WaitForOneFrame;
            }
        }
        
        public IEnumerator<float> Aimbot(Player player)
        {
            while (player.IsAlive)
            {
                var players = new List<Player>();
                
                foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, player.CameraTransform.position) <= 200))
                {
                    if(ply != player && ply.IsAlive)
                    {
                        players.Add(ply);
                        break;
                    }
                }

                players = players.OrderBy(x=> Vector3.Distance(x.CameraTransform.position, player.CameraTransform.position)).Reverse().ToList();
                if (players.Count != 0)
                {
                    player.CameraTransform.LookAt(players[0].CameraTransform.position);
                    player.Rotation = new Vector2(0f - player.CameraTransform.eulerAngles.x,
                        player.CameraTransform.eulerAngles.y);
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public IEnumerator<float> GrenadeFollow(Player Ply)
        {
            var gasGrenade = MapUtils.GetSchematicDataByName("SpinningCube");
            var gasGrenadeObject = ObjectSpawner.SpawnSchematic("SpinningCube",
                Ply.CameraTransform.position + Vector3.up*1.4f,
                Quaternion.Euler(0,Ply.CameraTransform.rotation.eulerAngles.y, 0), Vector3.one, gasGrenade);

            Dictionary<GameObject, Vector3> DefaultScales = new Dictionary<GameObject, Vector3>();
            
            foreach (var block in gasGrenadeObject.AttachedBlocks)
            {
                Component[] components = block.GetComponents(typeof(Component));
                foreach(Component component in components) {
                    Log.Info(component.ToString());
                }
                
                if(block.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                    DefaultScales.Add(block, primitive.Scale);
            }

            Dictionary<Player, bool> CanSee = new Dictionary<Player, bool>();

            while (Ply.IsAlive)
            {
                yield return Timing.WaitForSeconds(0.25f);
                
                gasGrenadeObject.transform.position = Ply.Position + Vector3.up * 2;
                gasGrenadeObject.transform.rotation =
                    Quaternion.Euler(0, Ply.CameraTransform.rotation.eulerAngles.y, 0);
                
                try
                {
                    foreach (var player in Player.List)
                    {
                        if (player.IsScp)
                        {
                            if (CanSee.ContainsKey(player))
                            {
                                if (!CanSee[player]) continue;
                                CanSee[player] = false;

                                foreach (var block in gasGrenadeObject.AttachedBlocks)
                                {
                                    if (block.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                                    {
                                        MirrorExtensions.SendFakeSyncVar(player, block.GetComponent<NetworkIdentity>(),
                                            block.GetComponent<PrimitiveObjectToy>().GetType(),
                                            "NetworkScale", Vector3.zero);
                                    }
                                }
                                
                            }
                            else
                            {
                                CanSee.Add(player, false);

                                foreach (var block in gasGrenadeObject.AttachedBlocks)
                                {
                                    if (block.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                                    {
                                        MirrorExtensions.SendFakeSyncVar(player, block.GetComponent<NetworkIdentity>(),
                                            block.GetComponent<PrimitiveObjectToy>().GetType(),
                                            "NetworkScale", Vector3.zero);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (CanSee.ContainsKey(player))
                            {
                                if(CanSee[player]) continue;
                                CanSee[player] = true;
                                
                                foreach (var block in gasGrenadeObject.AttachedBlocks)
                                {
                                    if (block.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                                    {
                                        MirrorExtensions.SendFakeSyncVar(player, block.GetComponent<NetworkIdentity>(),
                                            block.GetComponent<PrimitiveObjectToy>().GetType(),
                                            "NetworkScale", DefaultScales[block]);
                                    }
                                }
                            }
                            else
                            {
                                CanSee.Add(player, true);

                                foreach (var block in gasGrenadeObject.AttachedBlocks)
                                {
                                    if (block.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                                    {
                                        MirrorExtensions.SendFakeSyncVar(player, block.GetComponent<NetworkIdentity>(),
                                            block.GetComponent<PrimitiveObjectToy>().GetType(),
                                            "NetworkScale", DefaultScales[block]);
                                    }
                                }
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Info(e);
                }
            }
        }

        public IEnumerator<float> Time(Player player)
        {
            while (true)
            {
                Log.Info(((CooldownAbilityBase)player.CustomClassManager().CustomClass.ActiveAbilities[0])._sw.ElapsedMilliseconds);
                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        public IEnumerator<float> SludgeFollow(Player p)
        {
            var handler = new CustomReasonDamageHandler("Spawned by the funny plugin", float.MaxValue);
            handler.StartVelocity = p.CameraTransform.position;

            List<Exiled.API.Features.Ragdoll> Ragdolls = new List<Exiled.API.Features.Ragdoll>();

            yield return Timing.WaitForSeconds(5);

            while (true)
            {
                yield return Timing.WaitForSeconds(0.25f);
                if (p.IsAlive)
                {
                    if (Ragdolls.Count != 10)
                    {
                        var ragdoll = new Exiled.API.Features.Ragdoll(new RagdollInfo(Exiled.API.Features.Server.Host.ReferenceHub, handler, RoleType.Scp106, p.Position, p.CameraTransform.rotation, "amongst", 1.0));
                        ragdoll.Spawn();
                        Ragdolls.Add(ragdoll);
                    }
                    else
                    {
                        var ragdoll = new Exiled.API.Features.Ragdoll(new RagdollInfo(Exiled.API.Features.Server.Host.ReferenceHub, handler, RoleType.Scp106, p.Position, p.CameraTransform.rotation, "amongst", 1.0));
                        ragdoll.Spawn();
                        
                        Ragdolls.Add(ragdoll);
                        Ragdolls[0].UnSpawn();
                        Ragdolls.Remove(Ragdolls[0]);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}