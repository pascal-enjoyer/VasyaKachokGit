using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private float heightOffset = 0.2f;

    private EnemyHealth enemyHealth;
    private Collider enemyCollider;
    private Canvas healthBarCanvas;
    private Camera mainCamera;
    private float hideTimer;
    private bool isVisible;

    private void Awake()
    {
        healthBarCanvas = GetComponent<Canvas>();
        mainCamera = Camera.main;
        if (healthBarFill == null)
        {
            Debug.LogError("HealthBarFill Image is not assigned!", this);
        }
        if (healthBarCanvas == null)
        {
            Debug.LogError("Canvas component is missing!", this);
        }
        if (healthBarFill.type != Image.Type.Filled)
        {
            healthBarFill.type = Image.Type.Filled;
            healthBarFill.fillMethod = Image.FillMethod.Horizontal;
            healthBarFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
    }

    public void Initialize(EnemyHealth enemy)
    {
        enemyHealth = enemy;
        enemyCollider = enemy.GetComponent<Collider>();
        if (enemyCollider == null)
        {
            Debug.LogError("Enemy Collider is not found!", enemy);
            Destroy(gameObject);
            return;
        }
        UpdateHealthBar();
        healthBarCanvas.enabled = true;
        isVisible = true;
        hideTimer = displayTime;
    }

    private void Update()
    {
        if (enemyHealth == null || enemyCollider == null) return;

        Vector3 colliderTop = enemyCollider.bounds.center + Vector3.up * (enemyCollider.bounds.extents.y + heightOffset);
        transform.position = colliderTop;

        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }

        if (isVisible && hideTimer > 0)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0)
            {
                HideHealthBar();
            }
        }
    }

    public void ShowHealthBar()
    {
        healthBarCanvas.enabled = true;
        isVisible = true;
        hideTimer = displayTime;
        UpdateHealthBar();
    }

    public void HideHealthBar()
    {
        healthBarCanvas.enabled = false;
        isVisible = false;
    }

    public void UpdateHealthBar()
    {
        if (enemyHealth == null) return;
        float healthPercentage = (float)enemyHealth.GetCurrentHealth() / enemyHealth.GetMaxHealth();
        healthBarFill.fillAmount = Mathf.Clamp01(healthPercentage);
    }

    private void OnDestroy()
    {
        if (enemyHealth != null && HealthBarManager.Instance != null)
        {
            HealthBarManager.Instance.RemoveHealthBar(enemyHealth);
        }
    }
}