using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [Header("Settings")]
    public WeaponData weaponData;
    public WeaponRarity rarity = WeaponRarity.Common;

    [SerializeField] protected float cooldown = 1f;
    protected float lastUseTime;

    [SerializeField] protected int currentDamage;

    protected CharacterCombat owner;

    public abstract void Use(); // вызывается из CharacterCombat

    public virtual void SetOwner(CharacterCombat combat)
    {
        owner = combat;
    }

    public abstract void Reload();

    public virtual float GetCooldown()
    {
        return cooldown;
    }

    public bool CanUse()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    protected void RegisterUseTime()
    {
        lastUseTime = Time.time;
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
    }

    public WeaponData.WeaponType GetWeaponType()
    {
        return weaponData.weaponType;
    }
}
