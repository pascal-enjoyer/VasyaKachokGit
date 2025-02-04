// Weapon.cs
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private ParticleSystem hitEffect;

    public void Initialize(WeaponData data)
    {
        weaponData = data;
        ApplyVisuals();
    }

    // � Weapon.cs
    void ApplyVisuals()
    {
        if (weaponData.rarity == WeaponData.Rarity.Legendary)
        {
            var particles = GetComponentInChildren<ParticleSystem>();
            if (particles != null)
            {
                var main = particles.main;
                main.startColor = weaponData.glowColor;
            }
        }
    }

    public void Attack()
    {
        // ������ ����� � �������������� weaponData.CalculatedDamage
    }
}