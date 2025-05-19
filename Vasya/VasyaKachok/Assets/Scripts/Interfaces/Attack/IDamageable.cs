using UnityEngine;

// Интерфейс для получения урона
public interface IDamageable
{
    void TakeDamage(float damage);
    float GetHealth();
    bool IsAlive();
}
