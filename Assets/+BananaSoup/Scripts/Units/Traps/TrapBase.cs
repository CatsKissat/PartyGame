using UnityEngine;
using BananaSoup.Modifiers;
using BananaSoup.Traps;

namespace BananaSoup.Units
{
    public class TrapBase : UnitBase
    {
        [SerializeField]
        private TrapModifierType.Modifier trapModifier;

        // Bool to track if a modifier is selected.
        private bool modifierSelected = false;

        // Variables used to store and forward speed or size if they should be
        // modified.
        private float modifiedSpeed = 0f;
        private float modifiedSize = 0f;

        // References
        private Rigidbody[] childRigidbodies = null;
        private ModifierActions modActions = null;

        public bool ModifierSelected
        {
            get => modifierSelected;
        }

        public TrapModifierType.Modifier TrapModifier
        {
            get => trapModifier;
        }

        // Property used to store the modifiedSpeed value for the trap, which is
        // used in the actual trap functionality to effect the speed of the trap.
        public float ModifiedSpeed
        {
            get => modifiedSpeed;
            set => modifiedSpeed = value;
        }

        // Property used to store the modifiedSize valuefor the trap, which is
        // used in the actual trap functionality to effect the size of the trap.
        public float ModifiedSize
        {
            get => modifiedSize;
            set => modifiedSize = value;
        }

        protected override void Start()
        {
            base.Start();

            childRigidbodies = GetComponentsInChildren<Rigidbody>();

            if ( childRigidbodies != null )
            {
                foreach ( Rigidbody rb in childRigidbodies )
                {
                    rb.useGravity = isUsingGravity;
                    rb.isKinematic = isKinematic;
                } 
            }

            modActions = GetComponent<ModifierActions>();
            if ( modActions == null )
            {
                Debug.LogError($"TrapBase on {name} couldn't find a component of type ModifierActions!");
            }

            SelectRandomModifier();
        }

        public virtual void Setup()
        {

        }

        // Debug for testing modifier randomising.
        //[ContextMenu("Randomize mod")]
        /// <summary>
        /// Method used to select a random modifier for the trap out of all the 
        /// TrapModifierType.Modifiers.
        /// Then the ModifierActions scripts SetupModifier is called from here.
        /// </summary>
        private void SelectRandomModifier()
        {
            trapModifier = (TrapModifierType.Modifier)Random.Range(0, System.Enum.GetValues(typeof(TrapModifierType.Modifier)).Length);
            modifierSelected = true;
            Debug.Log($"Current modifier for {name} is: {trapModifier}");
            modActions.SetupModifier();
        }
    }
}
