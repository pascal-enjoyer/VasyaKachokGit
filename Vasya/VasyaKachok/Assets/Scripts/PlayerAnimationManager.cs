using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Параметры аниматора


    private string currentAnimation;

    public void ChangeAnimation(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;
        currentAnimation = newAnimation;
        animator.CrossFade(newAnimation, 0.2f);
    }
}
