using UnityEngine;

public class PlayerAnimationSystem : AnimationSystem
{
    private CharacterCombat characterCombat;
    private string currentAnimation = "";

    protected override void Awake()
    {
        base.Awake();
        characterCombat = GetComponent<CharacterCombat>();
        if (characterCombat == null)
        {
            //Debug.LogError("PlayerAnimationSystem: Компонент CharacterCombat не найден!", this);
            enabled = false;
            return;
        }
        //Debug.Log($"Текущее состояние анимации при старте: {GetCurrentAnimationName()}");
    }

    public override void InitializeAnimations(AnimationClip[] clips, string[] clipNames)
    {
        base.InitializeAnimations(clips, clipNames);
        currentAnimation = "";
        //Debug.Log("Анимации игрока инициализированы");
    }

    public void ChangeRunSpeed(float speed)
    {
        animator.SetFloat("MovementSpeed", speed);
        //Debug.Log($"Скорость бега изменена: {speed}, текущее состояние: {GetCurrentAnimationName()}");
    }

    public override void PlayAnimation(string animName)
    {
        if (string.IsNullOrEmpty(animName) || animator == null || !animator.isActiveAndEnabled)
        {
            //Debug.LogWarning($"Не удалось сменить анимацию: animName={animName}, animator={animator}", this);
            return;
        }

        int stateID = Animator.StringToHash(animName);
        if (!animator.HasState(0, stateID))
        {
            //Debug.LogWarning($"Состояние {animName} не найдено в контроллере!", this);
            return;
        }

        animator.Play(stateID);
        animator.Update(0f);
        currentAnimation = animName;
        //Debug.Log($"Анимация изменена на: {animName}, текущее состояние: {GetCurrentAnimationName()}");
    }

    public string GetCurrentAnimationName()
    {
        if (animator == null || !animator.isActiveAndEnabled)
        {
            //Debug.LogWarning("Аниматор не активен!", this);
            return currentAnimation;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (state.IsName(clip.name))
            {
                currentAnimation = clip.name;
                //Debug.Log($"Текущее состояние анимации: {currentAnimation}, stateHash={state.shortNameHash}");
                return currentAnimation;
            }
        }

        //Debug.LogWarning($"Неизвестное состояние анимации: stateHash={state.shortNameHash}, CurrentAnimation={currentAnimation}");
        return currentAnimation;
    }

    public void ForceAnimationUpdate()
    {
        if (animator == null || !animator.isActiveAndEnabled) return;

        string animName = GetCurrentAnimationName();
        animator.Play(animName, 0, 0f);
        animator.Update(0f);
        //Debug.Log($"Принудительное обновление анимации: {animName}");
    }

    public override bool IsPlayingAnimation(string stateName)
    {
        if (string.IsNullOrEmpty(stateName) || animator == null || !animator.isActiveAndEnabled)
        {
            Debug.LogWarning($"Неверное имя анимации или аниматор не активен: {stateName}!", this);
            return false;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlaying = state.IsName(stateName);
        Debug.Log($"Проверка анимации {stateName}: {isPlaying}, текущее состояние: {GetCurrentAnimationName()}");
        return isPlaying;
    }
}