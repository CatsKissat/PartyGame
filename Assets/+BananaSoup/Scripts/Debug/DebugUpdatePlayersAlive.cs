using UnityEngine;
using TMPro;
using BananaSoup.Managers;

namespace BananaSoup.Debugging
{
    public class DebugUpdatePlayersAlive : MonoBehaviour
    {
        private TMP_Text text;
        private GameManager gameManager;
        private string uiText = "Players Alive: ";

        void Start()
        {
            text = GetComponent<TMP_Text>();
            if ( text == null )
            {
                Debug.LogError($"{name} is missing a reference to the TMP_Text!");
            }

            gameManager = FindObjectOfType<GameManager>();
            if ( gameManager == null )
            {
                Debug.LogError($"{name} is missing a reference to the GameManager!");
            }
        }
        
        // NOTE: Not the best practice to update UI text in the FixedUpdate, but in this project I don't care.
        private void FixedUpdate()
        {
            if ( text != null )
            {
                text.text = uiText + gameManager.PlayersAlive.ToString();
            }
        }
    }
}
