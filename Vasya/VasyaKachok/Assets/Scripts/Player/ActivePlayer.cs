using UnityEditor;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

public class ActivePlayer : MonoBehaviour, IDamageable
{
    public PlayerAnimationManager animator;
    public PlayerData playerData;
    public int maxHealth => playerData.health;
    public int currentHealth;

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        animator.ChangeAnimation("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        animator.ChangeAnimation("die");
        Destroy(gameObject, 3f);
    }

}
