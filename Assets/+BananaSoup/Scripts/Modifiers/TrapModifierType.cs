using UnityEngine;

namespace BananaSoup.Modifiers
{
    public class TrapModifierType : MonoBehaviour
    {
        /// <summary>
        /// The modifiers available for different traps.
        /// </summary>
        public enum Modifier
        {
            Basic,
            Freeze,
            Electric,
            Speed,
            Size
        }
    }
}
