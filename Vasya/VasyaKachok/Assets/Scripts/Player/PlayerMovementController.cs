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
        // Получаем ввод от джойстика
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Получаем направление камеры
            Quaternion cameraRotation = thirdPersonCamera.GetCameraRotation();
            // Преобразуем ввод в направление относительно камеры
            Vector3 moveDirection = cameraRotation * direction;
            moveDirection.y = 0;

            // Устанавливаем скорость
            currentSpeed = isRunning ? runSpeed : walkSpeed;
            // Двигаем персонажа
            characterController.SimpleMove(moveDirection * currentSpeed);

            // Поворот персонажа в сторону движения
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // Если ввода нет, останавливаем персонажа
            characterController.SimpleMove(Vector3.zero);
        }
    }

    private void UpdateAnimations()
    {
        if (characterController.velocity.magnitude > 0.1f)
        {
            animationManager.ChangeAnimation(isRunning ? "Run" : "Walk");
        }
        else
        {
            animationManager.ChangeAnimation("Idle");
        }
    }

    public void OnRunButtonPressed() => isRunning = true;
    public void OnRunButtonReleased() => isRunning = false;

}