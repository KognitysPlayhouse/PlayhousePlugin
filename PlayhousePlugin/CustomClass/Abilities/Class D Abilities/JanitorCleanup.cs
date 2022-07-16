using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Mirror;
using UnityEngine;

namespace PlayhousePlugin.CustomClass.Abilities
{
    public class JanitorCleanup : NonCooldownAbilityBase
    {
        public override string Name { get; } = "Body Cleanup";
        public override Player Ply { get; }

        public JanitorCleanup(Player ply)
        {
            Ply = ply;
        }
        
        public override bool UseAbility()
        {
            List<Collider> colliders = Physics.OverlapSphere(Ply.Position, 3f, LayerMask.GetMask("Ragdoll")).Where(e => e.gameObject.GetComponentInParent<Ragdoll>() != null).ToList();
            
            colliders.Sort((x, y) => Vector3.Distance(x.gameObject.transform.position, Ply.Position).CompareTo(Vector3.Distance(y.gameObject.transform.position, Ply.Position)));

            if (colliders.Count == 0)
            {
	            Ply.ShowCenterDownHint($"<color=yellow>There are no bodies nearby</color>",3);
            	return false;
            }

            Ragdoll doll = colliders[0].gameObject.GetComponentInParent<Ragdoll>();

            int chance;
            List<ItemType> CardItems = new List<ItemType> { ItemType.Coin, ItemType.KeycardScientist, ItemType.KeycardZoneManager, ItemType.KeycardResearchCoordinator, ItemType.KeycardNTFOfficer, ItemType.KeycardContainmentEngineer, ItemType.KeycardO5 };

            chance = EventHandler.random.Next(0, 100);

            if (chance <= 100 && chance > 50)
            {
	            Ply.AddItem(ItemType.Coin);
            }
            else if (chance <= 50 && chance > 25)
            {
	            Ply.AddItem(ItemType.KeycardScientist);
            }
            else if (chance <= 25 && chance > 13)
            {
	            Ply.AddItem(ItemType.KeycardZoneManager);
            }
            else if (chance <= 13 && chance > 6)
            {
	            Ply.AddItem(ItemType.KeycardResearchCoordinator);
            }
            else if (chance <= 6 && chance > 3)
            {
	            Ply.AddItem(ItemType.KeycardNTFOfficer);
            }
            else if (chance <= 3 && chance > 2)
            {
	            Ply.AddItem(ItemType.KeycardContainmentEngineer);
            }
            else
            {
	            Ply.AddItem(ItemType.KeycardO5);
            }
            
            Ply.ShowCenterDownHint($"<color=yellow>Body Cleaned!</color>",3);
            NetworkServer.Destroy(doll.gameObject);
            return true;
        }
    }
}