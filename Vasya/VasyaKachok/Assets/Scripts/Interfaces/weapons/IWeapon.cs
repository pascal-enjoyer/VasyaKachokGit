using UnityEngine;

public interface IWeapon 
{ 
    WeaponData.WeaponType GetWeaponType(); 
    void Use(); 
    void Reload(); 
    float GetCooldown(); 
}