using UnityEngine;

namespace BananaSoup.PickUpSystem
{
    public class PickUpable : MonoBehaviour, IPickUpable
    {
        public GameObject GameObject => throw new System.NotImplementedException();
        public Transform Transform => throw new System.NotImplementedException();
        public bool PickedUp => throw new System.NotImplementedException();
        public bool Thrown => throw new System.NotImplementedException();

        public Vector3 Position => throw new System.NotImplementedException();
        public void OnPickUp(Transform container, Vector3 rotation)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrop(Vector3 currentVelocity, Vector3 forward, Vector3 up, float upWardForce, float forwardForce)
        {
            throw new System.NotImplementedException();
        }
    }
}
