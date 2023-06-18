using System.Collections;
using UnityEngine;
using BananaSoup.Utils;

namespace BananaSoup.Traps
{
    public class LaserTurretWeapon : PooledSpawner<LaserProjectile>
    {
        [SerializeField]
        private float spawnRate = 1.0f;

        [Space]

        [SerializeField]
        private float projectileAliveTime = 2.5f;

        private Coroutine shootCoroutine = null;

        // HACK
        private Vector3 spawnRotation = new Vector3(0f, 0f, 90f);

        private void OnDisable()
        {
            TryStopAndNullCoroutine();
        }

        private void Start()
        {
            Setup();

            shootCoroutine = StartCoroutine(ShootCoroutine());
        }

        private IEnumerator ShootCoroutine()
        {
            while ( true )
            {
                LaserProjectile projectile = Create(transform.position, Quaternion.Euler(spawnRotation), null);

                if ( projectile != null )
                {
                    projectile.Setup(projectileAliveTime);

                    projectile.Launch(transform.forward);

                    projectile.Expired += OnExpired;
                }

                yield return new WaitForSeconds(spawnRate);
            }
        }

        private void OnExpired(LaserProjectile projectile)
        {
            projectile.Expired -= OnExpired;
            Recycle(projectile);
        }

        private void TryStopAndNullCoroutine()
        {
            if ( shootCoroutine != null )
            {
                StopCoroutine(shootCoroutine);
                shootCoroutine = null;
            }
        }
    }
}
