using UnityEngine;
using BananaSoup.Utils;
using BananaSoup.Units;

namespace BananaSoup.Traps
{
    public class LaserProjectileSpawner : MonoBehaviour
    {
        [SerializeField, Tooltip("The prefab of the item to be pooled and spawned/fired.")]
        private LaserProjectile prefab;
        [SerializeField, Tooltip("The capacity of the pool.")]
        private int capacity = 1;

        private TrapBase trapBase = null;
        private ModifierActions modActions = null;

        private ComponentPool<LaserProjectile> pool;

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
        }


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
        }

        /// <summary>
        /// Method used to activate a pooled object.
        /// </summary>
        /// <param name="position">The position where the item should be placed.</param>
        /// <param name="rotation">The rotation the item should be created in.</param>
        /// <param name="parent">The desired parent for the object.
        /// Null if you want it to be put in the scene's root level.</param>
        /// <returns></returns>
        public LaserProjectile Create(Vector3 position, Quaternion rotation, Transform parent)
        {
            LaserProjectile item = pool.Get();
            if ( item != null )
            {
                // If the parent is null, Unity will put the GameObject to scene's root level.
                item.transform.parent = parent;
                item.transform.localPosition = position;
                item.transform.localRotation = rotation;
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
