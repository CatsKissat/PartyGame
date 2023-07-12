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

        //TODO: Where and when is this called?
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

                Debug.Log($"Player with ID: {playerID} entered the trigger of {name}!");

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

                if ( currentModifier == freezeMod )
                {
                    FreezeOnExit(player);
                }
            }
            else
            {
                return;
            }
        }

        private void FixedUpdate()
        {
            if ( amountOfPlayersInTrigger > 0 )
            {
                DetermineModAction();
            }
        }

        private void DetermineModAction()
        {
            switch ( currentModifier )
            {
                case basicMod:
                case speedMod:
                case sizeMod:
                    {
                        PushbackAction();

                        break;
                    }
                case freezeMod:
                    {
                        FreezeAction();
                        PushbackAction();

                        break;
                    }
                case electricMod:
                    {
                        StunAction();
                        PushbackAction();

                        break;
                    }
                default:
                    {
                        Debug.LogError($"{name} has no active modifier! This is a bug!");
                        break;
                    }
            }
        }

        private void PushbackAction()
        {
            foreach ( PlayerBase player in playersInTriggerzone )
            {
                player.Pushback(transform.forward, pushbackStrength);
            }
        }

        private void FreezeAction()
        {
            foreach ( PlayerBase player in playersInTriggerzone )
            {
                player.FreezeContinously(slowMultiplier);
            }
        }

        private void FreezeOnExit(PlayerBase player)
        {
            player.Freeze(slowDuration, slowMultiplier);
        }

        private void StunAction()
        {
            foreach ( PlayerBase player in playersInTriggerzone )
            {
                player.Stun(stunDuration);
            }
        }

        public void IncreasePushback()
        {
            pushbackStrength += speed;
        }
    }
}
