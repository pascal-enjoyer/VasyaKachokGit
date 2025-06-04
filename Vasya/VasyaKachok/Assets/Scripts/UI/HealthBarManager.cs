using UnityEngine;
using System.Collections.Generic;

public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance { get; private set; }

    [SerializeField] private GameObject healthBarPrefab; // Префаб HealthBar
    private Dictionary<EnemyHealth, EnemyHealthBar> activeHealthBars; // Активные полоски

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple HealthBarManager instances detected! Destroying this one.", this);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        activeHealthBars = new Dictionary<EnemyHealth, EnemyHealthBar>();
        if (healthBarPrefab == null)
        {
            Debug.LogError("HealthBar Prefab is not assigned in HealthBarManager!", this);
        }
    }

    private void OnEnable()
    {
        EnemyHealth.EnemyDamageTaken += OnEnemyDamageTaken;
    }

    private void OnDisable()
    {
        EnemyHealth.EnemyDamageTaken -= OnEnemyDamageTaken;
    }

    private void OnEnemyDamageTaken(EnemyHealth enemy)
    {
        if (activeHealthBars.ContainsKey(enemy))
        {
            activeHealthBars[enemy].ShowHealthBar();
            return;
        }

        GameObject healthBarGO = Instantiate(healthBarPrefab, enemy.GetTransform().position, Quaternion.identity);
        EnemyHealthBar healthBar = healthBarGO.GetComponent<EnemyHealthBar>();
        if (healthBar != null)
        {
            healthBar.Initialize(enemy);
            activeHealthBars.Add(enemy, healthBar);
        }
        else
        {
            Debug.LogError("HealthBar component not found on instantiated prefab!", healthBarGO);
            Destroy(healthBarGO);
        }
    }

    public void RemoveHealthBar(EnemyHealth enemy)
    {
        if (activeHealthBars.ContainsKey(enemy))
        {
            Destroy(activeHealthBars[enemy].gameObject);
            activeHealthBars.Remove(enemy);
        }
    }
}