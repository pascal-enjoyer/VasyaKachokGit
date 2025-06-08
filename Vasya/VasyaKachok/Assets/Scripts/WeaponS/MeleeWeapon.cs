using UnityEngine;
using System.Collections.Generic;

public class MeleeWeapon : WeaponBase, IHitboxWeapon
{
    [SerializeField] private Collider hitboxCollider;

    private HashSet<Collider> hitTargets;
    private bool hitboxWasDisabled;

    private void Awake()
    {
        if (hitboxCollider == null)
        {
            Debug.LogError("Hitbox Collider not assigned!", this);
        }
        else
        {
            hitboxCollider.enabled = false;
        }
        hitTargets = new HashSet<Collider>();
        hitboxWasDisabled = true;
    }

    public override void Use()
    {
        if (hitboxCollider.enabled)
        {
            Debug.LogWarning($"Hitbox was not disabled from previous attack on {gameObject.name}! Forcing disable.");
            DisableHitbox();
        }
        Debug.Log("Weapon Used");
        RegisterUseTime();
        EnableHitbox();
        hitboxWasDisabled = false;
    }

    public void EnableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitTargets.Clear();
            hitboxCollider.enabled = true;
            Debug.Log("Hitbox enabled");
        }
    }

    public void DisableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
            hitTargets.Clear();
            hitboxWasDisabled = true;
            Debug.Log("Hitbox disabled");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitboxCollider.enabled) return;

        if (!hitTargets.Contains(other))
        {
            if (other.TryGetComponent<IDamagable>(out IDamagable target))
            {
                target.TakeDamage(currentDamage);
                hitTargets.Add(other);
                Debug.Log($"Dealt {currentDamage} damage to {other.name}");
            }
        }
    }

    public override void Reload()
    {
        lastUseTime = Time.time - cooldown;
    }

    public bool WasHitboxDisabled() => hitboxWasDisabled;

}