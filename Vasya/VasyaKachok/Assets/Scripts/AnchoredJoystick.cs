using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AnchoredJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private Canvas canvas;

    [Header("Events")]
    public UnityEvent<Vector2> OnValueChanged;

    private float backgroundRadius;
    private Vector2 input = Vector2.zero;
    private int currentPointerId = -1;
    private Camera eventCamera;

    public float Horizontal => input.x;
    public float Vertical => input.y;
    public Vector2 Direction => input;

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

        // Calculate radius based on the smallest dimension (to ensure it's a circle)
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
        // Convert screen position to local position
        Vector2 localPosition;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            screenPosition,
            eventCamera,
            out localPosition)) return;

        // Calculate direction and clamp to background radius
        Vector2 direction = localPosition;
        float distance = direction.magnitude;

        // Normalize direction within the circular boundary
        if (distance > backgroundRadius)
            direction = direction.normalized * backgroundRadius;

        // Update handle position
        handle.anchoredPosition = direction;

        // Calculate input values with circular normalization
        input = direction / backgroundRadius;

        // Ensure the input is within the unit circle
        input = Vector2.ClampMagnitude(input, 1f);

        // Trigger event with current input values
        OnValueChanged?.Invoke(input);
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
    }
}