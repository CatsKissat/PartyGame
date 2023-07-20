using System;
using UnityEngine;
using BananaSoup.Modifiers;
using BananaSoup.Units;

namespace BananaSoup.Traps
{
    public class LaserProjectile : ProjectileBase
    {
        private float sizeIncrement = 0f;

        // Action variables
        private float slowMultiplier;
        private float slowDuration;

        private float stunDuration;

        private TrapModifierType.Modifier currentModifier = TrapModifierType.Modifier.Basic;

        // Event used to track Expiration of a projectile
        public event Action<LaserProjectile> Expired;

        // Constants for modifier types for readability
        private const TrapModifierType.Modifier basicMod = TrapModifierType.Modifier.Basic;
        private const TrapModifierType.Modifier freezeMod = TrapModifierType.Modifier.Freeze;
        private const TrapModifierType.Modifier electricMod = TrapModifierType.Modifier.Electric;
        private const TrapModifierType.Modifier speedMod = TrapModifierType.Modifier.Speed;
        private const TrapModifierType.Modifier sizeMod = TrapModifierType.Modifier.Size;

        protected override void OnTriggerEnter(Collider other)
        {
            if ( (playersLayerMask.value & (1 << other.transform.gameObject.layer)) > 0
                && other.TryGetComponent(out PlayerBase player) )
            {
                DetermineModAction(player);
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

        public void SetupModifierVariables(float slowDuration, float slowMultiplier, float stunDuration,
                                                    TrapModifierType.Modifier modifier,float sizeModifier)
        {
            currentModifier = modifier;

            this.slowDuration = slowDuration;
            this.slowMultiplier = slowMultiplier;

            this.stunDuration = stunDuration;

            CheckIfScaleShouldIncrease(sizeModifier);
        }

        private void CheckIfScaleShouldIncrease(float amountToIncrease)
        {
            if ( amountToIncrease > 0f && sizeIncrement == 0f )
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

        protected override void OnExpired()
        {
            base.OnExpired();

            if ( Expired != null )
            {
                Expired(this);
            }
        }

        /// <summary>
        /// Used in OnTriggerEnter to determine correct action(s) to the player.
        /// Basic, Speed and Size modifiers just kill the player.
        /// Freeze mod freezes the player.
        /// Electric mod stuns the player.
        /// Default case is a bug, where the trap has no active modifier.
        /// </summary>
        /// <param name="player">The target gameObject with a PlayerBase.</param>
        private void DetermineModAction(PlayerBase player)
        {
            switch ( currentModifier )
            {
                case basicMod:
                case speedMod:
                case sizeMod:
                    {
                        player.Kill();

                        break;
                    }
                case freezeMod:
                    {
                        player.Freeze(slowDuration, slowMultiplier);

                        break;
                    }
                case electricMod:
                    {
                        player.Stun(stunDuration);

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
