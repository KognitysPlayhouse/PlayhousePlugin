using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;

namespace PlayhousePlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnMovingTarget : ICommand
    {
        public string Command { get; } = "targetmove";
        public string[] Aliases { get; } = null;
        public string Description { get; } = "Debug command";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var Ply = Player.Get(sender);

            Timing.RunCoroutine(ShitFollow(Ply));
            //Timing.RunCoroutine(FollowPlayer2(Ply));
            
            response = $"lol";
            return true;
        }

        private IEnumerator<float> FollowPlayer2(Player Ply)
        {
            var gameObject = UnityEngine.Object.Instantiate<GameObject>(
                LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x => x.name == "sportTargetPrefab"));
            gameObject.transform.position = Ply.CameraTransform.position + Ply.CameraTransform.forward*1.5f;
            NetworkServer.Spawn(gameObject);

            yield return Timing.WaitForSeconds(1f);
            while (true)
            {
                yield return Timing.WaitForOneFrame;
                gameObject.transform.localPosition = Ply.CameraTransform.position + Ply.CameraTransform.forward*3f;
                gameObject.transform.localRotation = Quaternion.Euler(0, Ply.CameraTransform.rotation.y, 0);
                NetworkServer.Spawn(gameObject);
            }
            
        }
        
        private IEnumerator<float> ShitFollow(Player Ply)
        {
            var gameObject = UnityEngine.Object.Instantiate<GameObject>(
                LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x => x.name == "TantrumObj"));
            gameObject.transform.position = Ply.ReferenceHub.playerMovementSync.RealModelPosition;
            NetworkServer.Spawn(gameObject);

            yield return Timing.WaitForSeconds(1f);
            while (true)
            {
                yield return Timing.WaitForSeconds(0.25f);
                gameObject = UnityEngine.Object.Instantiate<GameObject>(
                    LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x => x.name == "TantrumObj"));
                gameObject.transform.position = Ply.ReferenceHub.playerMovementSync.RealModelPosition;
                NetworkServer.Spawn(gameObject);
            }
            
        }
        
        private IEnumerator<float> FollowPlayer(Player Ply)
        {
            var gameObject = UnityEngine.Object.Instantiate<GameObject>(
                LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x => x.name == "TantrumObj"));
            gameObject.transform.position = Ply.CameraTransform.position + Ply.CameraTransform.forward*1.5f;
            NetworkServer.Spawn(gameObject);

            yield return Timing.WaitForSeconds(1f);
            while (true)
            {
                yield return Timing.WaitForOneFrame;
                NetworkServer.Destroy(gameObject);
                gameObject = UnityEngine.Object.Instantiate<GameObject>(
                    LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.FirstOrDefault(x => x.name == "TantrumObj"));
                gameObject.transform.position = Ply.CameraTransform.position + Ply.CameraTransform.forward*3f;
                gameObject.transform.rotation = Quaternion.Euler(0, Ply.CameraTransform.rotation.y, 0);
                NetworkServer.Spawn(gameObject);
            }
            
        }
    }
}