// Weapon.cs
using UnityEngine;

public class EquipedWeapon : MonoBehaviour
{

    [Header("Settings")]
    public WeaponData weaponData;
    public WeaponRarity rarity = WeaponRarity.Common;
    private int currentDamage;

    public void Initialize(WeaponData data, WeaponRarity newRarity)
    {
        weaponData = data;
        rarity = newRarity;
        InitializeWeapon();
    }

    void InitializeWeapon()
    {
        currentDamage = WeaponManager.CalculateStats(weaponData, rarity);
    }




    public int GetDamage()
    {
        return currentDamage;
    }
}
