using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaSoup
{
    public class PlayerSpriteSelector : MonoBehaviour
    {
        private PlayerInput playerInput;
        private int playerId;

        private void OnEnable()
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

            //playerInput.deviceLostEvent.AddListener()

            Debug.Log($"PlayerID {playerId} joined the game.");

            AssingSprite();
        }

        private void OnDisable()
        {
            Debug.Log($"PlayerID {playerId} left the game.");
        }

        private void AssingSprite()
        {
            // TODO: Assing correct sprite to the player.
        }
    }
}
