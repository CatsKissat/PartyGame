using System;
using BananaSoup.Units;

namespace BananaSoup.Weapons
{
    public class PistolBullet : ProjectileBase
    {
        // Event used to track Expiration of a projectile
        public event Action<PistolBullet> Expired;

        /// <summary>
        /// Method called in the inherited OnTriggerEnter.
        /// Used to kill the colliding player.
        /// </summary>
        /// <param name="player"></param>
        protected override void TriggerEnterAction(PlayerBase player)
        {
            player.Kill();
        }

        /// <summary>
        /// Method called when the projectile expires.
        /// </summary>
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
