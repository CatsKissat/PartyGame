using UnityEngine;
using System.Collections.Generic;
using BananaSoup.Utils;

namespace BananaSoup.Weapons
{
    public class WeaponPistol : WeaponBase
    {
        [SerializeField]
        private PistolBullet bulletPrefab;

        private new ComponentPool<PistolBullet> pool;

        /// <summary>
        /// Call the inherited WeaponBase start with base.Start();
        /// Then setup the pool for this pistol.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            pool = new ComponentPool<PistolBullet>(bulletPrefab, capacity);
            pool.SetPooledObjectsParent(GetPooledProjectilesTransforms(), projectilePool);
        }

        /// <summary>
        /// Method called to fire a projectile from the weapon.
        /// First Create a projectile to the firingPoint.
        /// Then if the projectile is not null Setup the projectile with required parameters.
        /// Then Launch the projectile in the desired direction and make the projectiles
        /// Expired event listen to this components OnExpired.
        /// </summary>
        public override void Fire()
        {
            if ( equippedByAPlayer )
            {
                PistolBullet projectile = Create(firingPoint.transform.position);

                if ( projectile != null )
                {
                    projectile.Setup(projectileAliveTime, transform.rotation.eulerAngles, playersLayerMask, projectileSpeed);

                    projectile.Launch(transform.right);

                    projectile.Expired += OnExpired;
                }
            }
        }

        #region Pool
        /// <summary>
        /// Method used to get all the projectiles in the pool of this object.
        /// Then get the transforms of those objects and return the array of transforms.
        /// </summary>
        /// <returns>A Transform array of the projectiles transforms.</returns>
        protected override Transform[] GetPooledProjectilesTransforms()
        {
            List<PistolBullet> projectiles = pool.GetAllItems();
            Transform[] projectileTransforms = new Transform[capacity];

            for ( int i = 0; i < capacity; i++ )
            {
                projectileTransforms[i] = projectiles[i].transform;
            }

            return projectileTransforms;
        }

        protected new PistolBullet Create(Vector3 position)
        {
            PistolBullet item = pool.Get();
            if ( item != null )
            {
                item.transform.position = position;
            }

            return item;
        }

        /// <summary>
        /// Method used to call the pools Recycle method.
        /// </summary>
        /// <param name="projectile">The projectile to recycle.</param>
        /// <returns>True if a projectile was recycled, false if not.</returns>
        protected bool Recycle(PistolBullet projectile)
        {
            return pool.Recycle(projectile);
        }

        /// <summary>
        /// Stop listening to this components OnExpired on the projectiles Expired event.
        /// Recycle the projectile.
        /// </summary>
        /// <param name="projectile">The projectile to recycle.</param>
        private void OnExpired(PistolBullet projectile)
        {
            projectile.Expired -= OnExpired;
            Recycle(projectile);
        }
        #endregion
    }
}
