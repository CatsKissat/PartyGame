using System.Collections;
using UnityEngine;
using BananaSoup.Managers;
using BananaSoup.Weapons;

namespace BananaSoup.Blocks
{
    public class WeaponPedestal : BlockBase
    {
        [Space]

        [SerializeField, Tooltip("The weapon the pedestal should spawn.")]
        private WeaponBase weaponPrefab = null;
        [SerializeField, Tooltip("The WeaponPlacement child object where the weapon should be placed.")]
        private Transform weaponPlacement = null;

        [Space]

        [SerializeField, Tooltip("The time it takes for a new weapon to spawn on the pedestal.")]
        private float newWeaponSpawnTime = 15.0f;

        private Vector3 weaponPlacementVector = Vector3.zero;

        private Coroutine spawnNewWeaponRoutine = null;

        private WeaponBase spawnedWeapon = null;

        private GameManager gameManager = null;

        private void OnDisable()
        {
            TryStopAndNullRoutine(ref spawnNewWeaponRoutine);
            gameManager.NewRound -= Setup;
        }

        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Method called from GameManager to add a listener to the GameManager's
        /// NewRound event.
        /// </summary>
        public void AddListenerToNewRoundEvent()
        {
            gameManager = FindObjectOfType<GameManager>();
            if ( gameManager == null )
            {
                Debug.LogError($"{name} couldn't find an object of type {typeof(GameManager)}!");
            }

            gameManager.NewRound += Setup;
        }

        /// <summary>
        /// Method used to setup the pedestal on load. Get the weaponPlacement position
        /// and spawn a weapon.
        /// </summary>
        private void Setup()
        {
            weaponPlacementVector = weaponPlacement.position;
            SpawnWeapon(weaponPrefab);
        }

        /// <summary>
        /// Method used to spawn a weapon.
        /// First instantiate a new weapon with a reference to it in spawnedWeapon.
        /// Then listen to the weapons WeaponPickedUp event.
        /// Then toggle the weapons onPedestal bool to be true.
        /// </summary>
        /// <param name="weapon">The weapon prefab to spawn.</param>
        private void SpawnWeapon(WeaponBase weapon)
        {
            if ( weapon != null && spawnedWeapon == null )
            {
                spawnedWeapon = Instantiate(weapon, weaponPlacementVector, Quaternion.identity);
                spawnedWeapon.WeaponPickedUp += WeaponPickedUp;
                spawnedWeapon.onPedestal = true;
            }
        }

        /// <summary>
        /// Coroutine used to call SpawnWeapon after a set period of time (in seconds.
        /// </summary>
        /// <param name="weapon">The weapon prefab to spawn.</param>
        private IEnumerator SpawnNewWeapon(WeaponBase weapon)
        {
            yield return new WaitForSeconds(newWeaponSpawnTime);

            SpawnWeapon(weapon);

            spawnNewWeaponRoutine = null;
        }

        /// <summary>
        /// Method called when a weapon is picked up.
        /// Used to stop listening to the current weapons WeaponPickedUp event,
        /// to null current spawnedWeapon and to start the SpawnNewWeapon coroutine.
        /// </summary>
        private void WeaponPickedUp()
        {
            spawnedWeapon.WeaponPickedUp -= WeaponPickedUp;
            spawnedWeapon = null;

            if ( spawnNewWeaponRoutine == null )
            {
                spawnNewWeaponRoutine = StartCoroutine(SpawnNewWeapon(weaponPrefab));
            }
        }

        private void TryStopAndNullRoutine(ref Coroutine routine)
        {
            if ( routine != null )
            {
                StopCoroutine(routine);
                routine = null;
            }
        }
    }
}
