using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using BananaSoup.Units;

namespace BananaSoup.Managers
{
    public class GameManager : MonoBehaviour
    {
        public event Action StartFinished;
        public event Action NewRound;
        public event Action<int> RoundEnded;
        private PlayerInputManager inputManager;
        private PlayerBase[] playerBases;
        private int playersAlive = 0;
        private bool isRoundOver = true;
        private Coroutine winnerCheckRoutine;
        private int playerAmount;
        private int winnerID;

        // Debug
        private bool isDebugSetupCalled;

        public PlayerBase[] Players
        {
            get { return playerBases; }
        }

        public int PlayersAlive => playersAlive;
        public int PlayerAmount => playerAmount;

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
            NewRound -= SetupNewRound;

            if ( playerBases != null )
            {
                foreach ( PlayerBase player in playerBases )
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
                NewRound += SetupNewRound;
                StartFinished();
                FireStartNewRoundEvent();
            }
        }

        // Debug
        public void DebugSetup()
        {
            if ( !isDebugSetupCalled )
            {
                isDebugSetupCalled = true;
                FindPlayersAndInitialize();
                NewRound += SetupNewRound;
                StartFinished();
                FireStartNewRoundEvent();
            }
            else
            {
                Debug.Log("Setup Joined Players already called. New calls are disabled so the game wont break up.");
            }
        }

        public void FireStartNewRoundEvent()
        {
            NewRound();
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

            playerAmount = playerGameObjects.Length;
            playerBases = new PlayerBase[playerAmount];

            for ( int i = 0; i < playerGameObjects.Length; i++ )
            {
                // Set the players to corresponding index in players using PlayerID
                playerGameObjects[i].TryGetComponent(out PlayerBase playerBase);
                for ( int j = 0; j < playerAmount; j++ )
                {
                    if ( playerBase.PlayerID == j )
                    {
                        playerBases[j] = playerBase;
                    }
                }

                InitializingActionMapSetup(playerGameObjects, i);
            }

            // Adding a listener for all players.
            foreach ( PlayerBase player in playerBases )
            {
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
            Debug.Log("Setuping a new round");

            // Set all players alive
            foreach ( PlayerBase player in playerBases )
            {
                player.IsDead = false;
            }

            playersAlive = playerBases.Length;

            isRoundOver = false;
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
                isRoundOver = true;
                RoundEnded(-1);
            }
            else if ( playersAlive == 1 )
            {
                isRoundOver = true;

                for ( int i = 0; i < playerBases.Length; i++ )
                {
                    if ( !playerBases[i].IsDead )
                    {
                        winnerID = playerBases[i].PlayerID;
                        RoundEnded(winnerID);
                    }
                }
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
