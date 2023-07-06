using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using BananaSoup.Units;
using NaughtyAttributes;

namespace BananaSoup.LevelLogic
{
    public class SelectLevel : MonoBehaviour
    {
        [SerializeField] private float countdownToStartLevel = 3.0f;
        [SerializeField, Scene] private string playableLevel;
        private const string playerManagerName = "PlayerManager";
        private int joinedPlayers;
        private int playersOnLevelSelector;
        private Coroutine startCountdownRoutine;
        private PlayerInputManager playerInputManager;

        private void Start()
        {
            GetReferences();
        }

        private void OnTriggerEnter(Collider other)
        {
            if ( other.TryGetComponent(out PlayerBase player) )
            {
                UpdateJoinedPlayer();

                playersOnLevelSelector++;

                if ( playersOnLevelSelector == joinedPlayers )
                {
                    startCountdownRoutine = StartCoroutine(Countdown());
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ( other.TryGetComponent(out PlayerBase player) )
            {
                playersOnLevelSelector--;

                UpdateJoinedPlayer();

                TryEndCoroutine(ref startCountdownRoutine);
            }
        }

        private void GetReferences()
        {
            GameObject playerManager = GameObject.FindGameObjectWithTag(playerManagerName);
            if ( playerManager == null )
            {
                Debug.LogError($"{name} is missing reference to a PlayerManager GameObject!");
            }

            playerInputManager = playerManager.GetComponent<PlayerInputManager>();
            if ( playerInputManager == null )
            {
                Debug.LogError($"{name} is missing reference to a PlayerInputManager!");
            }
        }

        private void UpdateJoinedPlayer()
        {
            joinedPlayers = playerInputManager.playerCount;
        }

        private IEnumerator Countdown()
        {
            Debug.Log($"Starting countdown: {countdownToStartLevel}");
            yield return new WaitForSeconds(countdownToStartLevel);
            Debug.Log($"Loading Scene: {playableLevel}");
            SceneManager.LoadScene(playableLevel, LoadSceneMode.Single);
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
