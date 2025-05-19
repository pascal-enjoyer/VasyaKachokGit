using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;



    private string currentAnimation;


    public void ChangeRunSpeed(float speed)
    {
        animator.SetFloat("MovementSpeed", speed);
    }

    public void ChangeAnimation(string newAnimation)
    {
        currentAnimation = newAnimation;
    }
}
