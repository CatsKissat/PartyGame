using BananaSoup.Units;
using BananaSoup.Weapons;
using UnityEngine;

namespace BananaSoup.LevelLogic
{
    public class OutOfZoneKiller : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            if ( other.TryGetComponent(out PlayerBase player) )
            {
                player.Kill();

                Debug.Log($"Player {player.PlayerID} went out of bounds and is now dead.");
            }

            if ( other.TryGetComponent(out WeaponBase weapon) )
            {
                Destroy(weapon.gameObject);

                Debug.Log($"Weapon went out of bounds and is now destroyed.");
            }
        }
    }
}
