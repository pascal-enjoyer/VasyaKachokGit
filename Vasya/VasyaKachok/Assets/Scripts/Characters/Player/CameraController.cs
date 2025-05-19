using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private RectTransform touchAreaPanel;

    [Header("Settings")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float heightOffset = 2f;
    [SerializeField] private float touchRotationSpeed = 0.1f;

    [Header("Angle Restrictions")]
    [SerializeField] private float minVerticalAngle = 10f;
    [SerializeField] private float maxVerticalAngle = 80f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float cameraRadius = 0.2f;
    [SerializeField] private float adjustmentSpeed = 5f;
    [SerializeField] private float wallOffset = 0.1f;

    private float currentDistance;
    private float adjustedDistance;
    private Vector3 adjustedPosition;
    private Vector3 direction;

    private float currentX = 0f;
    private float currentY = 30f;
    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    private int? touchId = null;

    private void Start()
    {
        currentDistance = distance;
        adjustedDistance = distance;
    }
    private void LateUpdate()
    {
        if (target == null) return;

        UpdateCameraPosition();
        HandleAutoRotation();

        CheckCameraObstacles();
    }


    private void CheckCameraObstacles()
    {
        direction = cameraTransform.position - (target.position + Vector3.up * heightOffset);
        RaycastHit hit;

        float targetDistance = distance;

        // Используем SphereCast для учета радиуса камеры
        if (Physics.SphereCast(
            target.position + Vector3.up * heightOffset,
            cameraRadius,
            direction.normalized,
            out hit,
            distance,
            obstacleMask))
        {
            targetDistance = hit.distance - wallOffset;
        }

        // Плавная корректировка расстояния
        adjustedDistance = Mathf.Lerp(
            adjustedDistance,
            Mathf.Clamp(targetDistance, minDistance, maxDistance),
            adjustmentSpeed * Time.deltaTime);
    }


    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 offset = new Vector3(0, heightOffset, -adjustedDistance);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Плавное перемещение камеры
        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            desiredPosition,
            followSpeed * Time.deltaTime);

        // Плавное обновление текущего расстояния
        currentDistance = Mathf.Lerp(currentDistance, adjustedDistance, followSpeed * Time.deltaTime);

        cameraTransform.LookAt(target.position + Vector3.up * heightOffset);
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsValidTouch(eventData)) return;

        touchId = eventData.pointerId;
        lastTouchPosition = eventData.position;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || touchId != eventData.pointerId) return;

        Vector2 delta = eventData.position - lastTouchPosition;
        lastTouchPosition = eventData.position;

        HandleCameraRotation(delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (touchId == eventData.pointerId)
        {
            isDragging = false;
            touchId = null;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (touchId == eventData.pointerId)
        {
            isDragging = false;
            touchId = null;
        }
    }

    public void HandleCameraRotation(Vector2 delta)
    {
        currentX += delta.x * touchRotationSpeed;
        currentY -= delta.y * touchRotationSpeed;
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
    }

    public bool IsValidTouch(PointerEventData eventData)
    {
        // Проверка на другие UI элементы
        if (EventSystem.current.currentSelectedGameObject != null) return false;

        // Проверка зоны касания
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            touchAreaPanel,
            eventData.position,
            null,
            out localPoint))
        {
            return false;
        }

        return touchAreaPanel.rect.Contains(localPoint);
    }

    private void HandleAutoRotation()
    {
        CharacterController controller = target.GetComponent<CharacterController>();
        if (controller == null || isDragging) return;

        if (controller.velocity.magnitude > 0.1f)
        {
            Vector3 movementDirection = controller.velocity.normalized;
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
            currentX = Mathf.LerpAngle(
                currentX,
                targetAngle,
                rotationSpeed * Time.deltaTime * Mathf.Clamp01(1 - controller.velocity.magnitude * 0.1f));
        }
    }


    public Quaternion GetCameraRotation()
    {
        return Quaternion.Euler(0, currentX, 0);
    }
}