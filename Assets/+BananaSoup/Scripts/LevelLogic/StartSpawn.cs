using UnityEngine;
using BananaSoup.Managers;

namespace BananaSoup.LevelLogic
{
    public class StartSpawn : MonoBehaviour
    {
        private Transform[] spawnPoints;
        private GameManager gameManager;

        private void Awake()
        {
            GetReferences();

            Initialize();

            gameManager.NewRound += SetPlayersToSpawnPoints;
        }

        private void OnDisable()
        {
            gameManager.NewRound -= SetPlayersToSpawnPoints;
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

        /// <summary>
        /// Finds and add to a list all the Spawn Points in children.
        /// </summary>
        private void FindSpawnPoints()
        {
            SpawnPoint[] spawns = transform.GetComponentsInChildren<SpawnPoint>();
            spawnPoints = new Transform[spawns.Length];
            for ( int i = 0; i < spawns.Length; i++ )
            {
                spawnPoints[i] = spawns[i].transform;
            }
        }

        /// <summary>
        /// Goes through the list of players and sets a new position aka starting position for each of them.
        /// </summary>
        private void SetPlayersToSpawnPoints()
        {
            for ( int i = 0; i < gameManager.Players.Length; i++ )
            {
                gameManager.Players[i].SetPosition(spawnPoints[i]);
                Physics.SyncTransforms();
            }
        }
    }
}
