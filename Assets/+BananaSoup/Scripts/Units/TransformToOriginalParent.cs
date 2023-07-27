using UnityEngine;
using BananaSoup.Managers;

namespace BananaSoup.Units
{
    public class TransformToOriginalParent : MonoBehaviour
    {
        private Transform originalParent;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private GameManager gameManager;

        private void Start()
        {
            GetReferences();

            originalParent = transform.parent;
            originalPosition = transform.position;
            originalRotation = transform.rotation;

            gameManager.NewRound += SetPositionAndParentToOriginal;
        }

        private void OnDisable()
        {
            gameManager.NewRound -= SetPositionAndParentToOriginal;
        }

        private void GetReferences()
        {
            gameManager = FindObjectOfType<GameManager>();
            if ( gameManager == null )
            {
                Debug.LogError($"{name} is missing a reference to the GameManager!");
            }
        }

        private void SetPositionAndParentToOriginal()
        {
            transform.parent = originalParent;
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }
    }
}
