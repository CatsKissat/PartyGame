using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using BananaSoup.Units;

namespace BananaSoup.Managers
{
    public class GameManager : MonoBehaviour
    {
        public event Action StartNewRound;
        public event Action RoundEnded;
        private PlayerInputManager inputManager;
        private PlayerBase[] players;
        private int playersAlive = 0;
        private bool isRoundOver = true;
        private Coroutine winnerCheckRoutine;

        public PlayerBase[] Players
        {
            get { return players; }
        }

        public int PlayersAlive => playersAlive;

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
            Setup();
        }

        private void OnDisable()
        {
            StartNewRound -= SetupNewRound;

            if ( players != null )
            {
                foreach ( PlayerBase player in players )
                {
                    player.Killed -= DecreaseAlivePlayers;
                }
            }
        }

        private void Setup()
        {
            GetReferences();

            if ( !enableJoining )
            {
                DisableJoining();
            }

            if ( !skipAutoChangeActionMap )
            {
                FindPlayersAndInitialize();
                StartNewRound += SetupNewRound;
                FireStartNewRoundEvent();
            }
        }

        public void DebugSetup()
        {
            FindPlayersAndInitialize();
            StartNewRound += SetupNewRound;
            FireStartNewRoundEvent();
        }

        public void FireStartNewRoundEvent()
        {
            StartNewRound();
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
        /// Disable player joining in the game for example, in the gameplay Level.
        /// </summary>
        private void DisableJoining()
        {
            inputManager.DisableJoining();
        }

        /// <summary>
        /// Find all player in the Scene and call PlayerActionMapSelector to change ActionMaps.
        /// </summary>
        private void FindPlayersAndInitialize()
        {
            // Find Player GameObjects
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
                    if ( playerBase.PlayerID == j )
                    {
                        players[j] = playerBase;
                    }
                }

                InitializingActionMapSetup(playerGameObjects, i);
            }

            // Adding a listener for all players.
            foreach ( PlayerBase player in players )
            {
                Debug.Log($"Player {player.PlayerID} added to listener");
                player.Killed += DecreaseAlivePlayers;
            }
        }

        /// <summary>
        /// Get reference to PlayerActionMapSelector and call its Setup().
        /// </summary>
        /// <param name="playerGameObjects">Array of the player GameObjects.</param>
        /// <param name="i">Index in playerGameObjects.</param>
        private void InitializingActionMapSetup(GameObject[] playerGameObjects, int i)
        {
            PlayerActionMapSelector actionMapSelector = playerGameObjects[i].GetComponent<PlayerActionMapSelector>();
            if ( actionMapSelector == null )
            {
                Debug.LogError($"{name} is missing a reference to PlayerActionMapSelector on Player index [{i}]!");
            }

            actionMapSelector.Setup();
        }

        /// <summary>
        /// Sets a new round.
        /// </summary>
        private void SetupNewRound()
        {
            Debug.Log("Setting a new round");
            isRoundOver = false;
            playersAlive = players.Length;
        }

        /// <summary>
        /// Decreases int variable how many players there are still alive.
        /// </summary>
        private void DecreaseAlivePlayers()
        {
            playersAlive--;

            if ( !isRoundOver )
            {
                TryEndCoroutine(ref winnerCheckRoutine);
                winnerCheckRoutine = StartCoroutine(CheckForWinner());
            }
        }

        private IEnumerator CheckForWinner()
        {
            yield return new WaitForSeconds(0.1f);

            if ( playersAlive <= 0 )
            {
                Debug.Log("Round over. Draw.");
                isRoundOver = true;
            }
            else if ( playersAlive == 1 )
            {
                Debug.Log("Round over. Only one player left.");
                isRoundOver = true;
            }

            if ( isRoundOver )
            {
                RoundEnded();
            }
        }

        private void TryEndCoroutine(ref Coroutine routine)
        {
            if ( routine != null )
            {
                StopCoroutine(routine);
                routine = null;
            }
        }
    }
}
