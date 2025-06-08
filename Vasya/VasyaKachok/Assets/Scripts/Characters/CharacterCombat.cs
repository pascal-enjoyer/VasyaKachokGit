using UnityEngine;
using UnityEngine.Events;

public class CharacterCombat : MonoBehaviour
{
    [SerializeField] private float baseDamage = 10f;
    private IWeapon currentWeapon;
    private GameObject currentTarget;
    private ComboSystem comboSystem;
    private PlayerAnimationSystem animationSystem;

    public UnityEvent<GameObject> WeaponOnTargetUsed;

    private void Start()
    {
        comboSystem = GetComponent<ComboSystem>();
        animationSystem = GetComponent<PlayerAnimationSystem>();
    }

    public void EquipWeapon(IWeapon newWeapon)
    {
        currentWeapon = newWeapon;
        if (newWeapon is WeaponBase weaponBase)
        {
            weaponBase.Initialize(weaponBase.weaponData, weaponBase.rarity);
            if (comboSystem != null)
            {
                comboSystem.Initialize(weaponBase.weaponData);
            }
        }
    }

    public void UnequipWeapon()
    {
        if (currentWeapon is IHitboxWeapon hitboxWeapon)
        {
            hitboxWeapon.DisableHitbox();
        }
        currentWeapon = null;
    }

    public void UseWeapon()
    {
        currentWeapon.Use();
    }

    public void OnAttackAnimationStart()
    {
        UseWeapon();
    }

    public void OnAttackAnimationEnd()
    {
        if (currentWeapon is IHitboxWeapon hitboxWeapon)
        {
            hitboxWeapon.DisableHitbox();
        }
        if (comboSystem != null)
        {
            comboSystem.OnAttackEnd();
        }
    }

    public void ReloadWeapon() => currentWeapon?.Reload();

    public bool HasWeapon() => currentWeapon != null;

    public IWeapon GetCurrentWeapon() => currentWeapon;

    public void SetTarget(GameObject target) => currentTarget = target;

    public GameObject GetTarget() => currentTarget;

    public void TryAttack()
    {
        if (currentTarget != null && currentTarget.activeInHierarchy)
        {
            WeaponOnTargetUsed?.Invoke(currentTarget);
        }
        if (comboSystem != null)
        {
            comboSystem.TryAttack();
        }
        else
        {
            if (animationSystem != null)
            {
                animationSystem.PlayAnimation("Default_Attack");
            }
            else
            {
                Debug.LogWarning("No animation system found for default attack.");
            }
        }
    }
}