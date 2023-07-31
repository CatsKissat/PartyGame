using System.Collections;
using UnityEngine;
using BananaSoup.Units;

namespace BananaSoup.Traps
{
    [RequireComponent(typeof(LaserProjectileSpawner))]
    public class LaserTurret : TrapBase
    {
        [Space]
        [SerializeField, Tooltip("How of then the prefabs should be fired.")]
        private float fireRate = 1.0f;

        [Space]

        [SerializeField, Tooltip("The point from which the projectiles are fired.")]
        private GameObject firingPoint = null;

        [Space]

        [SerializeField, Tooltip("The speed of the projectile fired.")]
        private float projectileSpeed = 3.0f;
        [SerializeField, Tooltip("The time a projectile should stay alive.")]
        private float projectileAliveTime = 2.5f;

        // Coroutine used to store the ShootCoroutine which can be disabled and
        // nulled if the object is disabled.
        private Coroutine shootCoroutine = null;
        
        // References
        private LaserProjectileSpawner spawner = null;

        private void OnDisable()
        {
            TryStopAndNullCoroutine(shootCoroutine);
        }

        /// <summary>
        /// Get reference to the LaserProjectileSpawner, if it can't be found log an error.
        /// </summary>
        private void Awake()
        {
            spawner = GetComponent<LaserProjectileSpawner>();
            if ( spawner == null )
            {
                Debug.LogError($"{name} doesn't have a component of type LaserProjectileSpawner!");
            }
        }

        public override void Setup()
        {
            if ( ModifiedSpeed > 0 )
            {
                projectileSpeed += ModifiedSpeed;
            }

            spawner.Setup();
            shootCoroutine = StartCoroutine(ShootCoroutine());
        }

        /// <summary>
        /// Routine used to loop the firing of the projectiles.
        /// Create a projectile with set parameters, if the projectile is not null then
        /// call the projectiles Setup with desired alive time and projectile speed.
        /// Then call the projectiles Launch method with desired direction and then
        /// subscribe to the projectiles Expired event with OnExpired method.
        /// </summary>
        private IEnumerator ShootCoroutine()
        {
            while ( true )
            {
                ProjectileBase projectile = spawner.Create(firingPoint.transform.position);

                if ( projectile != null )
                {
                    projectile.Setup(projectileAliveTime, transform.rotation.eulerAngles, playersLayerMask, projectileSpeed);

                    projectile.Launch(transform.right);

                    projectile.Expired += OnExpired;
                }

                yield return new WaitForSeconds(fireRate);
            }
        }

        /// <summary>
        /// Method called when a projectiles Expired event is called.
        /// Recycle the projectile when this happens.
        /// </summary>
        /// <param name="projectile">The projectile in question.</param>
        private void OnExpired(ProjectileBase projectile)
        {
            projectile.Expired -= OnExpired;
            spawner.Recycle(projectile);
        }

        /// <summary>
        /// Method used to try to stop and null a coroutine.
        /// </summary>
        /// <param name="routine">The routine you want to try to stop and null.</param>
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
