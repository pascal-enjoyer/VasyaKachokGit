using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private RectTransform touchAreaPanel; // UI панель для зоны касания

    [Header("Settings")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float heightOffset = 2f;
    [SerializeField] private float touchRotationSpeed = 0.1f;

    [Header("Angle Restrictions")]
    [SerializeField] private float minVerticalAngle = 10f; // Минимальный угол наклона
    [SerializeField] private float maxVerticalAngle = 80f; // Максимальный угол наклона

    private float currentX = 0f;
    private float currentY = 30f;

    private bool rotating = false;

    private void LateUpdate()
    {
        if (target == null) return;
        
        HandleCameraRotation();
        UpdateCameraPosition();
    }

    public void HandleTouchRotation(Vector2 touchPos)
    {
        Vector2 deltaPosition = touchPos;
        currentX += deltaPosition.x * touchRotationSpeed;
        currentY -= deltaPosition.y * touchRotationSpeed;

        // Ограничение вертикального угла
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
     
    }

    public void ToggleUserRotation(bool isInPanel)
    {
        rotating = isInPanel;
    }

    private bool IsTouchInPanel(Vector2 touchPosition)
    {
        // Конвертируем позицию касания в локальные координаты панели
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            touchAreaPanel,
            touchPosition,
            null,
            out localPoint);

        // Проверяем находится ли точка внутри прямоугольника панели
        return touchAreaPanel.rect.Contains(localPoint);
    }

    private void HandleCameraRotation()
    {
        CharacterController controller = target.GetComponent<CharacterController>();
        if (controller == null) return;

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

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 offset = new Vector3(0, heightOffset, -distance);
        Vector3 desiredPosition = target.position + rotation * offset;

        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            desiredPosition,
            followSpeed * Time.deltaTime);

        cameraTransform.LookAt(target.position + Vector3.up * heightOffset);
    }

    public Quaternion GetCameraRotation()
    {
        return Quaternion.Euler(0, currentX, 0);
    }
}