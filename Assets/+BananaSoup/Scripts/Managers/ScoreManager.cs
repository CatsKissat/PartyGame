using UnityEngine;
using BananaSoup.Managers;

namespace BananaSoup.ScoreSystem
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerScorePanelPrefab;
        [SerializeField] private GameObject drawPanel;
        private PlayerScorePanel[] playerScorePanel;
        private GameManager gameManager;
        private ScorePanel scorePanel;
        private PlayersPanel playersPanel;
        private int previousWinner;

        private void Awake()
        {
            GetReferences();

            // Making sure that the panels aren't active when starting the game.
            scorePanel.gameObject.SetActive(false);
            drawPanel.SetActive(false);

            gameManager.StartFinished += Setup;
            gameManager.RoundEnded += UpdateScore;
            gameManager.NewRound += HideScores;
        }

        private void OnDisable()
        {
            gameManager.StartFinished -= Setup;
            gameManager.RoundEnded -= UpdateScore;
            gameManager.NewRound -= HideScores;
        }

        private void GetReferences()
        {
            scorePanel = GetComponentInChildren<ScorePanel>();
            if ( scorePanel == null )
            {
                Debug.LogError($"{name} is missing a reference to the ScorePanel!");
            }

            playersPanel = GetComponentInChildren<PlayersPanel>();
            if ( playersPanel == null )
            {
                Debug.LogError($"{name} is missing a reference to the PlayersPanel!");
            }

            gameManager = FindObjectOfType<GameManager>();
            if ( gameManager == null )
            {
                Debug.LogError($"{name} is missing a reference to the GameManager!");
            }
        }

        private void Setup()
        {
            playerScorePanel = new PlayerScorePanel[gameManager.PlayerAmount];

            // Instantiating a Score Panel for all of the players.
            for ( int i = 0; i < gameManager.PlayerAmount; i++ )
            {
                GameObject player = Instantiate(playerScorePanelPrefab, playersPanel.transform);
                playerScorePanel[i] = player.GetComponent<PlayerScorePanel>();
                playerScorePanel[i].PlayerID = gameManager.Players[i].PlayerID;
                playerScorePanel[i].Setup();
            }
        }

        private void UpdateScore(int winnerID)
        {
            scorePanel.gameObject.SetActive(true);

            if ( winnerID >= 0 )
            {
                playerScorePanel[winnerID].UpdateScore();
                previousWinner = winnerID;
            }
            else
            {
                drawPanel.SetActive(true);
            }
        }

        private void HideScores()
        {
            drawPanel.SetActive(false);
            scorePanel.gameObject.SetActive(false);
            playerScorePanel[previousWinner].HideWinnerText();
        }

        // NOTE: My check list
        // ScoreManager has reference to the GameManager
        // ScoreManager has (reference?) to playerIDs
        // ScoreManager instantiates correct amount of Player Panels to UI
        // ScoreManager has a reference which PlayerPanel belongs to whom
        // GameManager gives information who won the round
        // GameManager gives information if it's a draw
        // ScoreManager updates player x's score, when there is a winner
        // ScoreManager updates UI if it's a draw
        // TODO: ScoreManager closes ScorePanel and gives calls an event to start new round when player pushes a button
        // TODO: ScoreManager calls an event to end the game if match winner is found.
        // TODO: Set the players in score order from highest to lowest in ScorePanel
        // Set all player back alive when new round
        // TODO: Set players' controls to ScoreScreen when in Scoreboard
        // TODO: Set players' controls to Gameplay when leaving Scoreboard
    }
}