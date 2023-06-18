using System.Collections.Generic;
using UnityEngine;

namespace BananaSoup.Utils
{
    public abstract class Pool<T>
        where T : Object
    {
        private T prefab;
        private List<T> items;

        /// <summary>
        /// Creates a new pool.
        /// </summary>
        protected Pool(T prefab, int sizeOfPool)
        {
            this.prefab = prefab;
            items = new List<T>(sizeOfPool);

            for ( int i = 0; i < sizeOfPool; i++ )
            {
                Add();
            }
        }

        /// <summary>
        /// Gets the first inactive object from the pool.
        /// </summary>
        /// <returns>The first inactive object from the pool, null if there are none.</returns>
        public virtual T Get()
        {
            T item = null;

            for ( int i = 0; i < items.Count; i++ )
            {
                T currentItem = items[i];

                if ( currentItem != null && !IsActive(currentItem) )
                {
                    item = currentItem;

                    // Break to stop the loop, as a result has already been found.
                    break;
                }
                else if ( currentItem == null )
                {
                    Debug.LogError("A pooled item is null! An item has been destroyed " +
                        "from the pool!");
                }
            }

            if ( item != null )
            {
                SetActive(item, setActive: true);
            }

            return item;
        }

        /// <summary>
        /// Checks is there a inactive item in a pool.
        /// </summary>
        /// <returns>True = there is an inactive item. False = there are no inactive item in a pool, every item is currently in use.</returns>
        public bool DoesPoolHaveInactiveItem()
        {
            for ( int i = 0; i < items.Count; i++ )
            {
                T currentItem = items[i];

                if ( currentItem != null && !IsActive(currentItem) )
                {
                    return true;
                }
                else if ( currentItem == null )
                {
                    Debug.LogError("A pooled item is null! An item has been destroyed " +
                        "from the pool!");
                }
            }

            return false;
        }

        /// <summary>
        /// Call this method to return all currently active objects.
        /// </summary>
        /// <returns>List of active objects.</returns>
        public List<T> GetActiveItems()
        {
            List<T> activeItems = new List<T>();
            foreach ( T item in items )
            {
                if ( IsActive(item) )
                {
                    activeItems.Add(item);
                }
            }

            return activeItems;
        }

        /// <summary>
        /// Recycles an object back to the pool.
        /// </summary>
        /// <param name="item">Object to be recycled.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Recycle(T item)
        {
            if ( !IsActive(item) )
            {
                // The item is inactive, no need for recycling.
                return false;
            }

            // For-loop to check if the recycled item is from this pool.
            for ( int i = 0; i <= items.Count; i++ )
            {
                T currentItem = items[i];

                if ( currentItem == item )
                {
                    SetActive(item, setActive: false);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Use this method to return all objects back to the pool.
        /// </summary>
        public void Reset()
        {
            foreach ( var item in items )
            {
                SetActive(item, false);
            }
        }

        /// <summary>
        /// Use this method to activate/deactivate an object.
        /// </summary>
        /// <param name="item">The object to (de)activate.</param>
        /// <param name="setActive">True to activate, false to deactivate.</param>
        protected abstract void SetActive(T item, bool setActive);

        /// <summary>
        /// Check is the object active or inactive.
        /// </summary>
        /// <param name="item">Object to inspect.</param>
        /// <returns>True if active, false if not.</returns>
        protected abstract bool IsActive(T item);

        /// <summary>
        /// Add an object to the pool. The object is created from the set prefab.
        /// </summary>
        /// <param name="setActive">Should the object be activated by default?</param>
        /// <returns>The created object.</returns>
        private T Add(bool setActive = false)
        {
            T item = Object.Instantiate(prefab);

            SetActive(item, setActive: setActive);

            items.Add(item);

            return item;
        }
    }
}
