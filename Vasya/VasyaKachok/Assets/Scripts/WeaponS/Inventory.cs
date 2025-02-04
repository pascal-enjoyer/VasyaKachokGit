// Inventory.cs
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<WeaponData> weapons = new List<WeaponData>();
    public Transform weaponParent;

    public void AddWeapon(WeaponData newWeapon)
    {
        if (!weapons.Contains(newWeapon))
        {
            weapons.Add(newWeapon);
            EquipWeapon(newWeapon);
        }
    }

    public void EquipWeapon(WeaponData weaponToEquip)
    {
        ClearWeapon();

        GameObject weaponInstance = Instantiate(
            weaponToEquip.weaponPrefab,
            weaponParent.position,
            weaponParent.rotation,
            weaponParent
        );

        weaponInstance.GetComponent<Weapon>().Initialize(weaponToEquip);
    }

    void ClearWeapon()
    {
        foreach (Transform child in weaponParent)
        {
            Destroy(child.gameObject);
        }
    }
}