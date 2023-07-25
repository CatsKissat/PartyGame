using UnityEngine;

namespace BananaSoup.PickUpSystem
{
    public interface IPickUpable
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        Transform RootParent { get; }
        bool EquippedByAPlayer { get; }
        bool Thrown { get; }
        Vector3 Position { get; }

        void OnPickUp(Transform container, Vector3 rotation);
        void OnDrop(Vector3 currentVelocity, Vector3 forward, Vector3 up, float upWardForce, float forwardForce);
    }
}
