using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    [SerializeField] private Collider hitboxCollider;
    

    public override void Use()
    {
        if (!CanUse())
        {
            //Debug.LogWarning("Weapon on cooldown!");
            return;
        }
        RegisterUseTime();
        //Debug.Log("MeleeWeapon Use called");
    }

    public void EnableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = true;
            //Debug.Log("Hitbox enabled");
        }
    }

    public void DisableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
            //Debug.Log("Hitbox disabled");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("nihuya udar nachalsya");
        if (!hitboxCollider.enabled) return;

        Debug.Log("nihuya udar zashitalsya pochti");
        if (other.TryGetComponent<IDamagable>(out IDamagable target))
        {
            target.TakeDamage(currentDamage);
            Debug.Log($"Dealt {currentDamage} damage to {other.name}");
        }
    }

    public override void Reload()
    {
        lastUseTime = Time.time - cooldown;
    }
}