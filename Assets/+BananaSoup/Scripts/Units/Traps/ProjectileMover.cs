using UnityEngine;

namespace BananaSoup
{
    public class ProjectileMover : MonoBehaviour
    {
        private Rigidbody rb;
        private Vector3 direction;

        public float MovementSpeed
        {
            get;
            private set;
        }

        public void Move(Vector3 direction)
        {
            this.direction = direction;
        }

        public void Setup(float speed)
        {
            MovementSpeed = speed;
            rb = GetComponent<Rigidbody>();
            
            if ( rb == null )
            {
                Debug.LogError($"{name} doesn't have a Rigidbody!");
            }

            Debug.Log("ProjectileMover setup ready!");
        }

        private void FixedUpdate()
        {
            if ( rb == null )
            {
                return;
            }

            Move(Time.fixedDeltaTime);
        }

        private void Move(float deltaTime)
        {
            // Calculate the movement vector and add that to current position.
            Vector3 movement = direction * MovementSpeed * deltaTime;
            Vector3 newPosition = rb.position + movement;
            rb.MovePosition(newPosition);

            // Reset direction after movement.
            direction = Vector3.zero;
        }
    }
}
