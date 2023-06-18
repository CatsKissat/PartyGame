using UnityEngine;

namespace BananaSoup.Utils
{
    public class ComponentPool<TComponent> : Pool<TComponent>
        where TComponent : Component
    {
        public ComponentPool(TComponent prefab, int sizeOfPool)
            : base(prefab, sizeOfPool)
        {
        }

        protected override bool IsActive(TComponent item)
        {
            return item.gameObject.activeSelf;
        }

        protected override void SetActive(TComponent item, bool setActive)
        {
            item.gameObject.SetActive(setActive);
        }
    }
}
