using UnityEngine;

namespace BananaSoup.Units
{
    public class UnitBase : MonoBehaviour
    {
        [SerializeField]
        protected bool isKinematic;
        [SerializeField]
        protected bool isUsingGravity;

        protected Rigidbody rb;

        protected virtual void Start()
        {
            GetReferences();
            SetupRigidbody();
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
