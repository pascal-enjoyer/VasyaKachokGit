// Weapon.cs
using UnityEngine;

public class EquipedWeapon : MonoBehaviour
{

    [Header("Settings")]
    public WeaponData weaponData;
    public WeaponRarity rarity = WeaponRarity.Common;

    [Header("References")]
    public Light glowLight;

    private int currentDamage;

    void Awake()
    {
        InitializeWeapon();
    }

    public void Initialize(WeaponData data, WeaponRarity newRarity)
    {
        weaponData = data;
        rarity = newRarity;
        InitializeWeapon();
    }

    void InitializeWeapon()
    {

        currentDamage = WeaponManager.CalculateStats(weaponData, rarity);
        ApplyVisuals();
    }



    void ApplyVisuals()
    {
        // Настройка цвета свечения
        glowLight.color = WeaponManager.GetRarityColor(rarity);

        
    }


    public int GetDamage()
    {
        return currentDamage;
    }
}
