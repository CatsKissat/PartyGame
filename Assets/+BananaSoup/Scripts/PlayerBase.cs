using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaSoup.Units
{
    public class PlayerBase : UnitBase
    {
        private PlayerInput playerInput;
        private int playerId;
        private PlayerSpriteSelector playerSpriteSelector;

        private void OnEnable()
        {
            GetInputReference();

            Debug.Log($"PlayerID {playerId} joined the game.");
        }

        private void OnDisable()
        {
            Debug.Log($"PlayerID {playerId} left the game.");
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
                playerId = playerInput.playerIndex;

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
