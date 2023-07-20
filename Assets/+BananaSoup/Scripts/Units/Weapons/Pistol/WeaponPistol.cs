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

        protected override void Start()
        {
            base.Start();

            pool = new ComponentPool<PistolBullet>(bulletPrefab, capacity);
            pool.SetPooledObjectsParent(GetPooledProjectilesTransforms(), projectilePool);
        }

        public override void Fire()
        {
            if ( equipped )
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

        protected bool Recycle(PistolBullet item)
        {
            return pool.Recycle(item);
        }

        private void OnExpired(PistolBullet bullet)
        {
            bullet.Expired -= OnExpired;
            Recycle(bullet);
        }
        #endregion
    }
}
