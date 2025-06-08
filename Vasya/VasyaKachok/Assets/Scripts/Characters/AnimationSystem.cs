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
               // Debug.LogError($"{GetType().Name}: �������� �� ������ �� �������! �������� ��������� Animator.", this);
                enabled = false;
                return;
            }
        }

        if (animator.runtimeAnimatorController == null)
        {
            //Debug.LogError($"{GetType().Name}: � ��������� ����������� RuntimeAnimatorController! ��������� AnimatorController � ����������.", this);
            enabled = false;
            return;
        }

        baseController = animator.runtimeAnimatorController;
        //Debug.Log($"{GetType().Name}: ������� ������� ����������: {baseController.name} (������: {baseController.animationClips.Length})");

        try
        {
            overrideController = new AnimatorOverrideController(baseController);
            if (overrideController == null)
            {
                //Debug.LogError($"{GetType().Name}: �� ������� ������� AnimatorOverrideController!", this);
                enabled = false;
                return;
            }
            animator.runtimeAnimatorController = overrideController;
            //Debug.Log($"{GetType().Name}: AnimatorOverrideController ������� ������ � ��������: {overrideController.name}");
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"{GetType().Name}: ������ ��� �������� AnimatorOverrideController: {e.Message}. ������������ ������� ����������.", this);
            animator.runtimeAnimatorController = baseController;
            enabled = false;
            return;
        }
    }

    public virtual void InitializeAnimations(AnimationClip[] clips, string[] clipNames)
    {
        if (clips == null || clips.Length == 0 || clipNames == null || clipNames.Length == 0 || clips.Length != clipNames.Length)
        {
            //Debug.LogWarning($"{GetType().Name}: ����� �������� ��� ����� �� ������������� ��� �� ��������� �� �����. ������������ ������� ����������.", this);
            return;
        }

        if (overrideController == null)
        {
            //Debug.LogError($"{GetType().Name}: AnimatorOverrideController �� ���������������! ���������������.", this);
            try
            {
                overrideController = new AnimatorOverrideController(baseController);
                animator.runtimeAnimatorController = overrideController;
               // Debug.Log($"{GetType().Name}: AnimatorOverrideController ������������: {overrideController.name}");
            }
            catch (System.Exception e)
            {
                //Debug.LogError($"{GetType().Name}: ������ ��� �������������� AnimatorOverrideController: {e.Message}. ������������ ������� ����������.", this);
                animator.runtimeAnimatorController = baseController;
                enabled = false;
                return;
            }
        }

        //Debug.Log($"{GetType().Name}: ����� � ������� �����������:");
        foreach (var clip in baseController.animationClips)
        {
            //Debug.Log($"{GetType().Name}: {clip.name} (isHumanMotion: {clip.isHumanMotion})");
        }

        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);
        //Debug.Log($"{GetType().Name}: ������� ��������������� �� ���������:");
        foreach (var pair in overrides)
        {
            //Debug.Log($"{GetType().Name}: {pair.Key?.name} -> {pair.Value?.name}");
        }

        int validClips = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
            {
                //Debug.LogWarning($"{GetType().Name}: ���� �������� {clipNames[i]} ����� null, ��������!", this);
                continue;
            }

            if (clips[i].isHumanMotion != animator.isHuman)
            {
                //Debug.LogWarning($"{GetType().Name}: ���� {clips[i].name} ����������� (isHumanMotion={clips[i].isHumanMotion}, �������� isHuman={animator.isHuman}), ��������!", this);
                continue;
            }

            string clipName = clipNames[i];
            int stateID = Animator.StringToHash(clipName);
            if (!animator.HasState(0, stateID))
            {
                //Debug.LogWarning($"{GetType().Name}: ��������� {clipName} �� ������� � �����������, ��������!", this);
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
                //Debug.LogWarning($"{GetType().Name}: ���� {clipName} �� ������ � ������� �����������, ���������� ����� ����.", this);
                originalClip = clips[i];
            }

            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(originalClip, clips[i]));
            validClips++;
            //Debug.Log($"{GetType().Name}: ������������ ����: {clipName} -> {clips[i].name}");
        }

        if (validClips == 0)
        {
            //Debug.LogWarning($"{GetType().Name}: �� ���� ���� �������� �� ����������. ����������� ������� ����������.", this);
            animator.runtimeAnimatorController = baseController;
            return;
        }

        try
        {
            overrideController.ApplyOverrides(overrides);
            animator.runtimeAnimatorController = overrideController;
            //Debug.Log($"{GetType().Name}: ������������� �������� ���������. ����������� ������: {validClips}");

            overrides.Clear();
            overrideController.GetOverrides(overrides);
            //Debug.Log($"{GetType().Name}: ��������������� ����� ����������:");
            foreach (var pair in overrides)
            {
                //Debug.Log($"{GetType().Name}: {pair.Key?.name} -> {pair.Value?.name}");
            }
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"{GetType().Name}: ������ ��� ���������� ��������: {e.Message}. ��������������� ������� ����������.", this);
            animator.runtimeAnimatorController = baseController;
            enabled = false;
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            //Debug.LogError($"{GetType().Name}: RuntimeAnimatorController ��������� ����� �������������! ��������������� overrideController.", this);
            animator.runtimeAnimatorController = overrideController;
        }
        else if (animator.runtimeAnimatorController != overrideController)
        {
            //Debug.LogError($"{GetType().Name}: RuntimeAnimatorController �� ������������� overrideController! ���������������.", this);
            animator.runtimeAnimatorController = overrideController;
        }
    }

    public virtual void PlayAnimation(string stateName)
    {
        if (string.IsNullOrEmpty(stateName))
        {
            //Debug.LogWarning($"{GetType().Name}: ��� �������� ������!", this);
            return;
        }

        if (animator == null || !animator.isActiveAndEnabled)
        {
            //Debug.LogWarning($"{GetType().Name}: �������� �� �������!", this);
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            //Debug.LogError($"{GetType().Name}: RuntimeAnimatorController �����������! ��������������� overrideController.", this);
            animator.runtimeAnimatorController = overrideController ?? baseController;
        }

        int stateID = Animator.StringToHash(stateName);
        if (!animator.HasState(0, stateID))
        {
            //Debug.LogWarning($"{GetType().Name}: ��������� {stateName} �� ������� � �����������!", this);
            return;
        }

        animator.Play(stateID);
        animator.Update(0f);
        //Debug.Log($"{GetType().Name}: ��������������� ��������: {stateName}");
    }

    public virtual void PlayIdle()
    {
        if (animator == null || !animator.isActiveAndEnabled)
        {
            //Debug.LogWarning($"{GetType().Name}: �������� �� ������� ��� ��������������� Idle!", this);
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            //Debug.LogError($"{GetType().Name}: RuntimeAnimatorController �����������! ��������������� overrideController.", this);
            animator.runtimeAnimatorController = overrideController ?? baseController;
        }

        int idleStateID = Animator.StringToHash("Idle");
        int fromIdleToRunStateID = Animator.StringToHash("FromIdleToRun");

        if (animator.HasState(0, idleStateID))
        {
            animator.Play(idleStateID);
            animator.Update(0f);
            //Debug.Log($"{GetType().Name}: ��������������� �������� Idle");
        }
        else if (animator.HasState(0, fromIdleToRunStateID))
        {
            animator.Play(fromIdleToRunStateID);
            animator.SetFloat("MovementSpeed", 0f);
            animator.Update(0f);
            //Debug.Log($"{GetType().Name}: ��������� Idle �� �������, ��������������� FromIdleToRun � MovementSpeed=0");
        }
        else
        {
            //Debug.LogWarning($"{GetType().Name}: ��������� Idle � FromIdleToRun �� ������� � �����������!", this);
        }
    }

    public virtual bool IsPlayingAnimation(string stateName)
    {
        if (string.IsNullOrEmpty(stateName) || animator == null || !animator.isActiveAndEnabled)
        {
            //Debug.LogWarning($"{GetType().Name}: �������� ��� �������� ��� �������� �� �������!", this);
            return false;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlaying = state.IsName(stateName);
        //Debug.Log($"{GetType().Name}: �������� �������� {stateName}: {isPlaying}, stateHash={state.shortNameHash}");
        return isPlaying;
    }

    public virtual float GetCurrentAnimationTime()
    {
        if (animator == null || !animator.isActiveAndEnabled)
        {
            //Debug.LogWarning($"{GetType().Name}: �������� �� ������� ��� ��������� ������� ��������!", this);
            return 0f;
        }

        float time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        //Debug.Log($"{GetType().Name}: ������� ����� ��������: {time}");
        return time;
    }

    public virtual Animator GetAnimator()
    {
        return animator;
    }

}