using UnityEngine;

public interface IWeapon
{
    public enum WeaponType
    {
        Melee,
        Range
    }

    void Use();
    void Reload();
    float GetCooldown();
}