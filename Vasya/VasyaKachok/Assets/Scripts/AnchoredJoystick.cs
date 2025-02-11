using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AnchoredJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private Canvas canvas;

    [Header("Settings")]
    [SerializeField] private float runRadius = 100f;
    [SerializeField][Range(0.1f, 0.5f)] private float magnetZone = 0.15f;
    [SerializeField] private float magnetResponse = 10f;

    [Header("Events")]
    public UnityEvent RunPressed;
    public UnityEvent RunReleased;

    private float walkRadius => runRadius * 0.85f;
    private Vector2 input = Vector2.zero;
    private Vector2 joystickCenter;
    private int currentPointerId = -1;
    private bool isRunning;
    private bool runAllowed = true;
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

    public void SetRunEnabled(bool enabled)
    {
        runAllowed = enabled;
        if (!runAllowed && isRunning)
        {
            SnapToWalkRadius();
            RunReleased?.Invoke();
            isRunning = false;
        }
    }

    private void SnapToWalkRadius()
    {
        Vector2 currentDirection = handle.anchoredPosition.normalized;
        handle.anchoredPosition = currentDirection * walkRadius;
        input = handle.anchoredPosition / runRadius;
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

        Vector2 touchPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventCamera,
            out touchPos)) return;

        Vector2 rawDirection = touchPos - joystickCenter;
        ProcessInput(rawDirection);
    }

    private void ProcessInput(Vector2 rawDirection)
    {
        float currentDistance = rawDirection.magnitude;
        float targetRadius = CalculateTargetRadius(currentDistance);
        Vector2 newPosition;

        // Магнитный эффект
        if (currentDistance > targetRadius - runRadius * magnetZone &&
            currentDistance < targetRadius + runRadius * magnetZone)
        {
            newPosition = rawDirection.normalized * targetRadius;
        }
        else
        {
            newPosition = rawDirection.normalized * Mathf.Clamp(
                currentDistance,
                0,
                runAllowed ? runRadius : walkRadius
            );
        }

        // Плавное перемещение
        handle.anchoredPosition = Vector2.Lerp(
            handle.anchoredPosition,
            newPosition,
            Time.deltaTime * magnetResponse
        );

        UpdateInput(handle.anchoredPosition);
        UpdateRunState(handle.anchoredPosition.magnitude);
    }

    private float CalculateTargetRadius(float currentDistance)
    {
        if (!runAllowed) return walkRadius;

        // Определяем ближайший радиус
        float walkDistance = Mathf.Abs(currentDistance - walkRadius);
        float runDistance = Mathf.Abs(currentDistance - runRadius);

        return walkDistance < runDistance ? walkRadius : runRadius;
    }

    private void UpdateInput(Vector2 position)
    {
        input = position / (isRunning ? runRadius : walkRadius);
    }

    private void UpdateRunState(float currentDistance)
    {
        bool newState = runAllowed && currentDistance >= walkRadius;

        if (newState == isRunning) return;

        isRunning = newState;
        if (isRunning)
        {
            RunPressed?.Invoke();
            input = handle.anchoredPosition / runRadius;
        }
        else
        {
            RunReleased?.Invoke();
            input = handle.anchoredPosition / walkRadius;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == currentPointerId)
        {
            handle.anchoredPosition = Vector2.zero;
            input = Vector2.zero;
            currentPointerId = -1;

            if (isRunning)
            {
                isRunning = false;
                RunReleased?.Invoke();
            }
        }
    }

    public void SetRunningEnabled(bool enabled)
    {
        if (isRunning == enabled) return;

        isRunning = enabled;
        SnapToWalkRadius();
    }
}
