using UnityEngine;

// Интерфейс для атаки
public interface IAttackable
{
    void Attack(IDamageable damageable); 
    float GetAttackCooldown();
}
