using System.Collections;
using UnityEngine;

namespace BananaSoup
{
    public class LaserTurretWeapon : MonoBehaviour
    {
        [SerializeField]
        private LaserProjectile projectile;

        [SerializeField]
        private float spawnRate = 1.0f;

        private Coroutine shootCoroutine = null;

        private void OnDisable()
        {
            TryStopAndNullCoroutine();
        }

        private void Awake()
        {
            // TODO: Initialize pool here.
        }

        private void Start()
        {
            Debug.Log("Starting LaserTurretWeapon ShootCoroutine!");
            shootCoroutine = StartCoroutine(ShootCoroutine());
        }

        private IEnumerator ShootCoroutine()
        {
            while ( true )
            {
                // TODO: Get projectile from pool here
                if ( projectile != null )
                {
                    //projectile.transform.position = transform.position;
                    //projectile.transform.rotation = transform.rotation;

                    Instantiate(projectile, transform.position, projectile.transform.rotation);

                    projectile.Setup();

                    projectile.Launch(transform.up);
                }

                yield return new WaitForSeconds(spawnRate);
            }
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
