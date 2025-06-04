using System.Collections.Generic;
using UnityEngine;

public class ComboAttackManager : MonoBehaviour
{
    [SerializeField] private string[] comboStates = new[] { "1melee", "2melee", "3melee" };
    [SerializeField] private float comboInputThreshold = 0.85f;
    [SerializeField] private float comboResetDelay = 0.2f;

    public PlayerAnimationManager playerAnimationManager;
    public CharacterCombat characterCombat;
    private int currentComboIndex = 0;
    private bool isAttacking = false;
    private bool nextAttackQueued = false;
    private float lastAttackTime;

    private static readonly Dictionary<WeaponData.WeaponType, string[]> defaultCombos = new()
    {
        { WeaponData.WeaponType.Melee, new[] { "1melee", "2melee", "3melee" } },
        { WeaponData.WeaponType.Range, new[] { "Range1", "Range2", "Range3" } }
    };

    private void Start()
    {
        if (playerAnimationManager == null || playerAnimationManager.animator == null)
        {
            Debug.LogError("PlayerAnimationManager or Animator not assigned!");
            return;
        }
        if (characterCombat == null)
        {
            Debug.LogError("CharacterCombat not assigned!");
            return;
        }
        ResetCombo();
    }

    public void Initialize(ComboData comboData, WeaponData.WeaponType weaponType)
    {
        if (comboData != null && comboData.comboAnimationStates != null && comboData.comboAnimationStates.Length > 0)
        {
            comboStates = comboData.comboAnimationStates;
        }
        else if (defaultCombos.TryGetValue(weaponType, out var defaults))
        {
            comboStates = defaults;
            Debug.LogWarning($"Using default combo: {string.Join(", ", defaults)}");
        }
        else
        {
            comboStates = new[] { "Default_Attack" };
            Debug.LogWarning($"No default combo for {weaponType}. Using fallback: Default_Attack");
        }
        ResetCombo();
    }

    public void TryAttack()
    {
        if (playerAnimationManager == null || comboStates == null || comboStates.Length == 0)
        {
            Debug.LogError("PlayerAnimationManager or comboStates not set!");
            return;
        }

        if (!isAttacking)
        {
            currentComboIndex = 0;
            PlayCombo();
        }
        else
        {
            AnimatorStateInfo state = playerAnimationManager.animator.GetCurrentAnimatorStateInfo(0);
            if (currentComboIndex < comboStates.Length && state.normalizedTime < comboInputThreshold && IsCurrentComboAnimation())
            {
                if (currentComboIndex < comboStates.Length - 1)
                {
                    PlayCombo();
                }
                else
                {
                    nextAttackQueued = true;
                }
            }
        }
    }

    private void Update()
    {
        if (!isAttacking) return;

        AnimatorStateInfo state = playerAnimationManager.animator.GetCurrentAnimatorStateInfo(0);

        if (IsCurrentComboAnimation())
        {
            if (state.normalizedTime >= 1.0f)
            {
                isAttacking = false;
                if (nextAttackQueued && currentComboIndex < comboStates.Length)
                {
                    PlayCombo();
                }
                else if (Time.time - lastAttackTime >= comboResetDelay)
                {
                    ResetCombo();
                }
            }
        }
        else if (!state.IsName("FromIdleToRun"))
        {
            //Debug.LogWarning($"Unexpected animation state: {state.fullPathHash}. Resetting.");
            ResetCombo();
        }
    }

    private void PlayCombo()
    {
        if (currentComboIndex >= comboStates.Length)
        {
            ResetCombo();
            return;
        }

        // Поворот к цели
        GameObject target = characterCombat.GetTarget();
        if (target != null && target.activeInHierarchy)
        {
            Vector3 targetPosition = target.transform.position;
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }


        string animName = comboStates[currentComboIndex];
        playerAnimationManager.ChangeAnimation(animName);
        isAttacking = true;
        nextAttackQueued = false;
        lastAttackTime = Time.time;
        currentComboIndex++;
    }

    private bool IsCurrentComboAnimation()
    {
        AnimatorStateInfo state = playerAnimationManager.animator.GetCurrentAnimatorStateInfo(0);
        for (int i = 0; i < comboStates.Length; i++)
        {
            if (state.IsName(comboStates[i]))
                return true;
        }
        return false;
    }

    public void ResetCombo()
    {
        currentComboIndex = 0;
        isAttacking = false;
        nextAttackQueued = false;
        playerAnimationManager.ChangeAnimation("FromIdleToRun");
        playerAnimationManager.animator.Play("FromIdleToRun", 0, 0f);
    }
}