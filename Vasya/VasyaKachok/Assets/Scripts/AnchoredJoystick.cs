using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AnchoredJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private Canvas canvas;

    [Header("Radius Settings")]
    [SerializeField] private float walkRadius = 50f;
    [SerializeField] private float runRadius = 75f;
    [SerializeField] private float magnetForce = 15f;

    [Header("Events")]
    public UnityEvent RunPressed;
    public UnityEvent RunReleased;

    private Vector2 input = Vector2.zero;
    private Vector2 joystickCenter = Vector2.zero;
    private int currentPointerId = -1;
    private bool isRunning = false;
    private bool runEnabled = true;
    private Camera eventCamera;

    public float Horizontal => input.x;
    public float Vertical => input.y;
    public Vector2 Direction => input;

    private void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ?
            null : canvas.worldCamera;
    }

    public void SetRunningEnabled(bool enabled)
    {
        runEnabled = enabled;
        ApplyMagneticEffect(handle.anchoredPosition, true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentPointerId == -1)
        {
            currentPointerId = eventData.pointerId;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                eventData.position,
                eventCamera,
                out joystickCenter);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != currentPointerId) return;

        Vector2 touchPosition;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventCamera,
            out touchPosition)) return;

        Vector2 rawDirection = touchPosition - joystickCenter;
        Vector2 processedDirection = ApplyMagneticEffect(rawDirection);

        UpdateHandlePosition(processedDirection);
        UpdateRunState(processedDirection.magnitude);
    }
    private Vector2 ApplyMagneticEffect(Vector2 rawDirection, bool forceUpdate = false)
    {
        float currentMaxRadius = runEnabled ? runRadius : walkRadius;
        float rawDistance = rawDirection.magnitude;

        if (rawDistance <= 0.01f) return Vector2.zero;

        // Увеличиваем зону влияния магнита до 30% от радиуса
        float threshold = currentMaxRadius * 0.3f;
        float magnetZoneStart = currentMaxRadius - threshold;

        // Всегда применяем магнит к ближайшему радиусу
        float targetRadius = currentMaxRadius;
        if (!runEnabled)
        {
            // Если бег отключен, магнит только к радиусу ходьбы
            targetRadius = walkRadius;
            magnetZoneStart = walkRadius - threshold;
        }

        if (rawDistance > magnetZoneStart || forceUpdate)
        {
            // Усиливаем эффект магнита
            float lerpFactor = Mathf.Clamp01((rawDistance - magnetZoneStart) / threshold);
            lerpFactor = Mathf.Pow(lerpFactor, 0.5f); // Увеличиваем силу притяжения

            float targetDistance = Mathf.Lerp(magnetZoneStart, targetRadius, lerpFactor);
            return rawDirection.normalized * targetDistance;
        }
        return rawDirection;
    }
    private void UpdateHandlePosition(Vector2 direction)
    {
        float currentMaxRadius = runEnabled ? runRadius : walkRadius;
        float clampedDistance = Mathf.Clamp(direction.magnitude, 0, currentMaxRadius);

        // Явное притягивание к границе при близком расстоянии
        if (Mathf.Abs(clampedDistance - currentMaxRadius) < 2f)
        {
            clampedDistance = currentMaxRadius;
        }

        Vector2 finalDirection = direction.normalized * clampedDistance;
        handle.anchoredPosition = finalDirection;
        input = finalDirection / currentMaxRadius;
    }

    private void UpdateRunState(float currentDistance)
    {
        bool wasRunning = isRunning;
        isRunning = runEnabled && currentDistance > walkRadius;

        if (!wasRunning && isRunning) RunPressed?.Invoke();
        else if (wasRunning && !isRunning) RunReleased?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == currentPointerId)
        {
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            currentPointerId = -1;
            if (isRunning)
            {
                isRunning = false;
                RunReleased?.Invoke();
            }
        }
    }
}