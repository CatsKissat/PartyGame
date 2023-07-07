using System.Collections;
using UnityEngine;

namespace BananaSoup.Traps
{
    public class LaserBeam : TrapBase
    {
        [Space]
        // References to traps different parts.
        [SerializeField]
        private GameObject leftBeamer;
        [SerializeField]
        private GameObject rightBeamer;
        [SerializeField]
        private GameObject beam;

        [Space]

        [Header("Trap variables")]
        [SerializeField, Tooltip("Used to determine the default duration of the laser beam.")]
        private float beamDuration = 1.5f;
        [SerializeField, Tooltip("Used to determine the default cooldown of the laser beam.")]
        private float beamCooldown = 1.0f;

        // Bool to track if the trap is on cooldown
        private bool onCooldown = false;

        // Float to store the distance between the left and right beamers, which
        // is used to calculate the length of the actual beam.
        private float distanceBetweenBeamers = 0.0f;

        // Coroutine to store the routine where the beams functionality happens
        private Coroutine beamRoutine = null;

        private void OnDisable()
        {
            TryStopAndNullCoroutine(beamRoutine);
        }

        /// <summary>
        /// Setup where the beam is set to be off at the beginning and then checks
        /// for modified speed or size and after that the beams scale is set.
        /// If the beamRoutine is null, then start BeamRoutine in it.
        /// </summary>
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
                SetBeamerPositions(ModifiedSize);
            }

            SetBeamScale();

            if ( beamRoutine == null )
            {
                beamRoutine = StartCoroutine(BeamRoutine());
            }
        }

        /// <summary>
        /// Method to set the laser beams beamers positions if the size is modified.
        /// </summary>
        /// <param name="offset"></param>
        private void SetBeamerPositions(float offset)
        {
            Vector3 leftBeamerOffset = leftBeamer.transform.position;
            leftBeamerOffset.x -= offset;
            leftBeamer.transform.position = leftBeamerOffset;

            Vector3 rightBeamerOffset = rightBeamer.transform.position;
            rightBeamerOffset.x += offset;
            rightBeamer.transform.position = rightBeamerOffset;
        }

        /// <summary>
        /// Method to set the actual beams scale to match the distance between the beamers.
        /// </summary>
        private void SetBeamScale()
        {
            distanceBetweenBeamers = Vector3.Distance(leftBeamer.transform.position, rightBeamer.transform.position);
            Vector3 beamScale = new Vector3(distanceBetweenBeamers, 0.02f, 1.0f);
            beam.transform.localScale = beamScale;
        }

        /// <summary>
        /// Coroutine where the beam is set active for beamDuration and then inactive
        /// for the duration of the beamCooldown.
        /// </summary>
        /// <returns></returns>
        private IEnumerator BeamRoutine()
        {
            while ( true )
            {
                if ( onCooldown )
                {
                    yield return new WaitForSeconds(beamCooldown);
                    onCooldown = false;
                }
                else if ( !onCooldown )
                {
                    beam.SetActive(true);
                    yield return new WaitForSeconds(beamDuration);
                    onCooldown = true;
                    beam.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Method used to try to stop and null the coroutine if the given parameter
        /// Coroutine is not null. (Used in OnDisable)
        /// </summary>
        /// <param name="routine">The Coroutine you want to try to stop and null.</param>
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
