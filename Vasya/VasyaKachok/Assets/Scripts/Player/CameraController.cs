
using UnityEngine;


public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target; // Цель (игрок)
    [SerializeField] private Transform cameraTransform; // Трансформ камеры

    [Header("Settings")]
    [SerializeField] private float distance = 5f; // Дистанция от камеры до игрока
    [SerializeField] private float minDistance = 2f; // Минимальная дистанция
    [SerializeField] private float maxDistance = 10f; // Максимальная дистанция
    [SerializeField] private float rotationSpeed = 5f; // Скорость вращения камеры
    [SerializeField] private float followSpeed = 10f; // Скорость следования камеры
    [SerializeField] private float heightOffset = 2f; // Высота камеры относительно игрока

    private float currentX = 0f; // Текущий угол вращения по горизонтали
    private float currentY = 30f; // Текущий угол вращения по вертикали

    private void LateUpdate()
    {
        if (target == null) return;
        
        // Вращение камеры вокруг игрока
        HandleCameraRotation();
        // Обновление позиции камеры
        UpdateCameraPosition();
    }

    private void HandleCameraRotation()
    {
        // Автоматическое вращение камеры, если игрок движется
        if (target.GetComponent<CharacterController>().velocity.magnitude > 0.1f)
        {
            // Вычисляем направление движения игрока
            Vector3 movementDirection = target.GetComponent<CharacterController>().velocity.normalized;
            // Вычисляем целевой угол вращения камеры
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
            // Плавно интерполируем текущий угол к целевому
            currentX = Mathf.LerpAngle(currentX, targetAngle, rotationSpeed * Time.deltaTime);
        }

        
    }

    private void UpdateCameraPosition()
    {
        // Вычисляем позицию камеры на основе углов и дистанции
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 offset = new Vector3(0, heightOffset, -distance);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Плавное перемещение камеры
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPosition, followSpeed * Time.deltaTime);
        // Камера всегда смотрит на игрока
        cameraTransform.LookAt(target.position + Vector3.up * heightOffset);
    }

    // Метод для получения текущего угла вращения камеры (для управления игроком)
    public Quaternion GetCameraRotation()
    {
        return Quaternion.Euler(0, currentX, 0);
    }
}