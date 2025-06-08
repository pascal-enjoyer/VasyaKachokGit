using UnityEngine;
using UnityEngine.Animations;
using System.Collections.Generic;

public abstract class AnimationSystem : MonoBehaviour
{
    [SerializeField] protected Animator animator; protected RuntimeAnimatorController baseController; protected AnimatorOverrideController overrideController;

    protected virtual void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
               // Debug.LogError($"{GetType().Name}: Аниматор не найден на объекте! Добавьте компонент Animator.", this);
                enabled = false;
                return;
            }
        }

        if (animator.runtimeAnimatorController == null)
        {
            //Debug.LogError($"{GetType().Name}: У аниматора отсутствует RuntimeAnimatorController! Назначьте AnimatorController в инспекторе.", this);
            enabled = false;
            return;
        }

        baseController = animator.runtimeAnimatorController;
        //Debug.Log($"{GetType().Name}: Сохранён базовый контроллер: {baseController.name} (Клипов: {baseController.animationClips.Length})");

        try
        {
            overrideController = new AnimatorOverrideController(baseController);
            if (overrideController == null)
            {
                //Debug.LogError($"{GetType().Name}: Не удалось создать AnimatorOverrideController!", this);
                enabled = false;
                return;
            }
            animator.runtimeAnimatorController = overrideController;
            //Debug.Log($"{GetType().Name}: AnimatorOverrideController успешно создан и назначен: {overrideController.name}");
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"{GetType().Name}: Ошибка при создании AnimatorOverrideController: {e.Message}. Используется базовый контроллер.", this);
            animator.runtimeAnimatorController = baseController;
            enabled = false;
            return;
        }
    }

    public virtual void InitializeAnimations(AnimationClip[] clips, string[] clipNames)
    {
        if (clips == null || clips.Length == 0 || clipNames == null || clipNames.Length == 0 || clips.Length != clipNames.Length)
        {
            //Debug.LogWarning($"{GetType().Name}: Клипы анимации или имена не предоставлены или не совпадают по длине. Используется текущий контроллер.", this);
            return;
        }

        if (overrideController == null)
        {
            //Debug.LogError($"{GetType().Name}: AnimatorOverrideController не инициализирован! Восстанавливаем.", this);
            try
            {
                overrideController = new AnimatorOverrideController(baseController);
                animator.runtimeAnimatorController = overrideController;
               // Debug.Log($"{GetType().Name}: AnimatorOverrideController восстановлен: {overrideController.name}");
            }
            catch (System.Exception e)
            {
                //Debug.LogError($"{GetType().Name}: Ошибка при восстановлении AnimatorOverrideController: {e.Message}. Используется базовый контроллер.", this);
                animator.runtimeAnimatorController = baseController;
                enabled = false;
                return;
            }
        }

        //Debug.Log($"{GetType().Name}: Клипы в базовом контроллере:");
        foreach (var clip in baseController.animationClips)
        {
            //Debug.Log($"{GetType().Name}: {clip.name} (isHumanMotion: {clip.isHumanMotion})");
        }

        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);
        //Debug.Log($"{GetType().Name}: Текущие переопределения до изменения:");
        foreach (var pair in overrides)
        {
            //Debug.Log($"{GetType().Name}: {pair.Key?.name} -> {pair.Value?.name}");
        }

        int validClips = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
            {
                //Debug.LogWarning($"{GetType().Name}: Клип анимации {clipNames[i]} равен null, пропущен!", this);
                continue;
            }

            if (clips[i].isHumanMotion != animator.isHuman)
            {
                //Debug.LogWarning($"{GetType().Name}: Клип {clips[i].name} несовместим (isHumanMotion={clips[i].isHumanMotion}, аниматор isHuman={animator.isHuman}), пропущен!", this);
                continue;
            }

            string clipName = clipNames[i];
            int stateID = Animator.StringToHash(clipName);
            if (!animator.HasState(0, stateID))
            {
                //Debug.LogWarning($"{GetType().Name}: Состояние {clipName} не найдено в контроллере, пропущен!", this);
                continue;
            }

            AnimationClip originalClip = null;
            foreach (var clip in baseController.animationClips)
            {
                if (clip.name == clipName)
                {
                    originalClip = clip;
                    break;
                }
            }

            if (originalClip == null)
            {
                //Debug.LogWarning($"{GetType().Name}: Клип {clipName} не найден в базовом контроллере, используем новый клип.", this);
                originalClip = clips[i];
            }

            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(originalClip, clips[i]));
            validClips++;
            //Debug.Log($"{GetType().Name}: Переопределён клип: {clipName} -> {clips[i].name}");
        }

        if (validClips == 0)
        {
            //Debug.LogWarning($"{GetType().Name}: Ни один клип анимации не установлен. Сохраняется текущий контроллер.", this);
            animator.runtimeAnimatorController = baseController;
            return;
        }

        try
        {
            overrideController.ApplyOverrides(overrides);
            animator.runtimeAnimatorController = overrideController;
            //Debug.Log($"{GetType().Name}: Инициализация анимаций завершена. Установлено клипов: {validClips}");

            overrides.Clear();
            overrideController.GetOverrides(overrides);
            //Debug.Log($"{GetType().Name}: Переопределения после применения:");
            foreach (var pair in overrides)
            {
                //Debug.Log($"{GetType().Name}: {pair.Key?.name} -> {pair.Value?.name}");
            }
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"{GetType().Name}: Ошибка при применении анимаций: {e.Message}. Восстанавливаем базовый контроллер.", this);
            animator.runtimeAnimatorController = baseController;
            enabled = false;
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            //Debug.LogError($"{GetType().Name}: RuntimeAnimatorController сбросился после инициализации! Восстанавливаем overrideController.", this);
            animator.runtimeAnimatorController = overrideController;
        }
        else if (animator.runtimeAnimatorController != overrideController)
        {
            //Debug.LogError($"{GetType().Name}: RuntimeAnimatorController не соответствует overrideController! Восстанавливаем.", this);
            animator.runtimeAnimatorController = overrideController;
        }
    }

    public virtual void PlayAnimation(string stateName)
    {
        if (string.IsNullOrEmpty(stateName))
        {
            //Debug.LogWarning($"{GetType().Name}: Имя анимации пустое!", this);
            return;
        }

        if (animator == null || !animator.isActiveAndEnabled)
        {
            //Debug.LogWarning($"{GetType().Name}: Аниматор не активен!", this);
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            //Debug.LogError($"{GetType().Name}: RuntimeAnimatorController отсутствует! Восстанавливаем overrideController.", this);
            animator.runtimeAnimatorController = overrideController ?? baseController;
        }

        int stateID = Animator.StringToHash(stateName);
        if (!animator.HasState(0, stateID))
        {
            //Debug.LogWarning($"{GetType().Name}: Состояние {stateName} не найдено в контроллере!", this);
            return;
        }

        animator.Play(stateID);
        animator.Update(0f);
        //Debug.Log($"{GetType().Name}: Воспроизведение анимации: {stateName}");
    }

    public virtual void PlayIdle()
    {
        if (animator == null || !animator.isActiveAndEnabled)
        {
            //Debug.LogWarning($"{GetType().Name}: Аниматор не активен для воспроизведения Idle!", this);
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            //Debug.LogError($"{GetType().Name}: RuntimeAnimatorController отсутствует! Восстанавливаем overrideController.", this);
            animator.runtimeAnimatorController = overrideController ?? baseController;
        }

        int idleStateID = Animator.StringToHash("Idle");
        int fromIdleToRunStateID = Animator.StringToHash("FromIdleToRun");

        if (animator.HasState(0, idleStateID))
        {
            animator.Play(idleStateID);
            animator.Update(0f);
            //Debug.Log($"{GetType().Name}: Воспроизведение анимации Idle");
        }
        else if (animator.HasState(0, fromIdleToRunStateID))
        {
            animator.Play(fromIdleToRunStateID);
            animator.SetFloat("MovementSpeed", 0f);
            animator.Update(0f);
            //Debug.Log($"{GetType().Name}: Состояние Idle не найдено, воспроизведение FromIdleToRun с MovementSpeed=0");
        }
        else
        {
            //Debug.LogWarning($"{GetType().Name}: Состояния Idle и FromIdleToRun не найдены в контроллере!", this);
        }
    }

    public virtual bool IsPlayingAnimation(string stateName)
    {
        if (string.IsNullOrEmpty(stateName) || animator == null || !animator.isActiveAndEnabled)
        {
            //Debug.LogWarning($"{GetType().Name}: Неверное имя анимации или аниматор не активен!", this);
            return false;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlaying = state.IsName(stateName);
        //Debug.Log($"{GetType().Name}: Проверка анимации {stateName}: {isPlaying}, stateHash={state.shortNameHash}");
        return isPlaying;
    }

    public virtual float GetCurrentAnimationTime()
    {
        if (animator == null || !animator.isActiveAndEnabled)
        {
            //Debug.LogWarning($"{GetType().Name}: Аниматор не активен для получения времени анимации!", this);
            return 0f;
        }

        float time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        //Debug.Log($"{GetType().Name}: Текущее время анимации: {time}");
        return time;
    }

    public virtual Animator GetAnimator()
    {
        return animator;
    }

}