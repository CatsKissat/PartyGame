using System;
using UnityEngine;
using UnityEngine.InputSystem;
using BananaSoup.Utils;

namespace BananaSoup.Units
{
    public class PlayerBase : UnitBase
    {
        private bool isStunned = false;
        private bool isFrozen = false;
        private bool isDead = false;

        private PlayerInput playerInput;
        private int playerID;
        private PlayerSpriteSelector playerSpriteSelector;
        private PlayerMovement playerMovement;
        private CameraTargetAssigner cameraTargetAssigner;

        public int PlayerID => playerID;
        public bool IsStunned => isStunned;
        public bool IsFrozen => isFrozen;
        public bool IsDead => isDead;

        public event Action<float> Stunned;
        public event Action<float, float> Frozen;
        public event Action Killed;

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

            Debug.Log($"PlayerID {playerID} joined the game.");
        }

        private void OnLeave()
        {
            cameraTargetAssigner.RemovePlayerTarget(transform);

            Debug.Log($"PlayerID {playerID} left the game.");
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
                Debug.Log($"{name} is missing a PlayerSpriteSelector!");
            }
        }

        public void Stun(float duration)
        {
            Stunned(duration);
        }

        public void Freeze(float duration, float slowMultiplier)
        {
            Frozen(duration, slowMultiplier);
        }

        public void Kill()
        {
            if ( !isDead )
            {
                isDead = true;
                Killed();
            }
        }
    }
}
