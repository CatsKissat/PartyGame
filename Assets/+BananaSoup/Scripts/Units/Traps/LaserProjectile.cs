using System;
using System.Collections;
using UnityEngine;
using BananaSoup.Utils;

namespace BananaSoup.Traps
{
    public class LaserProjectile : MonoBehaviour
    {
        [SerializeField]
        private float movementSpeed = 2.5f;

        [SerializeField]
        private float aliveTime = 5.0f;

        private Vector3 direction = Vector3.zero;
        private bool isLaunched = false;

        private Coroutine aliveTimerRoutine = null;

        private ProjectileMover projectileMover;

        public event Action<LaserProjectile> Expired;

        private void Awake()
        {
            projectileMover = GetComponent<ProjectileMover>();

            if ( projectileMover == null )
            {
                Debug.LogError($"{name} doesn't have a ProjectileMover!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            OnExpired();
        }

        private void FixedUpdate()
        {
            if ( isLaunched )
            {
                projectileMover.Move(direction);
            }
        }

        public void Setup(float aliveTime, float speed = -1)
        {
            if ( speed < 0 )
            {
                speed = movementSpeed;
            }

            projectileMover.Setup(speed);
            isLaunched = false;

            aliveTimerRoutine = StartCoroutine(AliveTimer(aliveTime));
        }

        public void Launch(Vector3 direction)
        {
            this.direction = direction;
            isLaunched = true;
        }

        private void OnExpired()
        {
            if ( aliveTimerRoutine != null )
            {
                StopCoroutine(aliveTimerRoutine);
                aliveTimerRoutine = null;
            }

            if (Expired != null )
            {
                Expired(this);
            }
        }

        private IEnumerator AliveTimer(float aliveTime)
        {
            this.aliveTime = aliveTime;

            while (this.aliveTime > 0)
            {
                this.aliveTime -= Time.deltaTime;
                yield return null;
            }

            OnExpired();
        }
    }
}
