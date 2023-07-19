using System;
using UnityEngine;
using BananaSoup.Units;

namespace BananaSoup
{
    public class PistolBullet : ProjectileBase
    {
        // Event used to track Expiration of a projectile
        public event Action<PistolBullet> Expired;

        protected override void OnTriggerEnter(Collider other)
        {
            if ( (playersLayerMask.value & (1 << other.transform.gameObject.layer)) > 0 )
            {
                // TODO: Kill player
            }
            else
            {
                return;
            }

            base.OnTriggerEnter(other);
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
