using UnityEngine;
using NaughtyAttributes;

namespace BananaSoup.Utils
{
    public class SetWorldRotation : MonoBehaviour
    {
        [SerializeField, Tooltip("The desired worldrotation for the GameObject.")]
        private Vector3 rotation = Vector3.zero;

        /// <summary>
        /// Method assigned to a button to get the current rotation of the GameObject.
        /// </summary>
        [Button("Get current rotation")]
        private void GetRotation()
        {
            rotation = transform.rotation.eulerAngles;
        }

        /// <summary>
        /// Keep updating the GameObjects world rotation to the desired rotation.
        /// </summary>
        private void Update()
        {
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
