using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;

namespace BananaSoup.Units
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 40.0f;
        [SerializeField] private float runSpeed = 60.0f;

        public UnityAction LeaveGame;

        private float currentWalkSpeed = 0f;
        private float currentRunSpeed = 0f;
        private PlayerCharacterController controller;
        private Animator animator;
        private float moveSpeed = 0.0f;
        private float horizontalMove = 0.0f;
        private float moveInput;
        private bool jump = false;
        private bool isRunning = false;
        private PlayerInput playerInput;
        private Coroutine freezeRoutine = null;

        private void OnEnable()
        {
            playerInput = GetComponent<PlayerInput>();
            if ( playerInput == null )
            {
                Debug.LogError($"{name}'s PlayerInput is null!");
            }
        }

        private void OnDisable()
        {
            controller.Frozen -= FreezePlayerOnExit;
            controller.FrozenContinuously -= FreezePlayerContinuously;
            TryStopCoroutine(ref freezeRoutine);
        }

        private void Start()
        {
            GetReferences();

            currentWalkSpeed = walkSpeed;
            currentRunSpeed = runSpeed;

            moveSpeed = currentWalkSpeed;

            controller.Frozen += FreezePlayerOnExit;
            controller.FrozenContinuously += FreezePlayerContinuously;
        }

        void Update()
        {
            horizontalMove = moveInput * moveSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }

        void FixedUpdate()
        {
            // Check if the player is dead, if yes don't allow movement or jumping.
            if ( controller.IsDead )
            {
                return;
            }

            // Move our character
            controller.Move(horizontalMove, jump);
            jump = false;
        }

        private void GetReferences()
        {
            controller = GetComponent<PlayerCharacterController>();
            if ( controller == null )
            {
                Debug.LogError($"{name} is missing reference to a PlayerCharacterController!");
            }

            animator = GetComponent<Animator>();
            if ( animator == null )
            {
                Debug.LogError($"{name} is missing reference to a Animator!");
            }
        }

        #region OnInputs
        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<float>();
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if ( context.performed )
            {
                moveSpeed = currentRunSpeed;
                isRunning = true;
            }
            else if ( context.canceled )
            {
                moveSpeed = currentWalkSpeed;
                isRunning = false;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if ( context.performed )
            {
                jump = true;
            }
        }

        public void OnLeaveGame(InputAction.CallbackContext context)
        {
            if ( context.performed )
            {
                LeaveGame.Invoke();

                // NOTE: Fast fix to handle Unity getting bloated to destroy the player GameObject instead of pooling it.
                //DisablePlayerGameObject();
                DestroyPlayerGameObject();
            }
        }
        #endregion OnInputs

        public void OnDeviceLost()
        {
            //DisablePlayerGameObject();
            Debug.LogWarning($"PlayerID {playerInput.playerIndex} input device disconnected.");
        }

        public void OnDeviceReconnect()
        {
            Debug.LogWarning($"PlayerID {playerInput.playerIndex} reconnected.");
        }

        private void DisablePlayerGameObject()
        {
            gameObject.SetActive(false);
        }

        private void DestroyPlayerGameObject()
        {
            Destroy(gameObject);
        }

        // TODO: Move animation calls to corresponding place
        public void OnFall()
        {
            animator.SetBool("IsJumping", true);
        }

        public void OnLanding()
        {
            animator.SetBool("IsJumping", false);
        }

        public void FreezePlayerOnExit(float duration, float slowMultiplier)
        {
            if ( freezeRoutine == null )
            {
                freezeRoutine = StartCoroutine(FreezeRoutine(duration, slowMultiplier));
            }
            else if ( freezeRoutine != null )
            {
                TryStopCoroutine(ref freezeRoutine);
                freezeRoutine = StartCoroutine(FreezeRoutine(duration, slowMultiplier));
            }
        }

        public void FreezePlayerContinuously(float slowMultiplier)
        {
            currentWalkSpeed = slowMultiplier * walkSpeed;
            currentRunSpeed = slowMultiplier * runSpeed;

            TryUpdateMoveSpeed();
        }

        private IEnumerator FreezeRoutine(float duration, float slowMultiplier)
        {
            currentWalkSpeed = slowMultiplier * walkSpeed;
            currentRunSpeed = slowMultiplier * runSpeed;

            TryUpdateMoveSpeed();

            yield return new WaitForSeconds(duration);

            currentWalkSpeed = walkSpeed;
            currentRunSpeed = runSpeed;

            TryUpdateMoveSpeed();

            TryStopCoroutine(ref freezeRoutine);
            controller.SetIsFrozenFalse();
        }

        private void TryStopCoroutine(ref Coroutine routine)
        {
            if ( routine != null )
            {
                StopCoroutine(routine);
                routine = null;
            }
        }

        private void TryUpdateMoveSpeed()
        {
            switch ( isRunning )
            {
                case false:
                    if ( moveSpeed != currentWalkSpeed )
                    {
                        moveSpeed = currentWalkSpeed;
                    }

                    break;

                case true:
                    if ( moveSpeed != currentRunSpeed )
                    {
                        moveSpeed = currentRunSpeed;
                    }

                    break;
            }
        }
    }
}
