using UnityEngine;
using System.Collections;

public class MeleeWeapon : WeaponBase
{
    [SerializeField] private Collider hitboxCollider;
    [SerializeField] private float damage;

    public override void Use()
    {
        if (!CanUse()) return;

        StartCoroutine(PerformMeleeAttack());
        RegisterUseTime();
    }

    private IEnumerator PerformMeleeAttack()
    {
        hitboxCollider.enabled = true;
        yield return new WaitForSeconds(0.3f);
        hitboxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitboxCollider.enabled) return;

        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }

    public override void Reload()
    {
        // Ближнему оружию перезарядка не нужна
    }

    public override IWeapon.WeaponType GetWeaponType()
    {
        return IWeapon.WeaponType.Melee;
    }
}
