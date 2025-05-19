using UnityEngine;

// ��������� ��� ��������� �����
public interface IDamageable
{
    void TakeDamage(float damage);
    float GetHealth();
    bool IsAlive();
}
