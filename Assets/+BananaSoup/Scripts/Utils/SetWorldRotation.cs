using UnityEngine;
using NaughtyAttributes;
using System.Collections;

namespace BananaSoup.Utils
{
    public class SetWorldRotation : MonoBehaviour
    {
        [SerializeField, Tooltip("The desired worldrotation for the GameObject.")]
        private Vector3 rotation = Vector3.zero;
        
        // The time to delay start in deconds.
        private float startDelay = 0.1f;

        private Coroutine delayedStartRoutine = null;

        /// <summary>
        /// If delayed start isn't null when disabling the GameObject stop it and null it.
        /// </summary>
        private void OnDisable()
        {
            if ( delayedStartRoutine != null )
            {
                StopCoroutine(delayedStartRoutine);
                delayedStartRoutine = null;
            }
        }

        /// <summary>
        /// Method assigned to a button to get the current rotation of the GameObject.
        /// </summary>
        [Button("Get current rotation")]
        private void GetRotation()
        {
            rotation = transform.rotation.eulerAngles;
        }

        /// <summary>
        /// The desired world rotation of the GameObject is set in start after a
        /// desired delay (in seconds).
        /// </summary>
        private void Start()
        {
            if ( delayedStartRoutine == null )
            {
                delayedStartRoutine = StartCoroutine(DelayStart());
            }

            transform.rotation = Quaternion.Euler(rotation);
        }

        /// <summary>
        /// Coroutine used to delay Start().
        /// </summary>
        private IEnumerator DelayStart()
        {
            yield return new WaitForSeconds(startDelay);
        }
    }
}
