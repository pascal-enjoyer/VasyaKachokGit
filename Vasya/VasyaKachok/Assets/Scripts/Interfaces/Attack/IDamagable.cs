using UnityEngine;

public interface IDamagable 
{
    void TakeDamage(int damage);
    bool IsAlive();
    Transform GetTransform();
    void Kill();
}