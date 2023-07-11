using BananaSoup.Units;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaSoup.Managers
{
    public class GameManager : MonoBehaviour
    {
        public Action SetupNewRound;
        private PlayerInputManager inputManager;
        private PlayerBase[] players;

        public PlayerBase[] Players
        {
            get { return players; }
        }

        #region Debug
        private bool skipAutoChangeActionMap;
        private bool enableJoining;

        public bool SkipAutoChangeActionMap
        {
            get { return skipAutoChangeActionMap; }
            set { skipAutoChangeActionMap = value; }
        }

        public bool EnableJoining
        {
            get { return enableJoining; }
            set { enableJoining = value; }
        }
        #endregion Debug

        void Start()
        {
            GetReferences();
            FindPlayersAndInitializeActionMapSelector();
            DisableJoining();
            SetupNewRound.Invoke();
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

        /// <summary>
        /// Find all player in the Scene and call PlayerActionMapSelector to change ActionMaps.
        /// </summary>
        private void FindPlayersAndInitializeActionMapSelector()
        {
            // Debug check
            if ( skipAutoChangeActionMap )
            {
                return;
            }

            GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
            if ( playerGameObjects.Length <= 0 )
            {
                Debug.LogError($"{name} is missing reference(s) to the Player(s) GameObject(s)!");
            }

            players = new PlayerBase[playerGameObjects.Length];

            for ( int i = 0; i < playerGameObjects.Length; i++ )
            {
                // Set the players to corresponding index in players using PlayerID
                playerGameObjects[i].TryGetComponent(out PlayerBase playerBase);
                for ( int j = 0; j < playerGameObjects.Length; j++ )
                {
                    if (playerBase.PlayerID == j )
                    {
                        players[j] = playerBase;
                    }
                }

                // Get reference to PlayerActionMapSelector and call it's Setup()
                PlayerActionMapSelector actionMapSelector = playerGameObjects[i].GetComponent<PlayerActionMapSelector>();
                if ( actionMapSelector == null )
                {
                    Debug.LogError($"{name} is missing a reference to PlayerActionMapSelector on Player index [{i}]!");
                }

                actionMapSelector.Setup();
            }
        }

        /// <summary>
        /// Disable player joining in the game.
        /// </summary>
        private void DisableJoining()
        {
            // Debug check
            if ( enableJoining )
            {
                return;
            }

            inputManager.DisableJoining();
        }
    }
}
