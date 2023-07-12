using BananaSoup.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BananaSoup.LevelLogic
{
    public class StartSpawn : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        private GameManager gameManager;

        private void Awake()
        {
            GetReferences();

            Initialize();

            gameManager.SetupNewRound += SetPlayersToSpawnPoints;
        }

        private void OnDisable()
        {
            gameManager.SetupNewRound -= SetPlayersToSpawnPoints;
        }

        private void GetReferences()
        {
            // GameManager GameObject
            GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
            if ( gameManagerObject == null )
            {
                Debug.LogError($"{name} is missing a reference to the GameManager GameObject!");
            }

            // GameManager component
            gameManager = gameManagerObject.GetComponent<GameManager>();
            if ( gameManager == null )
            {
                Debug.LogError($"{name} is missing a reference to a GameManager!");
            }
        }

        public void Initialize()
        {
            FindSpawnPoints();
        }

        private void FindSpawnPoints()
        {
            SpawnPoint[] spawns = transform.GetComponentsInChildren<SpawnPoint>();
            spawnPoints = new Transform[spawns.Length];
            for ( int i = 0; i < spawns.Length; i++ )
            {
                spawnPoints[i] = spawns[i].transform;
            }
        }

        private void SetPlayersToSpawnPoints()
        {
            for ( int i = 0; i < gameManager.Players.Length; i++ )
            {
                Debug.Log("gameManager.Players[i - 1]: " + gameManager.Players[i]);
                Debug.Log("spawnPoints[i].position: " + spawnPoints[i].position);
                Debug.Log("Current position: " + gameManager.Players[i].SetPosition);
                gameManager.Players[i].SetPosition = spawnPoints[i].position;
                Debug.Log("New position: " + gameManager.Players[i].SetPosition);
            }
        }
    }
}
