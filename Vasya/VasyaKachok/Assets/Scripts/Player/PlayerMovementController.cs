using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AnchoredJoystick joystick;
    [SerializeField] private PlayerAnimationManager animationManager;
    [SerializeField] private CameraController thirdPersonCamera;

    [Header("Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    private CharacterController characterController;
    private bool isRunning;
    private float currentSpeed;
    private bool canRun = true;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        UpdateAnimations();
    }

    private void HandleMovement()
    {
        // Получаем направление от джойстика
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // Получаем нормализованную дистанцию от центра джойстика (от 0 до 1)
        float normalizedDistance = joystick.Direction.magnitude;

        if (direction.magnitude >= 0.1f)
        {
            // Учитываем поворот камеры
            Quaternion cameraRotation = thirdPersonCamera.GetCameraRotation();
            Vector3 moveDirection = cameraRotation * direction;
            moveDirection.y = 0;

            // Вычисляем текущую скорость с учетом дистанции от центра джойстика
            currentSpeed = isRunning && canRun ? runSpeed : walkSpeed;
            currentSpeed *= normalizedDistance; // Умножаем скорость на дистанцию от центра

            // Применяем движение
            characterController.SimpleMove(moveDirection * currentSpeed);

            // Плавный поворот персонажа в направлении движения
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // Если джойстик не активен, останавливаем персонажа
            characterController.SimpleMove(Vector3.zero);
        }
    }

    private void UpdateAnimations()
    {
        if (characterController.velocity.magnitude > 0.1f)
        {
            // Изменяем анимацию в зависимости от состояния бега
            animationManager.ChangeAnimation(isRunning && canRun ? "Run" : "Walk");
        }
        else
        {
            // Если персонаж не двигается, включаем анимацию покоя
            animationManager.ChangeAnimation("Idle");
        }
    }

    public void OnRunButtonPressed()
    {
        isRunning = true;
    }

    public void OnRunButtonReleased()
    {
        isRunning = false;
    }
}