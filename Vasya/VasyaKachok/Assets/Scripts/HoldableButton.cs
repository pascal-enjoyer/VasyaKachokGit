using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    [SerializeField] private KeyCode keyboardKey = KeyCode.Space;
    [SerializeField] private float holdDelay = 0.5f;
    [SerializeField] private float repeatInterval = 0.1f;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnPress;
    public UnityEngine.Events.UnityEvent OnRelease;
    public UnityEngine.Events.UnityEvent OnHold;

    private bool isPressed;
    private bool isHolding;
    private float holdTimer;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        HandleKeyboardInput();
        ProcessHold();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(keyboardKey) && !isPressed)
        {
            StartPress();
        }

        if (Input.GetKeyUp(keyboardKey) && isPressed)
        {
            EndPress();
        }
    }

    private void ProcessHold()
    {
        if (!isPressed || !button.interactable) return;

        holdTimer += Time.deltaTime;

        if (isHolding)
        {
            // Повторяющееся действие при удержании
            if (repeatInterval > 0 && holdTimer >= repeatInterval)
            {
                OnHold?.Invoke();
                holdTimer = 0;
            }
        }
        else if (holdTimer >= holdDelay)
        {
            // Начало удержания после задержки
            isHolding = true;
            holdTimer = 0;
            OnHold?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        StartPress();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;
        EndPress();
    }

    private void StartPress()
    {
        if (!button.interactable) return;

        isPressed = true;
        holdTimer = 0;
        isHolding = false;
        OnPress?.Invoke();

        // Анимация нажатия
        if (button.animator != null)
            button.animator.SetTrigger("Pressed");
    }

    private void EndPress()
    {
        isPressed = false;
        isHolding = false;
        OnRelease?.Invoke();

        // Анимация отпускания
        if (button.animator != null)
            button.animator.SetTrigger("Normal");
    }

    // Для внешнего управления
    public void SimulatePress(bool state)
    {
        if (state) StartPress();
        else EndPress();
    }
}