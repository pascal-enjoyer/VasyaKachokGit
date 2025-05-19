using UnityEngine;
using static UnityEngine.GraphicsBuffer;



// Компонент атаки
public class MeleeAttack : MonoBehaviour, IAttackable
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;

    private void Awake()
    {
        lastAttackTime = 0f;
    }

    public void Attack(IDamageable damageable)
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            damageable.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }

    public float GetAttackCooldown()
    {
        return attackCooldown;
    }
}
