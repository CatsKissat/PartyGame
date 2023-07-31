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
            TryStopAndNullRoutine(spawnNewWeaponRoutine);
            gameManager.NewRound -= Setup;
        }

        protected override void Start()
        {
            base.Start();

            gameManager = FindObjectOfType<GameManager>();
            if ( gameManager == null )
            {
                Debug.LogError($"{name} couldn't find an object of type {typeof(GameManager)}!");
            }

            gameManager.NewRound += Setup;
        }

        private void Setup()
        {
            weaponPlacementVector = weaponPlacement.position;
            SpawnWeapon(weaponPrefab);
        }

        private void SpawnWeapon(WeaponBase weapon)
        {
            if ( weapon != null && spawnedWeapon == null )
            {
                spawnedWeapon = Instantiate(weapon, weaponPlacementVector, Quaternion.identity);
                spawnedWeapon.WeaponPickedUp += WeaponPickedUp;
                spawnedWeapon.onPedestal = true;
            }
        }

        private IEnumerator SpawnNewWeapon(WeaponBase weapon)
        {
            yield return new WaitForSeconds(newWeaponSpawnTime);

            SpawnWeapon(weapon);

            spawnNewWeaponRoutine = null;
        }

        private void WeaponPickedUp()
        {
            spawnedWeapon.WeaponPickedUp -= WeaponPickedUp;
            spawnedWeapon = null;
            spawnNewWeaponRoutine = StartCoroutine(SpawnNewWeapon(weaponPrefab));
        }

        private void TryStopAndNullRoutine(Coroutine routine)
        {
            if ( routine != null )
            {
                StopCoroutine(routine);
                routine = null;
            }
        }
    }
}
