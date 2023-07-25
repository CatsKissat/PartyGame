using System;
using UnityEngine;
using BananaSoup.Units;

namespace BananaSoup.Weapons
{
    public class PistolBullet : ProjectileBase
    {
        // Event used to track Expiration of a projectile
        public event Action<PistolBullet> Expired;

        protected override void OnTriggerEnter(Collider other)
        {
            if ( (playersLayerMask.value & (1 << other.transform.gameObject.layer)) > 0 
                && other.TryGetComponent(out PlayerBase player))
            {
                player.Kill();
            }

            OnExpired();
        }

        protected override void OnExpired()
        {
            base.OnExpired();

            if ( Expired != null )
            {
                Expired(this);
            }
        }
    }
}
