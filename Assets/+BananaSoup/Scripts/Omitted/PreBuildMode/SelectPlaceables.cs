using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using BananaSoup.Traps;
using BananaSoup.Blocks;
using BananaSoup.Platforms;
using BananaSoup.Units;

namespace BananaSoup.PreBuildMode
{
    public class SelectPlaceables : MonoBehaviour
    {
        [SerializeField, Tooltip("Trap prefabs")]
        private TrapBase[] trapPrefabs;

        [SerializeField, Tooltip("Block prefabs")]
        private BlockBase[] blockPrefabs;

        [SerializeField, Tooltip("Platform prefabs")]
        private PlatformBase[] platformPrefabs;

        [Space]

        [SerializeField, Tooltip("Toggle debugging on or off.")]
        private bool debugThis = false;

        [SerializeField, ShowIf(nameof(debugThis))]
        private int numberOfPlayers = 0;
        [SerializeField, ShowIf(nameof(debugThis))]
        private int extraPlaceables = 2;

        private List<UnitBase> selectedPlaceables = new List<UnitBase>();

        private int prefabsNeeded = 0;

        private void Start()
        {
            if ( !debugThis )
            {
                // TODO: Get the amount of players automatically to numberOfPlayers.
                //numberOfPlayers = GetSecretHiddenNumberOfPlayersFromSomewhereWhereItExists.
                //prefabsNeeded = numberOfPlayers + extraPlaceables;
            }
            else if ( debugThis )
            {
                prefabsNeeded = numberOfPlayers + extraPlaceables;
            }

            SelectPrefabs();
        }

        private void SelectPrefabs()
        {
            int platforms = Random.Range(1, 2);
            prefabsNeeded -= platforms;

            int traps = Random.Range(1, 3);
            prefabsNeeded -= traps;

            int blocks = prefabsNeeded;
            prefabsNeeded = 0;
            
            GetRandomBlock(blocks);
            GetRandomPlatform(platforms);
            GetRandomTrap(traps);
        }

        private UnitBase GetRandomPrefab(UnitBase[] prefabs)
        {
            int randomIndex = Random.Range(0, prefabs.Length);
            return prefabs[randomIndex];
        }

        private void GetRandomBlock(int amountToGet)
        {
            for ( int i = 0; i < amountToGet; i++ )
            {
                selectedPlaceables.Add(GetRandomPrefab(blockPrefabs)); 
            }
        }

        private void GetRandomPlatform(int amountToGet)
        {
            for ( int i = 0; i < amountToGet; i++ )
            {
                selectedPlaceables.Add(GetRandomPrefab(platformPrefabs)); 
            }
        }

        private void GetRandomTrap(int amountToGet)
        {
            for ( int i = 0; i < amountToGet; i++ )
            {
                selectedPlaceables.Add(GetRandomPrefab(trapPrefabs));
            }
        }
    }
}
