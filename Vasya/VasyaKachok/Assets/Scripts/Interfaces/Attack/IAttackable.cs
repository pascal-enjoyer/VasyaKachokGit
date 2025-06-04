using UnityEngine;

public interface IAttackable
{
    void Attack(IDamagable target);
    float GetAttackRange();
    float GetAttackSpeed();
}
