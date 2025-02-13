
using UnityEngine;

public class WeaponPickupsSpawner : MonoBehaviour
{

    public GameObject weaponPickupPrefab;
    public Vector3 posToSpawn;
    public WeaponData test;

    private void Start()
    {
        Application.targetFrameRate = 60;    
    }

    public void SpawnPickupWeapon(/*WeaponData test, Vector3 posToSpawn*/)
    {
        GameObject newPickup = Instantiate(weaponPickupPrefab, posToSpawn, Quaternion.identity, transform);
        newPickup.GetComponent<WeaponPickup>().InitializePickup(test, WeaponManager.GetRandomWeaponRarity());
    }

    public void SpawnPickupWeapon(WeaponData weaponData, WeaponRarity rarity, Vector3 posToSpawn)
    {
        GameObject newPickup = Instantiate(weaponPickupPrefab, posToSpawn, Quaternion.identity, transform);
        newPickup.GetComponent<WeaponPickup>().InitializePickup(weaponData, rarity);
    }
}
