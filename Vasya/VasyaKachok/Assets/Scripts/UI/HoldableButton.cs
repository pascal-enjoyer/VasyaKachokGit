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
    public UnityEngine.Events.UnityEvent<Vector2> OnPress; // Событие с передачей позиции нажатия
    public UnityEngine.Events.UnityEvent<Vector2> OnRelease; // Событие с передачей позиции отпускания
    public UnityEngine.Events.UnityEvent<Vector2> OnHold; // Событие с передачей позиции удержания

    private bool isPressed;
    private bool isHolding;
    private float holdTimer;
    private Button button;
    private Vector2 lastPointerPosition; // Последняя позиция касания

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
            // Для клавиатуры используем позицию центра кнопки
            lastPointerPosition = GetButtonCenterPosition();
            StartPress(lastPointerPosition);
        }

        if (Input.GetKeyUp(keyboardKey) && isPressed)
        {
            EndPress(lastPointerPosition);
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
                OnHold?.Invoke(lastPointerPosition);
                holdTimer = 0;
            }
        }
        else if (holdTimer >= holdDelay)
        {
            // Начало удержания после задержки
            isHolding = true;
            holdTimer = 0;
            OnHold?.Invoke(lastPointerPosition);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;

        // Сохраняем позицию касания
        lastPointerPosition = eventData.position;
        StartPress(lastPointerPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;

        // Сохраняем позицию отпускания
        lastPointerPosition = eventData.position;
        EndPress(lastPointerPosition);
    }

    private void StartPress(Vector2 position)
    {
        if (!button.interactable) return;

        isPressed = true;
        holdTimer = 0;
        isHolding = false;
        OnPress?.Invoke(position);

        // Анимация нажатия
        if (button.animator != null)
            button.animator.SetTrigger("Pressed");
    }

    private void EndPress(Vector2 position)
    {
        isPressed = false;
        isHolding = false;
        OnRelease?.Invoke(position);

        // Анимация отпускания
        if (button.animator != null)
            button.animator.SetTrigger("Normal");
    }

    // Для внешнего управления
    public void SimulatePress(bool state, Vector2 position)
    {
        if (state) StartPress(position);
        else EndPress(position);
    }

    // Получение центра кнопки в экранных координатах
    private Vector2 GetButtonCenterPosition()
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
        return screenPosition;
    }
}