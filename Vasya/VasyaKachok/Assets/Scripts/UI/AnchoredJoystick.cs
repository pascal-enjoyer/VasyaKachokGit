using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting;

public class AnchoredJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private Canvas canvas;

    [Header("Events")]
    public UnityEvent<Vector2> OnValueChanged; // Отправляет направление (x, y)
    public UnityEvent<float> OnDistanceChanged; // Новое событие: 0-1 расстояние от центра

    private float backgroundRadius;
    private Vector2 input = Vector2.zero;
    private int currentPointerId = -1;
    private Camera eventCamera;

    public float Horizontal => input.x;
    public float Vertical => input.y;
    public Vector2 Direction => input;
    public float Distance => input.magnitude; // Новое свойство для прямого доступа

    private void Start()
    {
        InitializeJoystick();
    }

    private void InitializeJoystick()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ?
            null : canvas.worldCamera;

        backgroundRadius = Mathf.Min(background.rect.width, background.rect.height) / 2f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentPointerId == -1)
        {
            currentPointerId = eventData.pointerId;
            UpdateHandlePosition(eventData.position);


        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != currentPointerId) return;
        UpdateHandlePosition(eventData.position);

    }

    private void UpdateHandlePosition(Vector2 screenPosition)
    {
        Vector2 localPosition;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            screenPosition,
            eventCamera,
            out localPosition)) return;

        Vector2 direction = localPosition;
        float distance = direction.magnitude;

        if (distance > backgroundRadius)
            direction = direction.normalized * backgroundRadius;

        handle.anchoredPosition = direction;
        input = direction / backgroundRadius;
        input = Vector2.ClampMagnitude(input, 1f);

        // Вызов обоих событий
        OnValueChanged?.Invoke(input);

        //Debug.Log(input.magnitude);
        OnDistanceChanged?.Invoke(input.magnitude); // Отправляем нормализованное расстояние
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == currentPointerId)
        {
            ResetJoystick();
        }
    }

    private void ResetJoystick()
    {
        handle.anchoredPosition = Vector2.zero;
        input = Vector2.zero;
        currentPointerId = -1;
        OnValueChanged?.Invoke(input);
        OnDistanceChanged?.Invoke(0f); // Сбрасываем расстояние в 0
    }
}