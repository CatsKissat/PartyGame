using UnityEngine;
using TMPro;

namespace BananaSoup.ScoreSystem
{
    public class PlayerScorePanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private GameObject winner;
        private string playerString = "Player ";
        private int playerID;
        private int currentScore;

        public int PlayerID
        {
            get
            {
                return playerID;
            }
            set
            {
                playerID = value;
            }
        }

        public void Setup()
        {
            playerText.text = playerString + playerID;
            scoreText.text = currentScore.ToString();
            winner.gameObject.SetActive(false);
        }

        public void UpdateScore()
        {
            currentScore++;
            scoreText.text = currentScore.ToString();

            winner.SetActive(true);
        }

        public void HideWinnerText()
        {
            winner.SetActive(false);
        }
    }
}
