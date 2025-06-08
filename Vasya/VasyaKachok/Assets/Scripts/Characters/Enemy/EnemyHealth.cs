using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, IDamagable, IEnemyDataUser
{
    [SerializeField] private GameObject healthBarPrefab;

    private EnemyBase enemyBase;
    private EnemyData enemyData;
    private int currentHealth;
    private int maxHealth => enemyData.maxHealth;
    private HealthBar healthBar;

    public UnityEvent EnemyTookDamage;
    public UnityEvent EnemyDie;

    private void Awake()
    {
        enemyBase = GetComponent<EnemyBase>();
        if (healthBarPrefab == null)
        {
            Debug.LogError("HealthBar Prefab not assigned!", this);
        }
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
        
        if (healthBar == null && healthBarPrefab != null && currentHealth > 0)
        {
            GameObject healthBarGO = Instantiate(healthBarPrefab, transform);
            healthBar = healthBarGO.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.Initialize(this);
            }
            else
            {
                Debug.LogError("HealthBar component not found on instantiated prefab!", healthBarGO);
                Destroy(healthBarGO);
            }
        }

        if (healthBar != null)
        {
            healthBar.ShowHealthBar();
        }

        if (currentHealth <= 0)
        {
            // Логика смерти врага
            EnemyDie?.Invoke();

            Destroy(gameObject, 100000);
            
            return;
        }
        EnemyTookDamage?.Invoke();
    }

    public Transform GetTransform() => transform;

    public void Kill() => TakeDamage(currentHealth);

    public bool IsAlive() => currentHealth > 0;

    public int GetCurrentHealth() => currentHealth;

    public int GetMaxHealth() => maxHealth;
}