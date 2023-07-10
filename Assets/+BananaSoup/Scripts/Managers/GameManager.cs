using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaSoup
{
    public class GameManager : MonoBehaviour
    {
        private PlayerInputManager inputManager;

        void Start()
        {
            GetReferences();
            SetActionMapForPlayers();
            DisableJoining();
        }

        private void GetReferences()
        {
            GameObject playerManager = GameObject.FindGameObjectWithTag("PlayerManager");
            if ( playerManager == null )
            {
                Debug.LogError($"{name} is missing a reference to the PlayerManager GameObject!");
            }

            inputManager = playerManager.GetComponent<PlayerInputManager>();
            if ( inputManager == null )
            {
                Debug.LogError($"{name} is missing a reference to PlayerInputManager!");
            }
        }

        private void SetActionMapForPlayers()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if ( players.Length <= 0 )
            {
                Debug.LogError($"{name} is missing reference(s) to the Player(s) GameObject(s)!");
            }

            for ( int i = 0; i < players.Length; i++ )
            {
                PlayerActionMapSelector actionMapSelector = players[i].GetComponent<PlayerActionMapSelector>();
                if ( actionMapSelector == null )
                {
                    Debug.LogError($"{name} is missing a reference to PlayerActionMapSelector on Player index [{i}]!");
                }

                actionMapSelector.Setup();
            }
        }

        private void DisableJoining()
        {
            inputManager.DisableJoining();
        }
    }
}
