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

    private CharacterCombat characterCombat;

    private void Awake()
    {
        characterCombat = GetComponent<CharacterCombat>();
        if (characterCombat == null)
        {
            Debug.LogError("CharacterCombat не найден на объекте с Inventory!");
        }
    }

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

        // Создаём экземпляр оружия
        currentWeapon = Instantiate(data.weaponInHandPrefab, weaponParent);
        if (!currentWeapon.TryGetComponent<WeaponBase>(out WeaponBase weaponComponent))
        {
            Debug.LogError("Prefab оружия должен содержать WeaponBase!");
            return;
        }

        weaponComponent.Initialize(data, rarity);

        // Связать с CharacterCombat
        if (characterCombat != null)
        {
            characterCombat.EquipWeapon(weaponComponent); // передаём как IWeapon
        }

        WeaponEquiped?.Invoke(true);
    }

    public void DropWeapon()
    {
        foreach (Transform child in weaponParent)
        {
            if (child.TryGetComponent<WeaponBase>(out WeaponBase eqWeapon))
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

        if (characterCombat != null)
        {
            characterCombat.UnequipWeapon();
        }
    }
}
