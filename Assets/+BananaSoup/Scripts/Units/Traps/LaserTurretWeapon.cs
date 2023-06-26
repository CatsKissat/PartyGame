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
        private float projectileSpeed = 3.0f;
        [SerializeField]
        private float projectileAliveTime = 2.5f;

        private Coroutine shootCoroutine = null;

        private GameObject spriteRendererObject = null;
        private Vector3 spriteRotation = Vector3.zero;

        private void OnDisable()
        {
            TryStopAndNullCoroutine(shootCoroutine);
        }

        private void Awake()
        {
            spriteRendererObject = transform.parent.GetComponentInChildren<SpriteRenderer>(true).gameObject;

            if ( spriteRendererObject == null )
            {
                Debug.LogError($"{name} couldn't find a object with SpriteRenderer from it's transform!");
            }

            spriteRotation = spriteRendererObject.transform.rotation.eulerAngles;
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
                LaserProjectile projectile = Create(transform.position, Quaternion.Euler(spriteRotation), null);

                if ( projectile != null )
                {
                    projectile.Setup(projectileAliveTime, projectileSpeed);

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

        private void TryStopAndNullCoroutine(Coroutine routine)
        {
            if ( routine != null )
            {
                StopCoroutine(routine);
                routine = null;
            }
        }
    }
}
