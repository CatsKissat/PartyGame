using UnityEngine;

namespace BananaSoup.Utils
{
    public class ProjectileMover : MonoBehaviour
    {
        // Vector3 to store the direction of movement.
        private Vector3 direction;

        // References
        private Rigidbody rb;

        /// <summary>
        /// Public property used to store and share the MovementSpeed of the projectile.
        /// </summary>
        public float MovementSpeed
        {
            get;
            private set;
        }

        /// <summary>
        /// Method called to set the direction of the projectile.
        /// </summary>
        /// <param name="direction">The desired direction. (Usually transform.right)</param>
        public void SetDirection(Vector3 direction)
        {
            this.direction = direction;
        }

        /// <summary>
        /// Method called to setup the projectile.
        /// Set MovementSpeed to be the given float parameter, then get a reference
        /// of the projectiles Rigidbody and give an error if it doesn't have one.
        /// </summary>
        /// <param name="speed"></param>
        public void Setup(float speed)
        {
            MovementSpeed = speed;
            rb = GetComponent<Rigidbody>();
            
            if ( rb == null )
            {
                Debug.LogError($"{name} doesn't have a Rigidbody!");
            }
        }

        /// <summary>
        /// If the rb reference variable is null return.
        /// If not call this scripts private Move with the desired deltaTime.
        /// (usually Time.fixedDeltaTime)
        /// </summary>
        private void FixedUpdate()
        {
            if ( rb == null )
            {
                return;
            }

            Move(Time.fixedDeltaTime);
        }

        /// <summary>
        /// Method used to actually move the projectile.
        /// First calculate the movement vector and then add that to the current position.
        /// Then call rb.MovePosition with the newPosition and after that reset the
        /// direction.
        /// </summary>
        /// <param name="deltaTime">The desired deltaTime.</param>
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
