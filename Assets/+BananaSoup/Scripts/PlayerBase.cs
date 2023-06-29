using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaSoup.Units
{
    public class PlayerBase : UnitBase
    {
        [HideInInspector]
        public bool isStunned = false;
        [HideInInspector]
        public bool isFrozen = false;
        [HideInInspector]
        public bool isDead = false;

        private PlayerInput playerInput;
        private int playerID;
        private PlayerSpriteSelector playerSpriteSelector;
        private PlayerMovement playerMovement;
        private CameraTargetAssigner cameraTargetAssigner;

        public int PlayerID => playerID;

        private void OnEnable()
        {
            TryGetReferences();

            playerMovement.LeaveGame += OnLeave;

            GameObject cameraTargetGroup = GameObject.FindGameObjectWithTag("CameraTargetGroup");
            cameraTargetAssigner = cameraTargetGroup.GetComponent<CameraTargetAssigner>();
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
                playerID = playerInput.playerIndex;

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
    }
}
