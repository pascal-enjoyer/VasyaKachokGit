using UnityEngine;
using UnityEngine.EventSystems;

public class AnchoredJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private Canvas canvas;

    [Header("Settings")]
    [SerializeField] private float handleRange = 1f;
    [SerializeField] private float deadZone = 0f;
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;

    private Vector2 input = Vector2.zero;
    private Vector2 joystickCenter = Vector2.zero;
    private int currentPointerId = -1;

    public float Horizontal => input.x;
    public float Vertical => input.y;
    public Vector2 Direction => input;

    private void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentPointerId == -1)
        {
            currentPointerId = eventData.pointerId;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                eventData.position,
                canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null,
                out joystickCenter);

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != currentPointerId) return;

        Vector2 touchPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null,
            out touchPosition);

        Vector2 direction = touchPosition - joystickCenter;
        float radius = background.rect.width * 0.5f;

        input = (direction.magnitude > radius)
            ? direction.normalized
            : direction / radius;

        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);

        handle.anchoredPosition = input * radius * handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == currentPointerId)
        {
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            currentPointerId = -1;
        }
    }

    private void UpdateInput(Vector2 direction)
    {
        if (direction.magnitude > deadZone)
            input = direction.normalized * ((direction.magnitude - deadZone) / (1 - deadZone));
        else
            input = Vector2.zero;
    }

    public enum AxisOptions { Both, Horizontal, Vertical }
}