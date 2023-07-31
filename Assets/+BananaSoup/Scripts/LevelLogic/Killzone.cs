using UnityEngine;
using BananaSoup.Units;
using BananaSoup.Weapons;

namespace BananaSoup.LevelLogic
{
    public class Killzone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
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