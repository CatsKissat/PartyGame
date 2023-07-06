using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

namespace BananaSoup
{
    public class PlayerActionMapSelector : MonoBehaviour
    {
        [SerializeField, Scene] private string mainMenuSceneName;
        [SerializeField, Scene] private string gameplayLevelSceneName;
        [SerializeField] private InputActionAsset inputActions;
        private PlayerInput playerInput;
        private const string mainMenuActionMapName = "MainMenu";
        private const string gameplayActionMapName = "Gameplay";

        private void Start()
        {
            GetReferences();

            string currentSceneName = SceneManager.GetActiveScene().name;

            if ( currentSceneName == mainMenuSceneName )
            {
                //Debug.Log($"Changing {name}'s Action Map to {mainMenuActionMapName}");
                playerInput.SwitchCurrentActionMap(mainMenuActionMapName);
            }
            else if ( currentSceneName == gameplayLevelSceneName )
            {
                //Debug.Log($"Changing {name}'s Action Map to {gameplayActionMapName}");
                playerInput.SwitchCurrentActionMap(gameplayActionMapName);
            }
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
    }
}
