
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;


    private string currentAnimation;

    public void ChangeAnimation(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;
        currentAnimation = newAnimation;
        animator.CrossFade(newAnimation, 0.2f);
    }

    public void SetMovementSpeed(float speed)
    {
        if (speed > 0.1f)
            ChangeAnimation("Standard Run");
        else
            ChangeAnimation("Standart Walk");
    }

}