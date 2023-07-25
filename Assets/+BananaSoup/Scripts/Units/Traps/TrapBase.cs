using UnityEngine;
using NaughtyAttributes;
using BananaSoup.Modifiers;
using BananaSoup.Units;

namespace BananaSoup.Traps
{
    public class TrapBase : UnitBase
    {
        [SerializeField]
        protected bool selectRandomMod = false;

        [SerializeField, HideIf(nameof(selectRandomMod))]
        protected TrapModifierType.Modifier trapModifier;

        [SerializeField]
        protected LayerMask playersLayerMask;


        // Variable used to store the player ID of the player who places the trap.
        //private int placerID = -1;

        // Variables used to store and forward speed or size if they should be
        // modified.
        protected float modifiedSpeed = 0f;
        protected float modifiedSize = 0f;

        // References
        private Rigidbody[] childRigidbodies = null;
        protected ModifierActions modActions = null;

        public TrapModifierType.Modifier TrapModifier => trapModifier;

        public LayerMask PlayersLayerMask => playersLayerMask;

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

        /// <summary>
        /// First call the inherited UnitBase Start with base.Start().
        /// Then get the Rigidbodies of any child GameObject's into an array.
        /// If childRigidbodies isn't null set the useGravity and isKinematic values
        /// of the child Rigidbodies corresponding with the serialized variables.
        /// Then get a reference to the ModifierActions component, if it can't be found
        /// log an error.
        /// Then check if the trap should select a random mod if not then just setup the
        /// currently selected serialize modifier.
        /// </summary>
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

            if ( selectRandomMod )
            {
                SelectRandomModifier();
            }
            else
            {
                modActions.SetupModifier();
            }
        }

        public virtual void Setup()
        {

        }

        /// <summary>
        /// Method used to select a random modifier for the trap out of all the 
        /// TrapModifierType.Modifiers.
        /// Then the ModifierActions scripts SetupModifier is called from here.
        /// </summary>
        private void SelectRandomModifier()
        {
            trapModifier = (TrapModifierType.Modifier)Random.Range(0, System.Enum.GetValues(typeof(TrapModifierType.Modifier)).Length);
            Debug.Log($"Current modifier for {name} is: {trapModifier}");
            modActions.SetupModifier();
        }

        //public void StorePlayerID(int ID)
        //{
        //    if ( placerID < 0 )
        //    {
        //        placerID = ID;
        //    }
        //}
    }
}
