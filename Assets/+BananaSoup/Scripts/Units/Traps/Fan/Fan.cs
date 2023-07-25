using UnityEngine;

namespace BananaSoup.Traps
{
    public class Fan : TrapBase
    {
        private FanModifierActions fanModActions = null;

        /// <summary>
        /// Method used to setup the fan trap.
        /// First get a reference to FanModifierActions, if the GameObject doesn't
        /// have the component log an error.
        /// Then check if the trap has a modified speed, if it does (ModifiedSpeed > 0)
        /// increase the pushback of the fan.
        /// After that check for modified size, if the fan has a modified size then
        /// change the scale accordingly.
        /// </summary>
        public override void Setup()
        {
            fanModActions = GetComponent<FanModifierActions>();

            if ( fanModActions == null )
            {
                Debug.LogError($"No component of type {typeof(FanModifierActions)} found on {name}!");
            }

            if ( ModifiedSpeed > 0 )
            {
                fanModActions.IncreasePushback();
            }

            if ( ModifiedSize > 0 )
            {
                Vector3 increasedScale = transform.localScale + new Vector3(ModifiedSize, ModifiedSize, ModifiedSize);
                transform.localScale = increasedScale;
            }
        }
    }
}
