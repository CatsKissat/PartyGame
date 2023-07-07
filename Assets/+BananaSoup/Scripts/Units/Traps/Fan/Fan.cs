using UnityEngine;

namespace BananaSoup.Traps
{
    public class Fan : TrapBase
    {
        private FanModifierActions fanModActions = null;

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
