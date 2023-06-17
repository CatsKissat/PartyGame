using System.Collections;
using UnityEngine;

namespace BananaSoup
{
    public class LaserProjectile : MonoBehaviour
    {
        [SerializeField]
        private float movementSpeed = 2.5f;

        [SerializeField]
        private float aliveTime = 5.0f;

        private Vector3 direction = Vector3.zero;
        private bool isLaunched = false;
        private float aliveTimer = 0;

        private Coroutine aliveTimerRoutine = null;

        private ProjectileMover projectileMover;

        private void Awake()
        {
            projectileMover = GetComponent<ProjectileMover>();

            if ( projectileMover == null )
            {
                Debug.LogError($"{name} doesn't have a ProjectileMover!");
            }
            Debug.Log("Projectile awake ready!");
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"{name} collided with {other.gameObject}!");
            DestroyThis();
        }

        private void FixedUpdate()
        {
            if ( isLaunched )
            {
                projectileMover.Move(direction);
            }
        }

        public void Setup(float speed = -1)
        {
            if ( speed < 0 )
            {
                speed = movementSpeed;
            }

            projectileMover.Setup(speed);
            isLaunched = false;

            Debug.Log("LaserProjectile setup ready!");
        }

        public void Launch(Vector3 direction)
        {
            this.direction = direction;
            isLaunched = true;

            aliveTimerRoutine = StartCoroutine(AliveTimer(aliveTime));
        }

        // TODO: Reimplement this to recycle instead of destroying.
        private void DestroyThis()
        {
            if ( aliveTimerRoutine != null )
            {
                StopCoroutine(aliveTimerRoutine);
                aliveTimerRoutine = null;
            }

            Destroy(gameObject);
            // TODO: Add event trigger to recycle back in.
        }

        private IEnumerator AliveTimer(float aliveTime)
        {
            aliveTimer = aliveTime;

            while (aliveTimer > 0 )
            {
                aliveTimer -= Time.deltaTime;
                yield return null;
            }

            DestroyThis();
        }
    }
}
