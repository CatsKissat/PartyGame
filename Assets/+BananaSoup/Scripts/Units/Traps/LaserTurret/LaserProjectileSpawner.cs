using UnityEngine;
using BananaSoup.Utils;
using BananaSoup.Units;
using System.Collections.Generic;

namespace BananaSoup.Traps
{
    public class LaserProjectileSpawner : MonoBehaviour
    {
        [SerializeField, Tooltip("The prefab of the item to be pooled and spawned/fired.")]
        private LaserProjectile prefab;
        [SerializeField, Tooltip("The capacity of the pool.")]
        private int capacity = 1;

        private ComponentPool<LaserProjectile> pool;

        private TrapBase trapBase = null;
        private ModifierActions modActions = null;
        private Transform projectilePool = null;

        // Const string to used to find ProjectilePool GameObject with tag
        private const string projectilePoolTag = "ProjectilePool";

        protected ComponentPool<LaserProjectile> Pool
        {
            get { return pool; }
        }

        public LaserProjectile Prefab
        {
            get { return this.prefab; }
            protected set { this.prefab = value; }
        }

        public void Setup(LaserProjectile prefab = null)
        {
            GetReferences();

            if ( prefab != null )
            {
                this.prefab = prefab;
            }

            pool = new ComponentPool<LaserProjectile>(Prefab, capacity);
            pool.SetPooledObjectsParent(GetPooledProjectilesTransforms(), projectilePool);
        }

        /// <summary>
        /// Method used to get references and throw error(s) if they can't be found.
        /// </summary>
        private void GetReferences()
        {
            trapBase = GetComponent<TrapBase>();
            if ( trapBase == null )
            {
                Debug.LogError($"LaserProjectileSpawner couldn't find a component of type TrapBase on {name}!");
            }

            modActions = GetComponent<ModifierActions>();
            if ( modActions == null )
            {
                Debug.LogError($"LaserProjectileSpawner couldn't find a component of type ModifierActions on {name}!");
            }

            projectilePool = GameObject.FindGameObjectWithTag(projectilePoolTag).transform;
            if ( projectilePool == null )
            {
                Debug.LogError($"{name} couldn't find an object with the tag {projectilePoolTag} in the scene!");
            }
        }

        /// <summary>
        /// Method used to get all the projectiles in the pool of this object.
        /// Then get the transforms of those objects and return the array of transforms.
        /// </summary>
        /// <returns>A Transform array of the projectiles transforms.</returns>
        private Transform[] GetPooledProjectilesTransforms()
        {
            List<LaserProjectile> projectiles = pool.GetAllItems();
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
        public LaserProjectile Create(Vector3 position)
        {
            LaserProjectile item = pool.Get();
            if ( item != null )
            {
                item.transform.position = position;
                SetupProjectile(item);
            }

            return item;
        }
        private void SetupProjectile(LaserProjectile projectile)
        {
            projectile.SetupModifierVariables(modActions.PlayersLayerMask, modActions.SlowAmount,
                                              modActions.SlowDuration, modActions.StunDuration,
                                              trapBase.TrapModifier, trapBase.ModifiedSize);
        }

        /// <summary>
        /// Method used to reycle a pooled object.
        /// </summary>
        /// <param name="item">The item to be recycled.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Recycle(LaserProjectile item)
        {
            return pool.Recycle(item);
        }
    }
}
