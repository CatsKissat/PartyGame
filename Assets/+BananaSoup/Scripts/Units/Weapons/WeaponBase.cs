using System.Collections.Generic;
using UnityEngine;
using BananaSoup.Units;
using BananaSoup.Utils;

namespace BananaSoup.Weapons
{
    public class WeaponBase : UnitBase
    {
        protected bool equipped = false;

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

        private GameObject spriteRendererObject = null;
        protected Vector3 spriteRotation = Vector3.zero;

        protected ComponentPool<ProjectileBase> pool;

        protected Transform projectilePool = null;

        // Const string to used to find ProjectilePool GameObject with tag
        private const string projectilePoolTag = "ProjectilePool";

        protected ComponentPool<ProjectileBase> Pool
        {
            get { return pool; }
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

        /// <summary>
        /// Method used to get references and throw error(s) if they can't be found.
        /// </summary>
        private void GetReferences()
        {
            projectilePool = GameObject.FindGameObjectWithTag(projectilePoolTag).transform;
            if ( projectilePool == null )
            {
                Debug.LogError($"{name} couldn't find an object with the tag {projectilePoolTag} in the scene!");
            }

            spriteRendererObject = GetComponentInChildren<SpriteRenderer>(true).gameObject;
            if ( spriteRendererObject == null )
            {
                Debug.LogError($"{name} couldn't find a GameObject with a component of type {typeof(SpriteRenderer)} on it's children!");
            }
            else
            {
                spriteRotation = spriteRendererObject.transform.rotation.eulerAngles;
            }
        }

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

        protected virtual void Fire()
        {

        }
    }
}
