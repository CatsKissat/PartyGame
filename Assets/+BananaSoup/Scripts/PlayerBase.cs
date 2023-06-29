using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaSoup.Units
{
    public class PlayerBase : UnitBase
    {
        private PlayerInput playerInput;
        private int playerID;
        private PlayerSpriteSelector playerSpriteSelector;

        [HideInInspector]
        public bool isStunned = false;
        [HideInInspector]
        public bool isFrozen = false;
        [HideInInspector]
        public bool isDead = false;

        public int PlayerID => playerID;

        private void OnEnable()
        {
            GetInputReference();

            Debug.Log($"PlayerID {playerID} joined the game.");
        }

        private void OnDisable()
        {
            Debug.Log($"PlayerID {playerID} left the game.");
        }

        protected override void Start()
        {
            base.Start();

            GetReferences();
            playerSpriteSelector.AssingSprite();
        }

        private void GetInputReference()
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
