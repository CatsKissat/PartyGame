using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using BananaSoup.Weapons;

namespace BananaSoup.PickUpSystem
{
    public class PickUpHandler : MonoBehaviour
    {
        [SerializeField, Tooltip("The desired child GameObject where you want to store equipped item(s).")]
        private Transform itemContainer = null;

        [Space]

        [SerializeField, Tooltip("The forward force added to an item when dropping it.")]
        private float dropForwardForce = 2.5f;
        [SerializeField, Tooltip("The upward force added to an item when dropping it.")]
        private float dropUpwardForce = 2.5f;

        [Space]

        [SerializeField, Tooltip("Time in seconds when the player can pick up an item after having one taken away.")]
        private float resetItemEquipped = 0.5f;

        private bool itemEquipped = false;

        private Rigidbody rb = null;
        private Coroutine itemTakenRoutine = null;

        private List<IPickUpable> pickUpablesInRange = new List<IPickUpable>();
        private IPickUpable pickedUpItem = null;
        private WeaponBase itemWeaponScript = null;


        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if ( rb == null )
            {
                Debug.LogError($"PickUpHandler couldn't find a Rigidbody on {name}!");
            }
        }

        #region OnTriggers
        /// <summary>
        /// OnTriggerEnter to add IPickUpables to the pickUpablesInRange list when they
        /// enter the players pick up radius (collider).
        /// </summary>
        /// <param name="other">The other GameObject's collider.</param>
        private void OnTriggerEnter(Collider other)
        {
            if ( other.TryGetComponent(out IPickUpable pickUpable) )
            {
                foreach ( IPickUpable p in pickUpablesInRange )
                {
                    if ( pickUpable == p )
                    {
                        return;
                    }
                }
                pickUpablesInRange.Add(pickUpable);
            }
        }

        /// <summary>
        /// OnTriggerExit to remove IPickUpables from the pickUpablesInRange list when
        /// they leave the pick up radius (collider) of the player.
        /// </summary>
        /// <param name="other">The other GameObject's collider.</param>
        private void OnTriggerExit(Collider other)
        {
            if ( other.TryGetComponent(out IPickUpable pickUpable) )
            {
                foreach ( IPickUpable p in pickUpablesInRange )
                {
                    if ( p == pickUpable )
                    {
                        pickUpablesInRange.Remove(pickUpable);
                        return;
                    }
                }
            }
        }
        #endregion

        #region OnInputs
        /// <summary>
        /// Method called when pressing the PickUpOrFire keybind.
        /// If the player doesn't have an item equipped call OnPickUpItem method.
        /// If the player has an item equipped call OnFire method.
        /// </summary>
        public void OnPickUpOrFire(InputAction.CallbackContext context)
        {
            if ( !itemEquipped && context.performed )
            {
                OnPickUpItem();
            }
            else if ( itemEquipped && context.performed )
            {
                OnFire();
            }
        }

        /// <summary>
        /// Method called when pressing the DropItem keybind.
        /// If no item is equipped return.
        /// If an item is equipped set its parent to null and call the items OnDrop method with
        /// correct parameters.
        /// Set pickedUpItem to be null and itemEquipped to false, and if there is a
        /// reference to a itemWeaponScript null it.
        /// </summary>
        public void OnDropItem(InputAction.CallbackContext context)
        {
            if ( !itemEquipped )
            {
                return;
            }

            if ( context.performed )
            {
                if ( pickedUpItem != null )
                {
                    pickedUpItem.Transform.parent = null;
                    pickedUpItem.OnDrop(rb.velocity, transform.right, transform.up, dropUpwardForce, dropForwardForce);
                    pickedUpItem = null;
                    itemEquipped = false;

                    if ( itemWeaponScript != null )
                    {
                        itemWeaponScript = null;
                    }
                }
                else
                {
                    return;
                }
            }
        }
        #endregion

        /// <summary>
        /// Method used when picking up an item.
        /// Returns if there are no IPickUpables in range of the player.
        /// If there are use GetNearestPickUpable to get the closest one to the player
        /// and then check if it is equipped by a player, if it is reset the other players
        /// PickUpHandler weapon references and itemEquipped.
        /// Then call the pickedUpItem's OnPickUp method to set the parent and rotation of the
        /// weapon and get a reference to the weapons WeaponBase if it has one.
        /// </summary>
        private void OnPickUpItem()
        {
            if ( pickUpablesInRange.Count == 0 )
            {
                return;
            }

            pickedUpItem = GetNearestPickUpable(pickUpablesInRange);

            if ( pickedUpItem != null && !pickedUpItem.Thrown)
            {
                if ( pickedUpItem.EquippedByAPlayer )
                {
                    PickUpHandler otherPickUpHandler = pickedUpItem.RootParent.GetComponent<PickUpHandler>();
                    otherPickUpHandler.ItemTakenAway();
                }

                pickedUpItem.OnPickUp(itemContainer, transform.rotation.eulerAngles);
                itemEquipped = true;

                itemWeaponScript = pickedUpItem.GameObject.GetComponent<WeaponBase>();
                if ( itemWeaponScript == null )
                {
                    Debug.Log($"{pickedUpItem} doesn't have a component of type {typeof(WeaponBase)}");
                }
            }
        }

        /// <summary>
        /// Method used to call the Fire method on the currently equipped weapon.
        /// </summary>
        private void OnFire()
        {
            if ( itemWeaponScript != null )
            {
                itemWeaponScript.Fire();
            }
        }

        /// <summary>
        /// Method used to get the nearest IPickUpable close to the player.
        /// The method goes through a list of IPickUpables and calculates which one is
        /// the closest to the player by comparing the distance between the player and
        /// the IPickUpables.
        /// </summary>
        /// <param name="pickUpables">The list of IPickUpables that are near the player.</param>
        /// <returns>The nearest IPickUpable to the player.</returns>
        private IPickUpable GetNearestPickUpable(List<IPickUpable> pickUpables)
        {
            IPickUpable closestPickupable = null;
            float minDistance = Mathf.Infinity;
            Vector3 currentPos = transform.position;

            foreach ( IPickUpable p in pickUpables )
            {
                float distance = Vector3.Distance(p.Position, currentPos);
                if ( distance < minDistance )
                {
                    closestPickupable = p;
                    minDistance = distance;
                }
            }

            return closestPickupable;
        }

        #region ItemTakenByAnotherPlayer
        /// <summary>
        /// Method used to handle other player taking away the weapon from the current
        /// holder. Reset references and itemEquipped bool.
        /// </summary>
        public void ItemTakenAway()
        {
            if ( pickedUpItem != null )
            {
                pickedUpItem = null;
            }

            if ( itemWeaponScript != null )
            {
                itemWeaponScript = null;
            }

            if ( itemEquipped )
            {
                if ( itemTakenRoutine == null )
                {
                    itemTakenRoutine = StartCoroutine(ItemTaken());
                }
                else
                {
                    StopCoroutine(itemTakenRoutine);
                    itemTakenRoutine = null;
                    itemTakenRoutine = StartCoroutine(ItemTaken());
                }
            }
        }

        /// <summary>
        /// Coroutine used to reset itemEquipped bool so that the player can't immediately
        /// steal back the weapon that was taken away from them.
        /// </summary>
        private IEnumerator ItemTaken()
        {
            yield return new WaitForSeconds(resetItemEquipped);
            itemEquipped = false;
            itemTakenRoutine = null;
        }
        #endregion
    }
}
