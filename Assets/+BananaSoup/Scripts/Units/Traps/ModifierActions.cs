using UnityEngine;
using BananaSoup.Modifiers;
using BananaSoup.Units;

namespace BananaSoup.Traps
{
    public class ModifierActions : MonoBehaviour
    {
        [Header("Modifier variables")]
        [SerializeField, Tooltip("Bonus speed for the traps action/projectile.")]
        protected float speed = 0.5f;

        [Space]

        [SerializeField, Tooltip("How much should the traps size/projectile size be changed by.")]
        protected float sizeChange = 0.5f;

        [Space]

        [SerializeField, Tooltip("The percentage how much the slow should affect the player(s).")]
        protected float slowMultiplier = 0.5f;
        [SerializeField, Tooltip("The duration of the slow effect.")]
        protected float slowDuration = 2.5f;

        [Space]

        [SerializeField, Tooltip("The duration of the stun effect.")]
        protected float stunDuration = 1.5f;

        [Header("Player variables")]
        [SerializeField]
        protected LayerMask playersLayerMask;

        // Variable used to store the currentModifier of the trap.
        protected TrapModifierType.Modifier currentModifier;

        // References
        private TrapBase trapBase;

        public LayerMask PlayersLayerMask => playersLayerMask;
        public float SlowAmount => slowMultiplier;
        public float SlowDuration => slowDuration;
        public float StunDuration => stunDuration;

        // Constants for modifier types for readability
        protected const TrapModifierType.Modifier basicMod = TrapModifierType.Modifier.Basic;
        protected const TrapModifierType.Modifier freezeMod = TrapModifierType.Modifier.Freeze;
        protected const TrapModifierType.Modifier electricMod = TrapModifierType.Modifier.Electric;
        protected const TrapModifierType.Modifier speedMod = TrapModifierType.Modifier.Speed;
        protected const TrapModifierType.Modifier sizeMod = TrapModifierType.Modifier.Size;

        /// <summary>
        /// Method used to get a reference of the objects TrapBase component.
        /// </summary>
        private void GetTrapBaseReference()
        {
            trapBase = GetComponent<TrapBase>();
            if ( trapBase == null )
            {
                Debug.LogError($"{name} doesn't have a TrapBase component!");
            }
        }

        /// <summary>
        /// Method called from TrapBase to setup the modifier and check if the modifier
        /// is a size or a speed modifier and then call TrapBase's Setup method.
        /// </summary>
        public void SetupModifier()
        {
            GetTrapBaseReference();

            currentModifier = trapBase.TrapModifier;

            CheckForSizeOrSpeedMod();

            trapBase.Setup();
        }

        /// <summary>
        /// Used to check if an object on the playersLayerMask is entering the trigger.
        /// If yes then call the DetermineModAction method.
        /// </summary>
        /// <param name="other">The other GameObjects collider, usually a players.</param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            if ((playersLayerMask.value & (1 << other.transform.gameObject.layer)) > 0
                && other.TryGetComponent(out PlayerBase player))
            {
                DetermineModAction(player);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Used in OnTriggerEnter to determine correct action(s) to the player.
        /// Basic, Speed and Size modifiers just kill the player.
        /// Freeze mod freezes the player.
        /// Electric mod stuns the player.
        /// Default case is a bug, where the trap has no active modifier.
        /// </summary>
        /// <param name="player">The PlayerBase of the targeted player.</param>
        protected virtual void DetermineModAction(PlayerBase player)
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

        /// <summary>
        /// Used in Setup() on Start() to check if a speed or size mod is active.
        /// If either is active then change the speed/size accordingly and continue.
        /// If not, then just continue with default values.
        /// </summary>
        private void CheckForSizeOrSpeedMod()
        {
            switch ( currentModifier )
            {
                case speedMod:
                    {
                        Debug.Log("Trap should act faster / projectiles should be faster!");
                        trapBase.ModifiedSpeed = speed;
                        break;
                    }
                case sizeMod:
                    {
                        Debug.Log("Trap should be bigger / in a wider area!");
                        trapBase.ModifiedSize = sizeChange;
                        break;
                    }
                default:
                    {
                        Debug.Log("No speed or size mod active, keep default values.");
                        break;
                    }
            }
        }
    }
}
