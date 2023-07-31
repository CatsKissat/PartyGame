using BananaSoup.Units;
using BananaSoup.Utils;

namespace BananaSoup.Weapons
{
    public class WeaponPistol : WeaponBase
    {
        /// <summary>
        /// Method called to fire a projectile from the weapon.
        /// First Create a projectile to the firingPoint.
        /// Then if the projectile is not null Setup the projectile with required parameters.
        /// Then Launch the projectile in the desired direction and make the projectiles
        /// Expired event listen to this components OnExpired.
        /// </summary>
        public override void Fire()
        {
            if ( bulletsLeft <= 0 )
            {
                return;
            }

            if ( onCooldown )
            {
                return;
            }

            if ( equippedByAPlayer )
            {
                ProjectileBase projectile = Create(firingPoint.transform.position);

                if ( projectile != null )
                {
                    projectile.Setup(projectileAliveTime, transform.rotation.eulerAngles, playersLayerMask, projectileSpeed);

                    projectile.Launch(transform.right);

                    projectile.Expired += OnExpired;

                    ReduceBulletsLeft();
                }
            }

            if ( fireRateRoutine == null )
            {
                fireRateRoutine = StartCoroutine(FireRateRoutine()); 
            }
        }
    }
}
