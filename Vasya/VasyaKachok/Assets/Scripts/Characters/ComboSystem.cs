using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    [SerializeField] private float comboResetDelay = 0.2f;
    [SerializeField] private float inputWindowDuration = 0.3f;

    private CharacterCombat characterCombat;
    private PlayerAnimationSystem animationSystem;
    private WeaponData currentWeaponData;
    private int currentComboIndex = 0;
    private bool isAttacking = false;
    private bool inputWindowOpen = false;
    private float lastAttackTime;
    private float inputWindowStartTime;
    private string[] currentComboClips;
    private float attackStartTime;

    private void Awake()
    {
        characterCombat = GetComponent<CharacterCombat>();
        animationSystem = GetComponent<PlayerAnimationSystem>();
        if (characterCombat == null || animationSystem == null)
        {
            enabled = false;
        }
    }

    public void Initialize(WeaponData weaponData)
    {
        if (weaponData == null || weaponData.comboData == null || weaponData.comboData.comboAnimationClips == null)
        {
            currentComboClips = new[] { "Default_Attack" };
            animationSystem.InitializeAnimations(new AnimationClip[0], currentComboClips);
            ResetCombo();
            return;
        }

        currentWeaponData = weaponData;
        currentComboClips = new string[weaponData.comboData.comboAnimationClips.Length];
        for (int i = 0; i < weaponData.comboData.comboAnimationClips.Length; i++)
        {
            currentComboClips[i] = $"Attack_{(i + 1)}";
        }
        animationSystem.InitializeAnimations(weaponData.comboData.comboAnimationClips, currentComboClips);
        ResetCombo();
    }

    public void TryAttack()
    {
        if (!isAttacking)
        {
            PlayCombo();
        }
        else if (inputWindowOpen && currentComboIndex < currentComboClips.Length)
        {
            PlayCombo();
        }
    }

    private void Update()
    {
        if (!isAttacking)
        {
            if (currentComboIndex > 0 && Time.time - lastAttackTime > comboResetDelay)
            {
                bool isAttackPlaying = false;
                foreach (var clip in currentComboClips)
                {
                    if (animationSystem.IsPlayingAnimation(clip))
                    {
                        isAttackPlaying = true;
                        break;
                    }
                }
                if (isAttackPlaying)
                {
                    ResetCombo();
                }
            }
            return;
        }

        string currentAttackClip = currentComboClips[currentComboIndex - 1];
        AnimatorStateInfo state = animationSystem.GetAnimator().GetCurrentAnimatorStateInfo(0);
        bool isCurrentAttackPlaying = state.IsName(currentAttackClip);
        float normalizedTime = state.normalizedTime;

        if (inputWindowOpen && Time.time >= inputWindowStartTime + inputWindowDuration)
        {
            inputWindowOpen = false;
        }

        if (Time.time - attackStartTime > 0.1f && (!isCurrentAttackPlaying || normalizedTime >= 1f))
        {
            isAttacking = false;
            ResetCombo();
        }
    }

    private void PlayCombo()
    {
        if (currentComboIndex >= currentComboClips.Length)
        {
            ResetCombo();
            return;
        }

        GameObject target = characterCombat.GetTarget();
        if (target != null && target.activeInHierarchy)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        if (animationSystem.GetAnimator().HasState(0, Animator.StringToHash(currentComboClips[currentComboIndex])))
        {
            animationSystem.PlayAnimation(currentComboClips[currentComboIndex]);
            isAttacking = true;
            inputWindowOpen = false;
            lastAttackTime = Time.time;
            attackStartTime = Time.time;
            currentComboIndex++;
        }
        else
        {
            ResetCombo();
        }
    }

    public void OnAttackEnd()
    {
        inputWindowOpen = true;
        inputWindowStartTime = Time.time;
    }

    public void ResetCombo()
    {
        currentComboIndex = 0;
        isAttacking = false;
        inputWindowOpen = false;
        animationSystem.PlayIdle();
    }
}