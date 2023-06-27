using UnityEngine;
using UnityEngine.InputSystem;

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
        private bool dash = false;
        private PlayerInput playerInput;

        private void OnEnable()
        {
            playerInput = GetComponent<PlayerInput>();
            if ( playerInput == null )
            {
                Debug.LogError($"{name}'s PlayerInput is null!");
            }

            Debug.Log($"{name} is subscribing to a OnDeviceLost event.");
            playerInput.onDeviceLost += OnDeviceLost;
        }

        private void OnDisable()
        {
            Debug.Log($"{name} is desubscribing to a OnDeviceLost event.");
            playerInput.onDeviceLost -= OnDeviceLost;
        }

        private void Start()
        {
            GetReferences();
            moveSpeed = walkSpeed;
        }

        void Update()
        {
            horizontalMove = moveInput * moveSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }

        void FixedUpdate()
        {
            // Move our character
            controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash);
            jump = false;

            // TODO: Remove dash and its references
            dash = false;
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
                DisablePlayerGameObject();
            }
        }
        #endregion OnInputs

        //public void OnDeviceLost()
        //{
        //    //DisablePlayerGameObject();
        //    Debug.Log("Input device disconnected");
        //}

        private void OnDeviceLost(PlayerInput input)
        {
            Debug.Log("Input device lost for player: " + input.playerIndex);
            DisablePlayerGameObject();
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
    }
}
