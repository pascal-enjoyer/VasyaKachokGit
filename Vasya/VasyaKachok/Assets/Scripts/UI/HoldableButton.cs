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
    public UnityEngine.Events.UnityEvent<Vector2> OnPress; // ������� � ��������� ������� �������
    public UnityEngine.Events.UnityEvent<Vector2> OnRelease; // ������� � ��������� ������� ����������
    public UnityEngine.Events.UnityEvent<Vector2> OnHold; // ������� � ��������� ������� ���������

    private bool isPressed;
    private bool isHolding;
    private float holdTimer;
    private Button button;
    private Vector2 lastPointerPosition; // ��������� ������� �������

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
            // ��� ���������� ���������� ������� ������ ������
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
            // ������������� �������� ��� ���������
            if (repeatInterval > 0 && holdTimer >= repeatInterval)
            {
                OnHold?.Invoke(lastPointerPosition);
                holdTimer = 0;
            }
        }
        else if (holdTimer >= holdDelay)
        {
            // ������ ��������� ����� ��������
            isHolding = true;
            holdTimer = 0;
            OnHold?.Invoke(lastPointerPosition);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;

        // ��������� ������� �������
        lastPointerPosition = eventData.position;
        StartPress(lastPointerPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;

        // ��������� ������� ����������
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

        // �������� �������
        if (button.animator != null)
            button.animator.SetTrigger("Pressed");
    }

    private void EndPress(Vector2 position)
    {
        isPressed = false;
        isHolding = false;
        OnRelease?.Invoke(position);

        // �������� ����������
        if (button.animator != null)
            button.animator.SetTrigger("Normal");
    }

    // ��� �������� ����������
    public void SimulatePress(bool state, Vector2 position)
    {
        if (state) StartPress(position);
        else EndPress(position);
    }

    // ��������� ������ ������ � �������� �����������
    private Vector2 GetButtonCenterPosition()
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
        return screenPosition;
    }
}