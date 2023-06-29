using UnityEngine;
using Cinemachine;

namespace BananaSoup
{
    public class CameraTargetAssigner : MonoBehaviour
    {
        private CinemachineTargetGroup targetGroup;
        private float weight = 1.0f;
        private float radius = 2.0f;

        private void Start()
        {
            GetReferences();
        }

        private void GetReferences()
        {
            targetGroup = GetComponent<CinemachineTargetGroup>();
            if ( targetGroup == null )
            {
                Debug.LogError($"{name} is missing a CinemachineTargetGroup!");
            }
        }

        public void AssingPlayer(Transform playerTransform)
        {
            targetGroup.AddMember(playerTransform, weight, radius);
        }

        public void RemovePlayerTarget(Transform playerTransform)
        {
            targetGroup.RemoveMember(playerTransform);
        }
    }
}
