using BananaSoup.Weapons;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaSoup.PickUpSystem
{
    public class PickUpHandler : MonoBehaviour
    {
        [SerializeField]
        private Transform weaponContainer = null;

        [Space]

        [SerializeField]
        private float dropForwardForce = 2.5f;
        [SerializeField]
        private float dropUpwardForce = 2.5f;

        private bool itemEquipped = false;

        private List<IPickUpable> pickUpablesInRange = new List<IPickUpable>();
        private IPickUpable pickedUpItem = null;
        private WeaponBase itemWeaponScript = null;

        private Rigidbody rb = null;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if ( rb == null )
            {
                Debug.LogError($"PickUpHandler couldn't find a Rigidbody on {name}!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ( other.TryGetComponent(out IPickUpable pickUpable) )
            {
                foreach ( IPickUpable p in pickUpablesInRange )
                {
                    if ( pickUpable == p )
                    {
                        Debug.Log("The pickUpable is already on the list!");
                        return;
                    }
                }

                pickUpablesInRange.Add(pickUpable);
                Debug.Log("Added a pickupable to pickupables!");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ( other.TryGetComponent(out IPickUpable pickUpable) )
            {
                foreach ( IPickUpable p in pickUpablesInRange )
                {
                    if ( p == pickUpable )
                    {
                        pickUpablesInRange.Remove(pickUpable);
                        Debug.Log("Removed a pickupable from pickupables!");
                        return;
                    }
                }

                Debug.Log("No pickupables to remove!");
            }
        }

        #region OnInputs
        public void OnPickUpOrFire(InputAction.CallbackContext context)
        {
            if ( !itemEquipped && context.performed )
            {
                OnPickUpItem();
            }
            else if ( itemEquipped && context.performed )
            {
                Debug.Log("Player is trying to fire the weapon!");
                OnFire();
            }
        }

        public void OnDropItem(InputAction.CallbackContext context)
        {
            if ( !itemEquipped )
            {
                Debug.Log($"{name} tried to drop an item, but doesn't have one!");
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
                    Debug.Log($"{name} tried to drop an item, but doesn't have one!");
                }
            }
        }
        #endregion

        private void OnPickUpItem()
        {
            if ( pickUpablesInRange.Count == 0 )
            {
                Debug.Log($"There are no items to pick up near {name}!");
                return;
            }

            pickedUpItem = GetNearestPickUpable(pickUpablesInRange);

            if ( pickedUpItem != null && !pickedUpItem.Thrown)
            {
                pickedUpItem.OnPickUp(weaponContainer, transform.rotation.eulerAngles);
                itemEquipped = true;

                itemWeaponScript = pickedUpItem.GameObject.GetComponent<WeaponBase>();
                if ( itemWeaponScript == null )
                {
                    Debug.Log($"{pickedUpItem} doesn't have a component of type {typeof(WeaponBase)}");
                }
            }
        }

        private void OnFire()
        {
            if ( itemWeaponScript != null )
            {
                itemWeaponScript.Fire();
            }
        }

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
    }
}
