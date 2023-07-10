using UnityEngine;
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
        [ShowIf("HasGameManager")]
        [Tooltip("If enabled PlayerCharacters' Action Map doesn't change to the correct one when loading a scene with the GameManager.")]
        private bool skipAutoChangeActionMap = false;

        [SerializeField]
        [ShowIf("HasGameManager")]
        [Tooltip("If enabled the players can join the game e.g. in gameplay level.")]
        private bool enableJoining = false;
        #endregion GameManager

        public bool HasGameManager => gameManager && areDebugControlsEnabled;

        private void Awake()
        {
            if ( !areDebugControlsEnabled )
            {
                return;
            }

            if ( gameManager != null )
            {
                if ( skipAutoChangeActionMap )
                {
                    Debug.Log("Enabling SkipAutoChangeActionMap");
                    gameManager.SkipAutoChangeActionMap = true;
                }

                if ( enableJoining )
                {
                    Debug.Log("Enabling EnableJoining");
                    gameManager.EnableJoining = true;
                }
            }

        }
    }
}
