using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [SerializeField] private WeaponBase currentWeapon;

    public void UseWeapon()
    {
        if (currentWeapon != null)
            currentWeapon.Use();
    }

    public void SetWeapon(WeaponBase newWeapon)
    {
        currentWeapon = newWeapon;
        currentWeapon.SetOwner(this);
    }
}