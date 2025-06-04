using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour, IDamagable, IEnemyDataUser
{
    public static event Action<EnemyHealth> EnemyDamageTaken;

    private EnemyBase enemyBase;
    private EnemyData enemyData;
    private int currentHealth;
    private int maxHealth => enemyData.maxHealth;

    private void Awake()
    {
        enemyBase = GetComponent<EnemyBase>();
    }

    private void Start()
    {
        SetData(enemyBase.GetEnemyData());
    }

    public void Init()
    {
        currentHealth = maxHealth;
    }

    public void SetData(EnemyData enemyData)
    {
        this.enemyData = enemyData;
        Init();
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        Debug.Log($"Enemy {gameObject.name} health: {currentHealth}");
        EnemyDamageTaken?.Invoke(this);
        if (currentHealth <= 0)
        {
            if (HealthBarManager.Instance != null)
            {
                HealthBarManager.Instance.RemoveHealthBar(this);
            }
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Kill()
    {
        TakeDamage(currentHealth);
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}