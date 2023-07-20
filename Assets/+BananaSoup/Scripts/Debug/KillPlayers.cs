using UnityEngine;
using BananaSoup.Units;

namespace BananaSoup.Debugging
{
    public class KillPlayers : MonoBehaviour
    {
        public void KillPlayerByID(int playerID)
        {
            PlayerBase[] players = FindObjectsOfType<PlayerBase>();

            if ( (players.Length - 1) >= playerID )
            {
                players[playerID].Kill();
            }
        }

        public void KillAllPlayers()
        {
            PlayerBase[] players = FindObjectsOfType<PlayerBase>();

            foreach ( PlayerBase player in players )
            {
                player.Kill();
            }
        }
    }
}
