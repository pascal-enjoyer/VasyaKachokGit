// Inventory.cs

using UnityEngine;
using UnityEngine.Events;


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

    public UnityEvent<bool> WeaponEquiped;

    public void AddWeapon(WeaponData newData, WeaponRarity newRarity)
    {
        EquipWeapon(newData, newRarity);
    }

    public void EquipWeapon(WeaponData data, WeaponRarity rarity)
    {
        if (currentWeapon != null)
        {
            DropWeapon();
        }

        // —оздаем новый экземпл€р
        currentWeapon = Instantiate(data.weaponInHandPrefab, weaponParent);
        EquipedWeapon weaponComponent = currentWeapon.GetComponent<EquipedWeapon>();
        weaponComponent.Initialize(data, rarity);
        WeaponEquiped?.Invoke(true);

    }

    public void DropWeapon()
    {
        // ”дал€ем текущее оружие
        foreach (Transform child in weaponParent)
        {
            if (child.TryGetComponent<EquipedWeapon>(out EquipedWeapon eqWeapon))
            {
                pickupSpawner.SpawnPickupWeapon(eqWeapon.weaponData, eqWeapon.rarity, 
                                                transform.position + new Vector3(Random.Range(-2f, 2f),
                                                                                 0.5f,
                                                                                 Random.Range(-2f, 2f)));
            }
            Destroy(child.gameObject);
        }
        WeaponEquiped?.Invoke(false);
        currentWeapon = null;

    }
}
