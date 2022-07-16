using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs;
using UnityEngine;

namespace PlayhousePlugin
{
    public class InfiniteAmmoComponent : MonoBehaviour
    {
        private Player Hub;
        public void Awake()
        {
            Hub = Player.Get(gameObject);
            Exiled.Events.Handlers.Player.Shooting += RunWhenPlayerShoots;
            Exiled.Events.Handlers.Player.DroppingItem += RunWhenPlayerDropsItem;
        }

        public void OnDestroy()
		{
			Hub = null;
            Exiled.Events.Handlers.Player.DroppingItem -= RunWhenPlayerDropsItem;
            Exiled.Events.Handlers.Player.Shooting -= RunWhenPlayerShoots;
        }

        public void RunWhenPlayerShoots(ShootingEventArgs s)
        {
            if (s.Shooter.ReferenceHub != Hub.ReferenceHub)
                return;

            //ModifyAmmo(s.Shooter.ReferenceHub, 999);
			var f = s.Shooter.CurrentItem as Firearm;

			f.Ammo = byte.MaxValue;
        }

        public void RunWhenPlayerDropsItem(DroppingItemEventArgs d)
        {
			d.IsAllowed = false;
        }
    }
}
