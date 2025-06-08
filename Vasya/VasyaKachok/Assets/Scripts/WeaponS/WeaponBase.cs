using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [SerializeField] public WeaponData weaponData;
    [SerializeField] public WeaponRarity rarity = WeaponRarity.Common;
    [SerializeField] public float cooldown = 1f;
    protected float lastUseTime;
    protected int currentDamage;

    public abstract void Use();
    public abstract void Reload();

    public float GetCooldown()
    {
        return cooldown;
    }

    public WeaponData.WeaponType GetWeaponType()
    {
        return weaponData.weaponType;
    }

    public virtual void Initialize(WeaponData data, WeaponRarity newRarity)
    {
        weaponData = data;
        rarity = newRarity;
        currentDamage = WeaponManager.CalculateStats(weaponData, rarity);
    }

    protected bool CanUse()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    protected void RegisterUseTime()
    {
        lastUseTime = Time.time;
    }
}