using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

namespace BananaSoup
{
    public class PlayerActionMapSelector : MonoBehaviour
    {
        [SerializeField, Scene] private string mainMenuSceneName;
        [SerializeField, Scene] private string levelSceneName;
        [SerializeField] private InputActionAsset inputActions;
        private PlayerInput playerInput;
        private const string mainMenuActionMapName = "MainMenu";
        private const string gameplayActionMapName = "Gameplay";
        private const string scoreboardActionMapName = "Scoreboard";

        public void Setup()
        {
            GetReferences();
            SetActionMapOnSceneLoad();
        }

        private void GetReferences()
        {
            playerInput = GetComponent<PlayerInput>();
            if ( playerInput == null )
            {
                Debug.LogError($"{name} is missing reference to a {typeof(PlayerInput)}");
            }

            if ( inputActions == null )
            {
                Debug.LogError($"{name} is missing reference to a {typeof(InputActionAsset)}!");
            }
        }

        private void SetActionMapOnSceneLoad()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if ( currentSceneName == mainMenuSceneName )
            {
                playerInput.SwitchCurrentActionMap(mainMenuActionMapName);
            }
            else if ( currentSceneName == levelSceneName )
            {
                SetActionMapOnNewRound();
            }
        }

        public void SetActionMapOnDeath()
        {
            playerInput.SwitchCurrentActionMap(scoreboardActionMapName);
        }

        public void SetActionMapOnNewRound()
        {
            playerInput.SwitchCurrentActionMap(gameplayActionMapName);
        }
    }
}
