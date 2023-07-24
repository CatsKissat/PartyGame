using System;
using UnityEngine;
using UnityEngine.InputSystem;
using BananaSoup.Utils;

namespace BananaSoup.Units
{
    public class PlayerBase : UnitBase
    {
        [SerializeField]
        protected float stunCooldown = 1.0f;

        private bool isStunned = false;
        private bool isFrozen = false;
        private bool isDead = false;

        private PlayerInput playerInput;
        private int playerID;
        private PlayerSpriteSelector playerSpriteSelector;
        private PlayerMovement playerMovement;
        private CameraTargetAssigner cameraTargetAssigner;
        private PlayerActionMapSelector actionMapSelector;

        public int PlayerID => playerID;
        public bool IsStunned => isStunned;
        public bool IsFrozen => isFrozen;
        public bool IsDead => isDead;

        public event Action<float> Stunned;
        public event Action<float, float> Frozen;
        public event Action<float> FrozenContinuously;
        public event Action Killed;
        public event Action Continue;

        public virtual void PlayerFinished() { }

        protected virtual void OnEnable()
        {
            TryGetReferences();
            playerID = playerInput.playerIndex;

            playerMovement.LeaveGame += OnLeave;

            GameObject cameraTargetGroup = GameObject.FindGameObjectWithTag("CameraTargetGroup");
            if ( cameraTargetGroup == null )
            {
                Debug.LogError($"{name} is missing reference to CameraTargetGroup GameObject!");
            }

            cameraTargetAssigner = cameraTargetGroup.GetComponent<CameraTargetAssigner>();
            if ( cameraTargetAssigner == null )
            {
                Debug.LogError($"{name} is missing reference to the CameraTargetAssigner!");
            }

            cameraTargetAssigner.AssingPlayer(transform);
        }

        private void OnLeave()
        {
            cameraTargetAssigner.RemovePlayerTarget(transform);
        }

        protected override void Start()
        {
            base.Start();

            GetReferences();
            playerSpriteSelector.AssingSprite();
        }

        private void TryGetReferences()
        {
            if ( playerInput == null )
            {
                playerInput = GetComponent<PlayerInput>();

                if ( playerInput == null )
                {
                    Debug.LogError($"{name} is missing a PlayerInput!");
                }
            }

            if ( playerMovement == null )
            {
                playerMovement = GetComponent<PlayerMovement>();
                if ( playerMovement == null )
                {
                    Debug.LogError($"{name} is missing a PlayerMovement!");
                }
            }
        }

        private void GetReferences()
        {
            playerSpriteSelector = GetComponent<PlayerSpriteSelector>();
            if ( playerSpriteSelector == null )
            {
                Debug.LogError($"{name} is missing a PlayerSpriteSelector!");
            }

            actionMapSelector = GetComponent<PlayerActionMapSelector>();
            if ( actionMapSelector == null )
            {
                Debug.LogError($"{name} is missing a reference to PlayerActionMapSelector!");
            }
        }

        public void Stun(float duration)
        {
            if ( !isStunned )
            {
                isStunned = true;
            }

            if ( Stunned != null )
            {
                Stunned(duration);
            }
        }

        public void SetIsStunnedFalse()
        {
            isStunned = false;
        }

        public void Freeze(float duration, float slowMultiplier)
        {
            if ( !isFrozen )
            {
                isFrozen = true;
            }

            if ( Frozen != null )
            {
                Frozen(duration, slowMultiplier);
            }
        }

        public void FreezeContinously(float slowMultiplier)
        {
            if ( !isFrozen )
            {
                isFrozen = true;
            }

            if ( FrozenContinuously != null )
            {
                FrozenContinuously(slowMultiplier);
            }
        }

        public void SetIsFrozenFalse()
        {
            if ( isFrozen )
            {
                isFrozen = false;
            }
        }

        public void Kill()
        {
            if ( !isDead )
            {
                isDead = true;

                if ( Killed != null )
                {
                    Killed();

                    SetActionMapToScoreboard();
                }
            }
        }

        public void SetActionMapToScoreboard()
        {
            actionMapSelector.SetActionMapOnDeath();
        }

        public void Pushback(Vector3 direction, float pushbackStrength)
        {
            rb.AddForce(direction * pushbackStrength);
        }

        /// <summary>
        /// Sets the position of the player.
        /// </summary>
        /// <param name="newPosition">New position of the player.</param>
        public void SetPosition(Transform newPosition)
        {
            transform.position = newPosition.position;
        }

        /// <summary>
        /// Sets correct settings for the player when new round is being called.
        /// </summary>
        public void InitializePlayerOnNewRound()
        {
            isDead = false;
            actionMapSelector.SetActionMapOnNewRound();
        }

        public void OnContinue(InputAction.CallbackContext context)
        {
            if ( context.performed )
            {
                Continue();
            }
        }
    }
}
