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

        /// <summary>
        /// OnTriggerEnter used to track the objects the projectile collides with.
        /// If the other GameObject/Collider is on the player LayerMask and it has a 
        /// PlayerBase call TriggerEnterAction with the player as a parameter.
        /// Otherwise just call OnExpired().
        /// </summary>
        protected virtual void OnTriggerEnter(Collider other)
        {
            if ( (playersLayerMask.value & (1 << other.transform.gameObject.layer)) > 0
                && other.TryGetComponent(out PlayerBase player) )
            {
                TriggerEnterAction(player);
            }

            OnExpired();
        }

        /// <summary>
        /// Method used to determine the correct action(s) in OnTriggerEnter.
        /// Used in scripts that inherit this script.
        /// </summary>
        /// <param name="player">The colliding player.</param>
        protected virtual void TriggerEnterAction(PlayerBase player)
        {

        }

        /// <summary>
        /// Method used to move the projectile.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if ( isLaunched )
            {
                projectileMover.SetDirection(direction);
            }
        }

        /// <summary>
        /// Method used to setup the projectile with given parameters.
        /// </summary>
        /// <param name="aliveTime">The desired aliveTime of the projectile. (in seconds)</param>
        /// <param name="rotation">The desired rotation of the projectile.</param>
        /// <param name="playersLayerMask">The player LayerMask.</param>
        /// <param name="speed">The desired speed of the projectile.</param>
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

        /// <summary>
        /// Method used to launch the projectile in the given parameter Vector3 direction.
        /// </summary>
        /// <param name="direction">The desired direction for the projectile.</param>
        public void Launch(Vector3 direction)
        {
            this.direction = direction;
            isLaunched = true;
        }

        /// <summary>
        /// Method called when the projectile expires.
        /// Used to check if aliveTimerRoutine is not null, if it isn't
        /// stop and null it.
        /// </summary>
        protected virtual void OnExpired()
        {
            if ( aliveTimerRoutine != null )
            {
                StopCoroutine(aliveTimerRoutine);
                aliveTimerRoutine = null;
            }
        }

        /// <summary>
        /// Coroutine used to track the alive time of the projectile.
        /// While aliveTime is larger than 0 reduce it by Time.deltaTime and return null.
        /// After the aliveTime is less than 0 call the OnExpired() method.
        /// </summary>
        /// <param name="aliveTime">The desired aliveTime of the projectile.</param>
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
