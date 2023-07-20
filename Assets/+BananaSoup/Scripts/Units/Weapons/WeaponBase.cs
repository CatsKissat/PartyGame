using System.Collections.Generic;
using UnityEngine;
using BananaSoup.PickUpSystem;
using BananaSoup.Units;
using BananaSoup.Utils;
using System.Collections;

namespace BananaSoup.Weapons
{
    public class WeaponBase : UnitBase, IPickUpable
    {
        [Space]
        [SerializeField, Tooltip("The prefab of the item to be pooled and spawned/fired.")]
        protected ProjectileBase prefab;
        [SerializeField, Tooltip("The projectiles alive time.")]
        protected float projectileAliveTime = 5.0f;
        [SerializeField, Tooltip("The capacity of the pool.")]
        protected int capacity = 1;
        [SerializeField, Tooltip("The players LayerMask.")]
        protected LayerMask playersLayerMask;

        [Space]

        [SerializeField, Tooltip("The desired speed of the projectile.")]
        protected float projectileSpeed = 2.5f;

        [Space]

        [SerializeField, Tooltip("The transform of the firing point of the weapon.")]
        protected GameObject firingPoint;
        [SerializeField, Tooltip("The duration the weapon can't be picked up after being thrown.")]
        float thrownResetTime = 0.5f;

        protected bool equipped = false;
        protected bool thrown = false;

        protected Coroutine resetThrownRoutine = null;

        private Collider[] colliders;

        protected ComponentPool<ProjectileBase> pool;
        protected Transform projectilePool = null;

        // Const string to used to find ProjectilePool GameObject with tag
        private const string projectilePoolTag = "ProjectilePool";

        protected ComponentPool<ProjectileBase> Pool
        {
            get { return pool; }
        }

        #region IPickUpable
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        public bool PickedUp => equipped;
        public bool Thrown => thrown;

        public Vector3 Position => transform.position;

        public void OnPickUp(Transform container, Vector3 rotation)
        {
            equipped = true;
            rb.isKinematic = true;

            transform.rotation = Quaternion.Euler(Vector3.zero);

            transform.rotation = Quaternion.Euler(rotation);
            transform.parent = container;
            transform.position = container.position;

            foreach ( Collider col in colliders )
            {
                col.isTrigger = true;
            }

        }

        /// <summary>
        /// Method called when dropping a weapon.
        /// The method makes the weapons equipped Bool, Rigidbodys isKinematic value and
        /// Colliders isTrigger value false.
        /// Then we make the players current velocity the weapons velocity and add a
        /// forward and upward force.
        /// </summary>
        /// <param name="currentVelocity">The players Rigidbody's current velocity.</param>
        /// <param name="upWardForce">The desired upward force to apply to the weapon.</param>
        /// <param name="forwardForce">The desired forward force to apply to the weapon.</param>
        public void OnDrop(Vector3 currentVelocity, Vector3 forward, Vector3 up, float upWardForce, float forwardForce)
        {
            equipped = false;

            rb.isKinematic = false;
            rb.useGravity = true;

            foreach ( Collider col in colliders )
            {
                col.isTrigger = false;
            }

            // Make players velocity weapons velocity and add force.
            rb.velocity = currentVelocity;
            rb.AddForce(up * upWardForce, ForceMode.Impulse);
            rb.AddForce(forward * forwardForce, ForceMode.Impulse);

            // Randomize the X-rotation of the weapon.
            float random = Random.Range(-1f, 1f);
            Vector3 randomRotate = new Vector3(0, 0, random);
            rb.AddTorque(randomRotate * 10);

            if ( resetThrownRoutine == null )
            {
                resetThrownRoutine = StartCoroutine(ResetThrown());
            }
        }
        #endregion

        protected void OnDisable()
        {
            if ( resetThrownRoutine != null )
            {
                StopCoroutine(resetThrownRoutine);
                resetThrownRoutine = null;
            }
        }

        protected override void Start()
        {
            base.Start();
            Setup();
        }

        protected virtual void Setup()
        {
            GetReferences();
        }

        #region GetReferences
        /// <summary>
        /// Method used to get references and throw error(s) if they can't be found.
        /// </summary>
        private void GetReferences()
        {
            rb = GetComponent<Rigidbody>();
            if ( rb == null )
            {
                Debug.LogError($"WeaponBase on {name} couldn't find a component of type {typeof(Rigidbody)} on {name}!");
            }

            projectilePool = GameObject.FindGameObjectWithTag(projectilePoolTag).transform;
            if ( projectilePool == null )
            {
                Debug.LogError($"{name} couldn't find an object with the tag {projectilePoolTag} in the scene!");
            }

            colliders = GetComponents<Collider>();
        }
        #endregion

        #region Pool
        /// <summary>
        /// Method used to get all the projectiles in the pool of this object.
        /// Then get the transforms of those objects and return the array of transforms.
        /// </summary>
        /// <returns>A Transform array of the projectiles transforms.</returns>
        protected virtual Transform[] GetPooledProjectilesTransforms()
        {
            List<ProjectileBase> projectiles = pool.GetAllItems();
            Transform[] projectileTransforms = new Transform[capacity];

            for ( int i = 0; i < capacity; i++ )
            {
                projectileTransforms[i] = projectiles[i].transform;
            }

            return projectileTransforms;
        }

        /// <summary>
        /// Method used to activate a pooled object.
        /// </summary>
        /// <param name="position">The position where the item should be placed.</param>
        /// <returns>The item created in the given position.</returns>
        protected virtual ProjectileBase Create(Vector3 position)
        {
            ProjectileBase item = pool.Get();
            if ( item != null )
            {
                item.transform.position = position;
            }

            return item;
        }

        /// <summary>
        /// Method used to reycle a pooled object.
        /// </summary>
        /// <param name="item">The item to be recycled.</param>
        /// <returns>True if successful, otherwise false.</returns>
        protected virtual bool Recycle(ProjectileBase item)
        {
            return pool.Recycle(item);
        }
        #endregion

        public virtual void Fire()
        {
            
        }

        protected IEnumerator ResetThrown()
        {
            Debug.Log($"{name} was thrown!");
            thrown = true;
            yield return new WaitForSeconds(thrownResetTime);
            thrown = false;
            resetThrownRoutine = null;
            Debug.Log($"{name} can be picked up again!");
        }
    }
}
