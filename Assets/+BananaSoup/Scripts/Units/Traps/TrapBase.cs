using UnityEngine;
using BananaSoup.Modifiers;

namespace BananaSoup.Units
{
    public class TrapBase : UnitBase
    {
        [SerializeField]
        private ModifierType.Modifier currentModifier;
    }
}
