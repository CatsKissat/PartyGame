using UnityEngine;
using System.Collections;
using TMPro;
using BananaSoup.Managers;

namespace BananaSoup.ScoreSystem
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerScorePanelPrefab;
        [SerializeField] private GameObject drawPanel;
        [SerializeField] private GameObject winnerPanel;
        [SerializeField] private GameObject continuePanel;
        private PlayerScorePanel[] playerScorePanel;
        private GameManager gameManager;
        private ScorePanel scorePanel;
        private PlayersPanel playersPanel;
        private int previousWinner;
        private bool isScorescreenActive;
        private bool allowContinue;
        private float continueTimer = 2.0f;
        private Coroutine continueRoutine;

        public bool IsScorescreenActive => isScorescreenActive;
        public bool AllowContinue
        {
            get
            {
                return allowContinue;
            }
            set
            {
                allowContinue = value;
            }
        }

        private void Awake()
        {
            GetReferences();

            // Making sure that the panels aren't active when starting the game.
            scorePanel.gameObject.SetActive(false);
            drawPanel.SetActive(false);
            winnerPanel.gameObject.SetActive(false);

            gameManager.StartFinished += Setup;
            gameManager.RoundEnded += UpdateScore;
            gameManager.NewRound += HideScores;
            gameManager.WinnerFound += ShowEndResults;
        }

        private void OnDisable()
        {
            gameManager.StartFinished -= Setup;
            gameManager.RoundEnded -= UpdateScore;
            gameManager.NewRound -= HideScores;
            gameManager.WinnerFound -= ShowEndResults;

            TryEndRoutine();
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
            continuePanel.SetActive(false);

            scorePanel.gameObject.SetActive(true);

            isScorescreenActive = true;

            if ( winnerID >= 0 )
            {
                playerScorePanel[winnerID].UpdateScore();
                previousWinner = winnerID;
            }
            else
            {
                drawPanel.SetActive(true);
            }

            TryEndRoutine();
            continueRoutine = StartCoroutine(WaitBeforeContinue());
        }

        private void HideScores()
        {
            drawPanel.SetActive(false);
            scorePanel.gameObject.SetActive(false);
            playerScorePanel[previousWinner].HideWinnerText();
            isScorescreenActive = false;
        }

        private void ShowEndResults(int winnerID)
        {
            TMP_Text text = winnerPanel.GetComponent<TMP_Text>();
            string winnerText = $"Player {winnerID} is the Winner!";
            text.text = winnerText;

            winnerPanel.gameObject.SetActive(true);
        }

        private IEnumerator WaitBeforeContinue()
        {
            yield return new WaitForSeconds(continueTimer);
            allowContinue = true;
            continuePanel.SetActive(true);
        }

        private void TryEndRoutine()
        {
            if ( continueRoutine != null )
            {
                StopCoroutine(continueRoutine);
                continueRoutine = null;
            }
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
        // ScoreManager closes ScorePanel and gives calls an event to start new round when player pushes a button
        // TODO: ScoreManager calls an event to end the game if match winner is found.
        // TODO: Set the players in score order from highest to lowest in ScorePanel
        // Set all player back alive when new round
        // Set players' controls to ScoreScreen when in Scoreboard
        // Set players' controls to Gameplay when leaving Scoreboard
        // TODO: ScoreManager gets scores from PlayerBase
        // TODO: Wait x time before accepting Continue
    }
}
