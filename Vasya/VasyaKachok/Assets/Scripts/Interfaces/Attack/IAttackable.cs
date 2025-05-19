using UnityEngine;

// ��������� ��� �����
public interface IAttackable
{
    void Attack(IDamageable damageable); 
    float GetAttackCooldown();
}
