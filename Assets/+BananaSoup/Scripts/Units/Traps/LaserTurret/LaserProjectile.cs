using System;
using System.Collections;
using UnityEngine;
using BananaSoup.Utils;
using BananaSoup.Modifiers;

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

        private float sizeIncrement = 0f;

        // Action variables
        private float slowAmount;
        private float slowDuration;

        private float stunDuration;

        private LayerMask playersLayerMask;
        private TrapModifierType.Modifier currentModifier = TrapModifierType.Modifier.Basic;

        // Coroutine used to store AliveTimerRoutine.
        private Coroutine aliveTimerRoutine = null;

        // References
        private ProjectileMover projectileMover;
        
        // Event used to track Expiration of a projectile
        public event Action<LaserProjectile> Expired;

        // Constants for modifier types for readability
        private const TrapModifierType.Modifier basicMod = TrapModifierType.Modifier.Basic;
        private const TrapModifierType.Modifier freezeMod = TrapModifierType.Modifier.Freeze;
        private const TrapModifierType.Modifier electricMod = TrapModifierType.Modifier.Electric;
        private const TrapModifierType.Modifier speedMod = TrapModifierType.Modifier.Speed;
        private const TrapModifierType.Modifier sizeMod = TrapModifierType.Modifier.Size;

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
            if ( (playersLayerMask.value & (1 << other.transform.gameObject.layer)) > 0 )
            {
                DetermineModAction(other);
            }
            else
            {
                return;
            }

            OnExpired();
        }

        private void FixedUpdate()
        {
            if ( isLaunched )
            {
                projectileMover.Move(direction);
            }
        }

        public void SetupModifierVariables(LayerMask playersLayerMask, float slowAmount,
                                                    float slowDuration, float stunDuration,
                                                    TrapModifierType.Modifier modifier, float sizeModifier)
        {
            currentModifier = modifier;
            this.playersLayerMask = playersLayerMask;

            this.slowAmount = slowAmount;
            this.slowDuration = slowDuration;

            this.stunDuration = stunDuration;

            CheckIfScaleShouldIncrease(sizeModifier);
        }

        private void CheckIfScaleShouldIncrease(float amountToIncrease)
        {
            if ( amountToIncrease > 0f && sizeIncrement == 0f)
            {
                Vector3 originalScale = transform.localScale;
                Vector3 modifiedSize = originalScale + new Vector3(amountToIncrease, amountToIncrease, amountToIncrease);
                transform.localScale = modifiedSize;
                sizeIncrement = amountToIncrease;
            }
            else
            {
                return;
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

        /// <summary>
        /// Used in OnTriggerEnter to determine correct action(s) to the player.
        /// Basic, Speed and Size modifiers just kill the player.
        /// Freeze mod freezes the player.
        /// Electric mod stuns the player.
        /// Default case is a bug, where the trap has no active modifier.
        /// </summary>
        /// <param name="other">The other object which the trap is colliding with.</param>
        private void DetermineModAction(Collider other)
        {
            switch ( currentModifier )
            {
                case basicMod:
                case speedMod:
                case sizeMod:
                    {
                        Debug.Log($"{other.name} should get killed!");
                        break;
                    }
                case freezeMod:
                    {
                        Debug.Log($"{other.name} should get frozen!");
                        break;
                    }
                case electricMod:
                    {
                        Debug.Log($"{other.name} should get stunned!");
                        break;
                    }
                default:
                    {
                        Debug.LogError($"{name} has no active modifier! This is a bug!");
                        break;
                    }
            }
        }
    }
}
