using BananaSoup.Units;
using UnityEngine;

namespace BananaSoup.LevelLogic
{
    public class OutOfZoneKiller : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            if ( other.TryGetComponent(out PlayerBase player))
            {
                // TODO: Add methdod to kill player and call it here.
                // This is just a placeholder kill method
                player.gameObject.SetActive(false);

                Debug.Log($"Player {player.PlayerID} went out of bounds.");
            }
        }
    }
}
