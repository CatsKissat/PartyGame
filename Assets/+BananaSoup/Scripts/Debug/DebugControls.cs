using UnityEngine;
using BananaSoup.Managers;
using NaughtyAttributes;

namespace BananaSoup.Debugging
{
    public class DebugControls : MonoBehaviour
    {
        [InfoBox("Changing values during runtime doesn't effect anywhere. Values are used on Awake only!", EInfoBoxType.Warning)]

        [Space]

        [SerializeField]
        private bool areDebugControlsEnabled;

        #region GameManager
        [SerializeField]
        [ShowIf(nameof(areDebugControlsEnabled))]
        [Header("GameManager")]
        private GameManager gameManager;

        [SerializeField]
        [ShowIf(nameof(HasGameManager))]
        [Tooltip("If true PlayerCharacters' Action Map doesn't change to the correct one when loading a scene with the GameManager.")]
        private bool skipAutoChangeActionMap = false;

        [SerializeField]
        [ShowIf(nameof(HasGameManager))]
        [Tooltip("If true the players can join the game e.g. in gameplay level.")]
        private bool enableJoining = false;
        #endregion GameManager

        #region DebugUI
        [SerializeField]
        [ShowIf(nameof(areDebugControlsEnabled))]
        [Header("Debug UI")]
        private GameObject debugUI;

        [SerializeField]
        [ShowIf(nameof(HasDebugUI))]
        [Tooltip("If true enable Debug UI")]
        private bool isDebugUIEnabled = false;
        #endregion DebugUI

        private bool HasGameManager => gameManager && areDebugControlsEnabled;
        private bool HasDebugUI => debugUI && areDebugControlsEnabled;

        private void Awake()
        {
            if ( !areDebugControlsEnabled )
            {
                return;
            }

            Debug.LogWarning("Debug Controls are enabled");

            if ( gameManager != null )
            {
                if ( skipAutoChangeActionMap )
                {
                    gameManager.SkipAutoChangeActionMap = true;
                    Debug.Log($"{this} set {nameof(gameManager)}'s SkipAutoChangeActionMap to {gameManager.SkipAutoChangeActionMap}");
                }

                if ( enableJoining )
                {
                    gameManager.EnableJoining = true;
                    Debug.Log($"{this} set {nameof(gameManager)}'s EnableJoining to {gameManager.EnableJoining}");
                }
            }

            if ( debugUI != null )
            {
                if ( isDebugUIEnabled )
                {
                    debugUI.SetActive(true);
                }
            }
        }
    }
}
