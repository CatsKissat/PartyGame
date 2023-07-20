using System.Collections;
using UnityEngine;
using BananaSoup.Utils;

namespace BananaSoup.Units
{
    public class ProjectileBase : MonoBehaviour
    {
        [SerializeField]
        protected float movementSpeed = 2.5f;

        [SerializeField]
        protected float aliveTime = 5.0f;

        protected Vector3 direction = Vector3.zero;
        protected bool isLaunched = false;

        protected LayerMask playersLayerMask;

        // Coroutine used to store AliveTimerRoutine.
        protected Coroutine aliveTimerRoutine = null;

        // References
        protected ProjectileMover projectileMover;

        private void Awake()
        {
            projectileMover = GetComponent<ProjectileMover>();

            if ( projectileMover == null )
            {
                Debug.LogError($"{name} doesn't have a ProjectileMover!");
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            
        }

        private void FixedUpdate()
        {
            if ( isLaunched )
            {
                projectileMover.Move(direction);
            }
        }

        public void Setup(float aliveTime, Vector3 rotation, LayerMask playersLayerMask, float speed = -1)
        {
            if ( speed < 0 )
            {
                speed = movementSpeed;
            }

            this.playersLayerMask = playersLayerMask;

            transform.rotation = Quaternion.Euler(rotation);

            projectileMover.Setup(speed);
            isLaunched = false;

            aliveTimerRoutine = StartCoroutine(AliveTimer(aliveTime));
        }

        public void Launch(Vector3 direction)
        {
            this.direction = direction;
            isLaunched = true;
        }

        protected virtual void OnExpired()
        {
            if ( aliveTimerRoutine != null )
            {
                StopCoroutine(aliveTimerRoutine);
                aliveTimerRoutine = null;
            }
        }

        private IEnumerator AliveTimer(float aliveTime)
        {
            this.aliveTime = aliveTime;

            while ( this.aliveTime > 0 )
            {
                this.aliveTime -= Time.deltaTime;
                yield return null;
            }

            OnExpired();
        }
    }
}
