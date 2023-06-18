using UnityEngine;

namespace BananaSoup.Utils
{
    public abstract class PooledSpawner<TComponent> : MonoBehaviour
        where TComponent : Component
    {
        [SerializeField] private TComponent prefab;
        [SerializeField] private int capacity = 1;

        private ComponentPool<TComponent> pool;

        protected ComponentPool<TComponent> Pool
        {
            get { return pool; }
        }


        public TComponent Prefab
        {
            get { return this.prefab; }
            protected set { this.prefab = value; }
        }

        public virtual void Setup(TComponent prefab = null)
        {
            if ( prefab != null )
            {
                this.prefab = prefab;
            }
            pool = new ComponentPool<TComponent>(Prefab, capacity);
        }


        public virtual TComponent Create(Vector3 position, Quaternion rotation, Transform parent)
        {
            TComponent item = pool.Get();
            if ( item != null )
            {
                // If the parent is null, Unity will put the GameObject to scene's root level.
                // TODO: Local or global positions?
                item.transform.parent = parent;
                item.transform.localPosition = position;
                item.transform.localRotation = rotation;
            }

            return item;
        }

        public virtual bool Recycle(TComponent item)
        {
            return pool.Recycle(item);
        }

    }
}
