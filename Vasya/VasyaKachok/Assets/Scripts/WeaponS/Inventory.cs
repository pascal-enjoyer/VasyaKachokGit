// Inventory.cs

using UnityEngine;


public enum WeaponRarity { Common, Rare, Legendary }

public class Inventory : MonoBehaviour
{

    [System.Serializable]

    public class WeaponInstance
    {
        public WeaponData data;
        public WeaponRarity rarity;
    }

    public GameObject currentWeapon;
    public Transform weaponParent;
    
    public WeaponPickupsSpawner pickupSpawner;


    public void AddWeapon(WeaponData newData, WeaponRarity newRarity)
    {
        EquipWeapon(newData, newRarity);
    }

    public void EquipWeapon(WeaponData data, WeaponRarity rarity)
    {
        // ”дал€ем текущее оружие
        foreach (Transform child in weaponParent)
        {
            if (child.TryGetComponent<EquipedWeapon>(out EquipedWeapon eqWeapon))
            {
                pickupSpawner.SpawnPickupWeapon(eqWeapon.weaponData, eqWeapon.rarity, transform.position + Vector3.forward);
            }
            Destroy(child.gameObject);
        }

        // —оздаем новый экземпл€р
        GameObject newWeapon = Instantiate(data.weaponInHandPrefab, weaponParent);
        EquipedWeapon weaponComponent = newWeapon.GetComponent<EquipedWeapon>();
        weaponComponent.Initialize(data, rarity);
    }
}
