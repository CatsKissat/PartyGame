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
            //Debug.Log("Initialize()");
            FindSpawnPoints();
        }

        /// <summary>
        /// Finds and add to a list all the Spawn Points in children.
        /// </summary>
        private void FindSpawnPoints()
        {
            //Debug.Log("FindSpawnPoints()");
            SpawnPoint[] spawns = transform.GetComponentsInChildren<SpawnPoint>();
            spawnPoints = new Transform[spawns.Length];
            for ( int i = 0; i < spawns.Length; i++ )
            {
                spawnPoints[i] = spawns[i].transform;
                //Debug.Log($"SpawnPoint[{i}] added");
            }
        }

        /// <summary>
        /// Goes through the list of players and sets a new position aka starting position for each of them.
        /// </summary>
        private void SetPlayersToSpawnPoints()
        {
            //Debug.Log("SetPlayersToSpawnPoints()");
            for ( int i = 0; i < gameManager.Players.Length; i++ )
            {
                //Debug.Log("gameManager.Players[i]: " + gameManager.Players[i].name);
                //Debug.Log($"spawnPoints[i] -> {spawnPoints[i].name}'s {spawnPoints[i].position}");
                //Debug.Log($"Player {gameManager.Players[i].name}'s old position: " + gameManager.Players[i].Position);
                gameManager.Players[i].SetPosition(spawnPoints[i]);
                //Debug.Log($"Player {gameManager.Players[i].name}'s new position: " + gameManager.Players[i].Position);
            }
        }
    }
}
