using UnityEngine;

namespace BananaSoup.PickUpSystem
{
    public interface IPickUpable
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        bool PickedUp { get; }
        Vector3 Position { get; }

        void OnPickUp(Transform container, Vector3 localScale);
        void OnDrop(Vector3 currentVelocity, Vector3 forward, Vector3 up, float upWardForce, float forwardForce);
    }
}
