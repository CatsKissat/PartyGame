using UnityEngine;
using UnityEngine.InputSystem;
using BananaSoup.Units;

namespace BananaSoup.Traps
{
    public class FanModifierActions : ModifierActions
    {
        [Space]

        [SerializeField]
        private float pushbackStrength = 1.5f;

        private int amountOfPlayersInTrigger = 0;

        private PlayerInputManager playerManager = null;
        private PlayerBase[] players = null;
        private PlayerBase[] playersInTriggerzone = null;

        private const string playerManagerTag = "PlayerManager";
        private const string playerTag = "Player";

        private void Start()
        {
            playerManager = GameObject.FindGameObjectWithTag(playerManagerTag).GetComponent<PlayerInputManager>();

            if ( playerManager == null )
            {
                Debug.Log($"{name} couldn't find a GameObject with the tag {playerManagerTag} with the component {typeof(PlayerInputManager)}!");
            }
        }

        public void GetPlayers()
        {
            if ( players == null )
            {
                players = new PlayerBase[playerManager.playerCount];

                GameObject[] tempPlayers = new GameObject[playerManager.playerCount];
                tempPlayers = GameObject.FindGameObjectsWithTag(playerTag);

                foreach ( GameObject player in tempPlayers )
                {
                    int playerIndex = player.GetComponent<PlayerBase>().PlayerID;

                    if ( players[playerIndex] == null )
                    {
                        players[playerIndex] = player.GetComponent<PlayerBase>();
                    }
                }
            }

            playersInTriggerzone = new PlayerBase[playerManager.playerCount];
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if ( (playersLayerMask.value & (1 << other.transform.gameObject.layer)) > 0
                && other.TryGetComponent(out PlayerBase player) )
            {
                int playerID = player.PlayerID;

                playersInTriggerzone[playerID] = players[playerID];
                //while ( playersInTriggerzone[playerID] != null )
                //{
                //    DetermineModAction(other);
                //}
                amountOfPlayersInTrigger++;
            }
            else
            {
                return;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ( (playersLayerMask.value & (1 << other.transform.gameObject.layer)) > 0
                && other.TryGetComponent(out PlayerBase player))
            {
                int playerID = player.PlayerID;

                playersInTriggerzone[playerID] = null;
                amountOfPlayersInTrigger--;
            }
            else
            {
                return;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if ( amountOfPlayersInTrigger > 0 )
            {
                DetermineModAction(other);
            }
        }

        protected override void DetermineModAction(Collider other)
        {
            switch ( currentModifier )
            {
                case basicMod:
                case speedMod:
                case sizeMod:
                    {
                        DefaultAction();

                        break;
                    }
                case freezeMod:
                    {
                        if ( other.gameObject.GetComponent<PlayerBase>() )
                        {
                            other.GetComponent<PlayerBase>().Freeze(slowDuration, slowMultiplier);
                        }

                        break;
                    }
                case electricMod:
                    {
                        if ( other.gameObject.GetComponent<PlayerBase>() )
                        {
                            other.GetComponent<PlayerBase>().Stun(stunDuration);
                        }

                        break;
                    }
                default:
                    {
                        Debug.LogError($"{name} has no active modifier! This is a bug!");
                        break;
                    }
            }
        }

        private void DefaultAction()
        {
            foreach ( PlayerBase player in playersInTriggerzone )
            {
                player.Pushback(transform.forward, pushbackStrength);
            }
        }
    }
}
