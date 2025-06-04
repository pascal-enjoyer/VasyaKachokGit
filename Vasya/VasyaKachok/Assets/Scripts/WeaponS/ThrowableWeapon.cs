using UnityEngine;

public class ThrowableWeapon : WeaponBase
{
    [SerializeField] private GameObject throwablePrefab;
    [SerializeField] private Transform throwPoint;

    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float upwardForce = 2f;

    [SerializeField] private int maxAmmo = 3;
    private int currentAmmo;

    private void Awake()
    {
        currentAmmo = maxAmmo;
    }

    public override void Use()
    {
        if (!CanUse() || currentAmmo <= 0)
            return;

        GameObject obj = Instantiate(throwablePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 forceDir = throwPoint.forward * throwForce + throwPoint.up * upwardForce;
            rb.AddForce(forceDir, ForceMode.VelocityChange);
        }

        // Можно установить владельца, если у throwable объекта есть логика урона
        Projectile projectile = obj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetOwner(owner.gameObject);
        }

        currentAmmo--;
        RegisterUseTime();
    }

    public override void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log("Метательное оружие перезаряжено.");
    }

    public override float GetCooldown()
    {
        return cooldown;
    }

}
