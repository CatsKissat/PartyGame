using UnityEngine;
using UnityEngine.InputSystem;
using BananaSoup.Units;

namespace BananaSoup
{
    public class Goal : MonoBehaviour
    {
        private int playerCount;
        private int finishedPlayers;
        private PlayerInputManager inputManager;

        public int PlayerFinished
        {
            set { finishedPlayers--; }
        }

        private void Start()
        {
            GetReferences();
        }

        private void OnTriggerEnter(Collider other)
        {
            if ( other.TryGetComponent(out PlayerBase player) )
            {
                player.PlayerFinished();
                finishedPlayers++;

                if ( finishedPlayers >= inputManager.playerCount )
                {
                    Debug.Log("-->> Round finished! <<--");

                    // TODO: Give results (scores)
                }
            }
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
                Debug.LogError($"{name} is missing a reference to the PlayerInputManager!");
            }
        }
    }
}
