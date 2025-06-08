using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private float heightOffset = 0.2f;

    private EnemyHealth enemyHealth;
    private Collider enemyCollider;
    private Canvas healthBarCanvas;
    private float hideTimer;
    private bool isVisible;

    private void Awake()
    {
        healthBarCanvas = GetComponent<Canvas>();
        if (healthBarFill == null || healthBarCanvas == null)
        {
            //Debug.LogError("HealthBarFill or Canvas not assigned!", this);
            Destroy(gameObject);
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
            //Debug.LogError("Enemy Collider not found!", enemy);
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

        transform.LookAt(transform.position + Camera.main.transform.forward);

        if (isVisible && hideTimer > 0)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0)
            {
                healthBarCanvas.enabled = false;
                isVisible = false;
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

    public void UpdateHealthBar()
    {
        if (enemyHealth == null) return;
        healthBarFill.fillAmount = (float)enemyHealth.GetCurrentHealth() / enemyHealth.GetMaxHealth();
    }
}