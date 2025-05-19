using UnityEngine;
using UnityEngine.Events;

public class StaminaManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerData playerData;

    [Header("Settings")]
    [SerializeField] private float regenDelay = 2f;
    [SerializeField][Range(0f, 1f)] private float runUnlockThreshold = 0.1f;

    public UnityEvent<float> OnStaminaChanged;
    public UnityEvent OnStaminaDepleted;
    public UnityEvent OnStaminaRestored;

    private float currentStamina;
    private float lastRunTime;
    private bool isRunning;
    private bool staminaDepleted;

    public bool CanRun => !staminaDepleted && currentStamina > 0;

    private void Awake()
    {
        currentStamina = playerData.playerStamina.maxStamina;
    }

    private void Update()
    {
        if (Time.time - lastRunTime > regenDelay && !isRunning)
        {
            RegenerateStamina();
            CheckRunUnlock();
        }
    }

    private void CheckRunUnlock()
    {
        if (staminaDepleted && currentStamina >= playerData.playerStamina.maxStamina * runUnlockThreshold)
        {
            staminaDepleted = false;
            OnStaminaRestored?.Invoke();
        }
    }

    public void StartRunning()
    {
        if (!CanRun) return;
        isRunning = true;
    }

    public void StopRunning()
    {
        isRunning = false;
        lastRunTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (isRunning && CanRun)
        {
            currentStamina -= playerData.playerStamina.staminaDrainRate * Time.fixedDeltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, playerData.playerStamina.maxStamina);

            UpdateStaminaUI();

            if (currentStamina <= 0)
            {
                staminaDepleted = true;
                OnStaminaDepleted?.Invoke();
            }
        }
    }

    private void RegenerateStamina()
    {
        if (currentStamina >= playerData.playerStamina.maxStamina) return;

        currentStamina += playerData.playerStamina.staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, playerData.playerStamina.maxStamina);
        UpdateStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        float staminaPercent = currentStamina / playerData.playerStamina.maxStamina;
        OnStaminaChanged?.Invoke(staminaPercent);
    }

    public void ResetStamina()
    {
        currentStamina = playerData.playerStamina.maxStamina;
        staminaDepleted = false;
        UpdateStaminaUI();
    }
}