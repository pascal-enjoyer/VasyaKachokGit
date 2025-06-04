using UnityEngine;
using UnityEngine.Events;

public class CharacterCombat : MonoBehaviour
{
    private IWeapon currentWeapon;
    [SerializeField] private GameObject currentTarget;
    public UnityEvent<GameObject> WeaponOnTargetUsed;
    public UnityEvent CharacterAttacked;
    [SerializeField] private ComboAttackManager comboManager;
    [SerializeField] private Animator animator;
    private float lastInputTime;
    [SerializeField] private float inputCooldown = 0.1f;
    [SerializeField] private float baseDamage = 10f;

    private void Start()
    {
        if (comboManager == null)
        {
            Debug.LogError("ComboManager not assigned!");
        }
        else
        {
            CharacterAttacked.AddListener(comboManager.TryAttack);
        }
        if (animator == null)
        {
            Debug.LogError("Animator not assigned!");
        }
    }

    public void EquipWeapon(IWeapon newWeapon)
    {
        currentWeapon = newWeapon;
        if (newWeapon is WeaponBase weaponBase)
        {
            weaponBase.SetOwner(this);
            if (comboManager != null)
            {
                comboManager.Initialize(
                    weaponBase.weaponData.comboData,
                    weaponBase.weaponData.weaponType);
            }
        }
    }

    public void UnequipWeapon() => currentWeapon = null;

    public void UseWeapon()
    {
        if (Time.time - lastInputTime < inputCooldown)
            return;

        currentWeapon.Use();
        CharacterAttacked?.Invoke();
        lastInputTime = Time.time;

        if (currentTarget != null && currentTarget.activeInHierarchy)
        {
            WeaponOnTargetUsed?.Invoke(currentTarget);
        }
    }

    public void ReloadWeapon() => currentWeapon?.Reload();

    public bool HasWeapon() => currentWeapon != null;

    public IWeapon GetCurrentWeapon() => currentWeapon;

    public void SetTarget(GameObject target) => currentTarget = target;

    public GameObject GetTarget() => currentTarget;

    public float GetLastInputTime() => lastInputTime;

    public void SetBaseDamage(float damage) => baseDamage = damage;
}