using UnityEngine;

public class RangedWeapon : WeaponBase
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField] private int maxAmmo = 10;
    private int currentAmmo;

    private void Awake()
    {
        currentAmmo = maxAmmo;
    }

    public override void Use()
    {
        if (!CanUse() || currentAmmo <= 0) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.SetOwner(owner.gameObject);

        currentAmmo--;
        RegisterUseTime();
    }

    public override void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log("Перезарядка завершена.");
    }

    public override IWeapon.WeaponType GetWeaponType()
    {
        return IWeapon.WeaponType.Range;
    }
}
