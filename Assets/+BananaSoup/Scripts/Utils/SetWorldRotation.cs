using UnityEngine;
using NaughtyAttributes;

namespace BananaSoup.Utils
{
    public class SetWorldRotation : MonoBehaviour
    {
        [SerializeField, Tooltip("The desired worldrotation for the GameObject.")]
        private Vector3 rotation = Vector3.zero;

        [Button("Get current rotation")]
        private void GetRotation()
        {
            rotation = transform.rotation.eulerAngles;
        }

        private void Update()
        {
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
