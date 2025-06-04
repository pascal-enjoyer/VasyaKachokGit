using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private string currentAnimation = "";
    private const float CrossFadeDuration = 0.2f;

    public void ChangeAnimation(string animName)
    {
        if (animator == null || string.IsNullOrEmpty(animName) || animName == currentAnimation)
        {
            //Debug.LogWarning($"ChangeAnimation failed: animator={animator}, animName={animName}, currentAnimation={currentAnimation}");
            return;
        }

        //Debug.Log($"Changing animation to: {animName}");
        animator.CrossFade(Animator.StringToHash(animName), CrossFadeDuration);
        currentAnimation = animName;
    }


    public void ForceAnimationUpdate()
    {
        if (animator == null || !animator.isActiveAndEnabled) return;

        string animName = GetCurrentAnimationName();
        //Debug.Log($"[Animation] Force updating animation: {animName}");
        animator.CrossFade(Animator.StringToHash(animName), CrossFadeDuration, 0, 0f);
        animator.Update(0f);
    }

    public bool IsAnimationPlaying(string animName)
    {
        if (animator == null || !animator.isActiveAndEnabled)
            return false;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        return state.IsName(animName) && state.normalizedTime < 1f;
    }

    public string GetCurrentAnimationName()
    {
        if (animator == null || !animator.isActiveAndEnabled)
        {
            //Debug.LogWarning("[Animation] Animator is not active!");
            return currentAnimation;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // ��������� ��� ��������� �����
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (animator.HasState(0, Animator.StringToHash(clip.name)))
            {
                if (state.IsName(clip.name))
                {
                    currentAnimation = clip.name;
                    return currentAnimation;
                }
            }
        }

        //Debug.LogWarning($"[Animation] Unknown animation state: {state.shortNameHash}");
        return currentAnimation;
    }
}