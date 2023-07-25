using UnityEngine;
using NaughtyAttributes;

namespace BananaSoup.Units
{
    public class UnitBase : MonoBehaviour
    {
        [SerializeField]
        protected bool baseHasRigidbody = true;
        [SerializeField, ShowIf(nameof(baseHasRigidbody))]
        protected bool isKinematic;
        [SerializeField, ShowIf(nameof(baseHasRigidbody))]
        protected bool isUsingGravity;

        protected Rigidbody rb;

        /// <summary>
        /// If the base is supposed to have a rigidbody call the GetReferences() and
        /// SetupRigidbody() methods.
        /// </summary>
        protected virtual void Start()
        {
            if ( baseHasRigidbody )
            {
                GetReferences();
                SetupRigidbody(); 
            }
        }

        /// <summary>
        /// Used to get a reference to the GameObject's Rigidbody.
        /// If it can't be found log an error.
        /// </summary>
        private void GetReferences()
        {
            rb = GetComponent<Rigidbody>();
            if ( rb == null )
            {
                Debug.LogError($"{name} is missing a Rigidbody!");
            }
        }

        /// <summary>
        /// Method used to setup the Rigidbody with the serialized values isUsingGravity and isKinematic.
        /// </summary>
        protected virtual void SetupRigidbody()
        {
            rb.useGravity = isUsingGravity;
            rb.isKinematic = isKinematic;
        }
    }
}
