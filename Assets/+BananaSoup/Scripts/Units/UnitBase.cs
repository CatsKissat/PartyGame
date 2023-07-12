using UnityEngine;
using NaughtyAttributes;

namespace BananaSoup.Units
{
    public class UnitBase : MonoBehaviour
    {
        [SerializeField]
        protected bool baseHasRigidbody = true;
        [SerializeField, ShowIf("baseHasRigidbody")]
        protected bool isKinematic;
        [SerializeField, ShowIf("baseHasRigidbody")]
        protected bool isUsingGravity;

        protected Rigidbody rb;

        protected virtual void Start()
        {
            if ( baseHasRigidbody )
            {
                GetReferences();
                SetupRigidbody(); 
            }
        }

        private void GetReferences()
        {
            rb = GetComponent<Rigidbody>();
            if ( rb == null )
            {
                Debug.LogError($"{name} is missing a Rigidbody!");
            }
        }

        protected virtual void SetupRigidbody()
        {
            rb.useGravity = isUsingGravity;
            rb.isKinematic = isKinematic;
        }
    }
}
