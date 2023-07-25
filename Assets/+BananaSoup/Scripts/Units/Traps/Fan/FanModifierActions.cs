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

        /// <summary>
        /// Used to get a reference to the PlayerInputManager/PlayerManager.
        /// </summary>
        private void Start()
        {
            playerManager = GameObject.FindGameObjectWithTag(playerManagerTag).GetComponent<PlayerInputManager>();

            if ( playerManager == null )
            {
                Debug.Log($"{name} couldn't find a GameObject with the tag {playerManagerTag} with the component {typeof(PlayerInputManager)}!");
            }
        }

        //TODO: Where and when is this called?
        /// <summary>
        /// Method used to get an array of existing players and order them by PlayerID's.
        /// Then set the length of the playersInTriggerzone array to the amount of players.
        /// </summary>
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

        /// <summary>
        /// When a player enters the trigger collider of the fan add them to the
        /// the corresponding slot in the playersInTriggerzone array and
        /// increase amountOfPlayersInTrigger by one.
        /// If the entering object is not on the player LayerMask and doesn't have a
        /// PlayerBase return.
        /// </summary>
        /// <param name="other">The other GameObjects collider.</param>
        protected override void OnTriggerEnter(Collider other)
        {
            if ( (playersLayerMask.value & (1 << other.transform.gameObject.layer)) > 0
                && other.TryGetComponent(out PlayerBase player) )
            {
                int playerID = player.PlayerID;

                playersInTriggerzone[playerID] = players[playerID];

                amountOfPlayersInTrigger++;
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Check if the exiting object is on the player LayerMask and has a PlayerBase.
        /// If not return, otherwise
        /// remove the player from the playersInTriggerzone array with the players
        /// PlayerID.
        /// Check if the current modifier of the fan is freezeMod, if it is freeze
        /// the player on exit.
        /// Otherwise return.
        /// </summary>
        /// <param name="other"></param>
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

        /// <summary>
        /// Used to determine the correct action towards players on a constant loop.
        /// </summary>
        private void FixedUpdate()
        {
            if ( amountOfPlayersInTrigger > 0 )
            {
                DetermineModAction();
            }
        }

        /// <summary>
        /// Used to determine the correct action based on the current modifier of the trap.
        /// </summary>
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

        /// <summary>
        /// The default pushback action of the trap.
        /// Simply pushback the player towards the traps transform.forward with the
        /// set pushbackStrength.
        /// </summary>
        private void PushbackAction()
        {
            foreach ( PlayerBase player in playersInTriggerzone )
            {
                player.Pushback(transform.forward, pushbackStrength);
            }
        }

        /// <summary>
        /// Method used to constantly freeze the player with the slowMultiplier.
        /// </summary>
        private void FreezeAction()
        {
            foreach ( PlayerBase player in playersInTriggerzone )
            {
                player.FreezeContinously(slowMultiplier);
            }
        }

        /// <summary>
        /// Method used to freeze the player when they exit the fans triggerzone.
        /// </summary>
        /// <param name="player">The player to freeze.</param>
        private void FreezeOnExit(PlayerBase player)
        {
            player.Freeze(slowDuration, slowMultiplier);
        }

        /// <summary>
        /// Method used to stun the player in the triggerzone.
        /// </summary>
        private void StunAction()
        {
            foreach ( PlayerBase player in playersInTriggerzone )
            {
                player.Stun(stunDuration);
            }
        }

        /// <summary>
        /// Method used to increase the pushbackStrength with the speed float inherited
        /// from the TrapBase.
        /// </summary>
        public void IncreasePushback()
        {
            pushbackStrength += speed;
        }
    }
}
