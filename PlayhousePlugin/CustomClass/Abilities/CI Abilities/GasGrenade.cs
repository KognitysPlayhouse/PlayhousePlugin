using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MapEditorReborn.API.Features;
using MEC;
using Mirror;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class GasGrenade : NonCooldownAbilityBase
    {
        public override string Name { get; } = "Gas Grenade";
        public override Player Ply { get; }
        public int GasAmount = 0;
        public GasGrenade(Player ply)
        {
            Ply = ply;
            Timing.RunCoroutine(GenerateGas());
        }
        public override bool UseAbility()
        {
            if (GasAmount / 30 >= 1)
            {
                GasAmount -= 30;
                // Spawn gas grenade
                Timing.RunCoroutine(ThrowGasGrenade());
                Ply.ShowCenterDownHint($"<color=yellow>Throwing Gas Grenade...</color>",3);
                return true;
            }

            Ply.ShowCenterDownHint($"<color=yellow>Gas is recharging...</color>",3);
            return false;
        }

        public override string GenerateHud()
        {
            if(GasAmount == 60)
                return $"Selected: {Name} (Canister 1: Ready, Canister 2: Ready)";
            
            if(GasAmount / 30 >= 1)
                return $"Selected: {Name} (Canister 1: Ready, Canister 2: {Math.Round((GasAmount-30)/30f*100, 0)}%)";
                
            return $"Selected: {Name} (Canister 1: {Math.Round((GasAmount)/30f*100, 0)}%, Canister 2: 0%)";
        }

        private IEnumerator<float> GenerateGas()
        {
            yield return Timing.WaitForSeconds(1);
            while (Ply.CustomClassManager().CustomClass?.Name == "Chaos Exterminator")
            {
                yield return Timing.WaitForSeconds(1);
                if(GasAmount != 60)
                    GasAmount += 1;
            }
        }

        private IEnumerator<float> ThrowGasGrenade()
        {
            var gasGrenade = MapUtils.GetSchematicDataByName("GasGrenade");
            var gasGrenadeObject = ObjectSpawner.SpawnSchematic("GasGrenade",
                Ply.CameraTransform.position + Ply.CameraTransform.forward + Vector3.up*1.4f,
                Quaternion.Euler(0,Ply.CameraTransform.rotation.eulerAngles.y-90, 0), Vector3.one, gasGrenade);
            
            var t = gasGrenadeObject.gameObject.AddComponent<Rigidbody>();
            t.mass = 0.3f;
            t.angularDrag = 0.2f;
            t.drag = 0.2f;
            t.AddForce(Ply.CameraTransform.forward * 5, ForceMode.Impulse);
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
            collider.gameObject.layer = LayerMask.NameToLayer("PlayerModel");
            yield return Timing.WaitForSeconds(3);

            var circle = UnityEngine.Object.Instantiate(Utils.PrimitiveBaseObject);
            circle.NetworkPrimitiveType = PrimitiveType.Cylinder;
            circle.NetworkMaterialColor = new Color(15f/255f, 48f/255f, 11f/255f, 170f/255f);
            circle.NetworkMovementSmoothing = 60;
            circle.transform.position = gasGrenadeObject.gameObject.transform.position;
            circle.transform.localScale = new Vector3(-8, -0.05f, -8);
            
            NetworkServer.Spawn(circle.gameObject);
            circle.UpdatePositionServer();
            
            int time = 0;
            List<Player> PlayersAlreadyAffected = new List<Player>();
            while (time != 30)
            {
                circle.transform.position = gasGrenadeObject.gameObject.transform.position;

                PlayersAlreadyAffected.Clear();
                foreach (var ply in Player.List.Where(x=> Vector3.Distance(x.Position, circle.transform.position) <= 4))
                {
                    if ((ply.Role.Team == Team.MTF || ply.Role.Team == Team.RSC || ply.Role.Team == Team.SCP) && !ply.IsCuffed)
                    {
                        if (PlayersAlreadyAffected.Contains(ply)) continue;
                        UtilityMethods.ApplyPoison(ply, Ply);
                        PlayersAlreadyAffected.Add(ply);
                    }
                }
                
                yield return Timing.WaitForSeconds(0.5f);
                time += 1;
            }
            
            NetworkServer.Destroy(circle.gameObject);
            gasGrenadeObject.Destroy();
        }
    }
}