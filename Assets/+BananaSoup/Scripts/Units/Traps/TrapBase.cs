using UnityEngine;
using BananaSoup.Modifiers;

namespace BananaSoup.Units
{
    public class TrapBase : UnitBase
    {
        [SerializeField]
        private ModifierType.Modifier currentModifier;

        private Rigidbody rb = null;
        private Rigidbody[] childRigidbodies = null;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            if ( rb == null )
            {
                Debug.LogWarning($"There is no Rigidbody on {name} is this on purpose?");
            }
            else
            {
                rb.useGravity = isUsingGravity;
                rb.isKinematic = isKinematic;
            }

            childRigidbodies = GetComponentsInChildren<Rigidbody>();

            if ( childRigidbodies != null )
            {
                foreach ( Rigidbody rb in childRigidbodies )
                {
                    rb.useGravity = isUsingGravity;
                    rb.isKinematic = isKinematic;
                } 
            }
        }
    }
}
