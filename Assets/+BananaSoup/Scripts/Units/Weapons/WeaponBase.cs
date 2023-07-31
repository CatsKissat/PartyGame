using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using BananaSoup.PickUpSystem;
using BananaSoup.Units;
using BananaSoup.Utils;
using BananaSoup.Managers;

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

        [SerializeField, Tooltip("The amount of bullets the gun has.")]
        protected int bullets = 10;
        [SerializeField, Tooltip("The desired speed of the projectile.")]
        protected float projectileSpeed = 2.5f;

        [Space]

        [SerializeField, Tooltip("The transform of the firing point of the weapon.")]
        protected GameObject firingPoint;
        [SerializeField, Tooltip("The duration the weapon can't be picked up after being thrown.")]
        float thrownResetTime = 0.5f;

        public bool onPedestal = false;

        protected bool equippedByAPlayer = false;
        protected bool thrown = false;

        protected int bulletsLeft = 0;

        protected Coroutine resetThrownRoutine = null;

        private Collider[] colliders;
        private GameManager gameManager;

        protected ComponentPool<ProjectileBase> pool;
        protected Transform projectilePool = null;

        public event Action WeaponPickedUp;

        // Const string to used to find ProjectilePool GameObject with tag
        private const string projectilePoolTag = "ProjectilePool";

        protected ComponentPool<ProjectileBase> Pool
        {
            get { return pool; }
        }

        #region IPickUpable
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        public Transform RootParent => transform.root;
        public bool EquippedByAPlayer => equippedByAPlayer;
        public bool Thrown => thrown;

        public Vector3 Position => transform.position;

        /// <summary>
        /// Method called when the weapon is picked up.
        /// Used to set the weapons equippedByAPlayer true, set the weapons Rigidbody
        /// to be kinematic, set the weapons rotation with the parameter rotation,
        /// set the weapons parent to be the parameter Transform container,
        /// set the position to be the containers position and then set the weapons
        /// colliders to be triggers.
        /// </summary>
        /// <param name="container">The desired container for the weapon.</param>
        /// <param name="rotation">The desired rotation for the weapon.</param>
        public void OnPickUp(Transform container, Vector3 rotation)
        {
            equippedByAPlayer = true;
            onPedestal = false;
            rb.isKinematic = true;

            transform.rotation = Quaternion.Euler(rotation);
            transform.parent = container;
            transform.position = container.position;

            foreach ( Collider col in colliders )
            {
                col.isTrigger = true;
            }

            if ( WeaponPickedUp != null )
            {
                WeaponPickedUp();
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
            equippedByAPlayer = false;

            rb.isKinematic = false;
            rb.useGravity = true;

            foreach ( Collider col in colliders )
            {
                col.isTrigger = false;
            }

            // Give weapons Rigidbody the players velocity and add force.
            rb.velocity = currentVelocity;
            rb.AddForce(up * upWardForce, ForceMode.Impulse);
            rb.AddForce(forward * forwardForce, ForceMode.Impulse);

            // Randomize the Z-rotation of the weapon.
            float random = UnityEngine.Random.Range(-1f, 1f);
            Vector3 randomRotate = new Vector3(0, 0, random);
            rb.AddTorque(randomRotate * 10);

            if ( resetThrownRoutine == null )
            {
                resetThrownRoutine = StartCoroutine(ResetThrown());
            }
        }
        #endregion

        /// <summary>
        /// If resetThrownRoutine isn't null when the GameObject is disabled
        /// stop the resetThrownRoutine coroutine and null it.
        /// </summary>
        protected void OnDisable()
        {
            TryStopAndNullRoutine(resetThrownRoutine);

            gameManager.NewRound -= SetupNewRound;
        }

        /// <summary>
        /// If the weapons thrown bool is true the GameObject is enabled set it false.
        /// </summary>
        protected void OnEnable()
        {
            if ( thrown )
            {
                thrown = false;
            }
        }

        /// <summary>
        /// Call the UnitBase Start() here with base.Start().
        /// Then call the Setup() method.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            Setup();
        }

        /// <summary>
        /// Method used to Setup the weapon.
        /// Call GetReferences method.
        /// </summary>
        protected virtual void Setup()
        {
            GetReferences();
            bulletsLeft = bullets;

            gameManager.NewRound += SetupNewRound;
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

            gameManager = FindObjectOfType<GameManager>();
            if ( gameManager == null )
            {
                Debug.LogError($"{name} is missing a reference to the GameManager!");
            }
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

        /// <summary>
        /// Method used to fire the weapon.
        /// </summary>
        public virtual void Fire()
        {
            
        }

        /// <summary>
        /// Method used to reduce bullets left in the weapon.
        /// </summary>
        protected void ReduceBulletsLeft()
        {
            if ( bulletsLeft > 0 )
            {
                bulletsLeft--;
            }
        }

        /// <summary>
        /// Coroutine to add a delay to picking up the weapon again after it is thrown.
        /// </summary>
        protected IEnumerator ResetThrown()
        {
            thrown = true;
            yield return new WaitForSeconds(thrownResetTime);
            thrown = false;
            resetThrownRoutine = null;
        }

        /// <summary>
        /// Method called when a new round starts. Used to destroy weapons which aren't
        /// on a pedestal and to ensure that the ones on pedestals are on their desired
        /// state(s).
        /// </summary>
        private void SetupNewRound()
        {
            if ( !onPedestal )
            {
                Destroy(gameObject);
                return;
            }

            TryStopAndNullRoutine(resetThrownRoutine);

            equippedByAPlayer = false;
            thrown = false;
        }

        /// <summary>
        /// Method used to try to stop and null a coroutine.
        /// </summary>
        /// <param name="routine">The routine to try to stop and null.</param>
        private void TryStopAndNullRoutine(Coroutine routine)
        {
            if ( routine != null )
            {
                StopCoroutine(routine);
                routine = null;
            }
        }
    }
}
