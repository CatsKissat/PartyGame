using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BananaSoup
{
    public class PickUpHandler : MonoBehaviour
    {
        [SerializeField]
        private Collider pickUpRadius = null;

        [Space]

        [SerializeField]
        private float dropForwardForce = 2.5f;
        [SerializeField]
        private float dropUpwardForce = 2.5f;

        private bool itemInPickUpRange = false;
        private bool itemEquipped = false;

        private Rigidbody weaponRb = null;

        private List<PickUpable> pickUpablesInRange = new List<PickUpable>();

        private void OnTriggerEnter(Collider other)
        {
            
        }

        private void OnTriggerExit(Collider other)
        {
            
        }

        public void OnPickUpItem(InputAction.CallbackContext context)
        {
            if ( context.performed )
            {

            }
        }

        public void OnDropItem(InputAction.CallbackContext context)
        {

        }
    }
}
