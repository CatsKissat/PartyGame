using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;

namespace BananaSoup
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 40.0f;
        [SerializeField] private float runSpeed = 60.0f;
        private PlayerCharacterController controller;
        private Animator animator;
        private float moveSpeed = 0.0f;
        private float horizontalMove = 0.0f;
        private float moveInput;
        private bool jump = false;
        private PlayerInput playerInput;
        public UnityAction LeaveGame;

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
            controller.Frozen -= FreezePlayer;
            TryStopAndNullCoroutine(ref freezeRoutine);
        }

        private void Start()
        {
            GetReferences();
            moveSpeed = walkSpeed;

            controller.Frozen += FreezePlayer;
        }

        void Update()
        {
            horizontalMove = moveInput * moveSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }

        void FixedUpdate()
        {
            // Move our character
            controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
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
                moveSpeed = runSpeed;
            }
            else if ( context.canceled )
            {
                moveSpeed = walkSpeed;
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
                DisablePlayerGameObject();
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

        // TODO: Move animation calls to corresponding place
        public void OnFall()
        {
            animator.SetBool("IsJumping", true);
        }

        public void OnLanding()
        {
            animator.SetBool("IsJumping", false);
        }

        public void FreezePlayer(float duration, float slowMultiplier)
        {
            if ( freezeRoutine == null )
            {
                freezeRoutine = StartCoroutine(FreezeRoutine(duration, slowMultiplier, walkSpeed, runSpeed));
            }
        }

        private IEnumerator FreezeRoutine(float duration, float slowMultiplier, float walkSpeed, float runSpeed)
        {
            float previousWalkSpeed = walkSpeed;
            float previousRunSpeed = runSpeed;

            this.walkSpeed = slowMultiplier * walkSpeed;
            this.runSpeed = slowMultiplier * runSpeed;

            yield return new WaitForSeconds(duration);

            this.walkSpeed = previousWalkSpeed;
            this.runSpeed = previousRunSpeed;
        }

        private void TryStopAndNullCoroutine(ref Coroutine routine)
        {
            if ( routine != null )
            {
                StopCoroutine(routine);
                routine = null;
            }
        }
    }
}
