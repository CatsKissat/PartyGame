using BananaSoup.Units;
using System.Collections;
using UnityEngine;

namespace BananaSoup.Traps
{
    public class LaserBeamTrap : TrapBase
    {
        [Space]
        [SerializeField]
        private GameObject leftBeamer;
        [SerializeField]
        private GameObject rightBeamer;
        [SerializeField]
        private GameObject beam;

        [Space]

        [SerializeField]
        private float beamDuration = 1.5f;
        [SerializeField]
        private float beamCooldown = 1.0f;

        private bool onCooldown = false;

        private float distanceBetweenBeamers = 0.0f;

        private Coroutine beamRoutine = null;

        private void OnDisable()
        {
            TryStopAndNullCoroutine(beamRoutine);
        }

        public override void Setup()
        {
            beam.SetActive(false);

            if ( ModifiedSpeed > 0 )
            {
                beamDuration -= ModifiedSpeed;
                beamCooldown -= ModifiedSpeed;
            }
            
            if ( ModifiedSize > 0 ) 
            {
                SetBeamSize(ModifiedSize);
            }

            SetBeamScale();

            if ( beamRoutine == null )
            {
                beamRoutine = StartCoroutine(BeamRoutine());
            }
        }

        private void SetBeamSize(float offset)
        {
            Vector3 leftBeamerOffset = leftBeamer.transform.position;
            leftBeamerOffset.x -= offset;
            leftBeamer.transform.position = leftBeamerOffset;

            Vector3 rightBeamerOffset = rightBeamer.transform.position;
            rightBeamerOffset.x += offset;
            rightBeamer.transform.position = rightBeamerOffset;
        }

        private void SetBeamScale()
        {
            distanceBetweenBeamers = Vector3.Distance(leftBeamer.transform.position, rightBeamer.transform.position);
            Vector3 beamScale = new Vector3(distanceBetweenBeamers, 0.02f, 1.0f);
            beam.transform.localScale = beamScale;
        }

        private IEnumerator BeamRoutine()
        {
            while ( true )
            {
                if ( !onCooldown )
                {
                    beam.SetActive(true);
                    yield return new WaitForSeconds(beamDuration);
                    onCooldown = true;
                    beam.SetActive(false);
                }
                else
                {
                    yield return new WaitForSeconds(beamCooldown);
                    onCooldown = false;
                }
            }
        }

        private void TryStopAndNullCoroutine(Coroutine routine)
        {
            if ( routine != null )
            {
                StopCoroutine(routine);
                routine = null;
            }
        }
    }
}
