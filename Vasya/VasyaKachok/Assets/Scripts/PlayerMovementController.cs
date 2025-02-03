using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AnchoredJoystick joystick;
    [SerializeField] private PlayerAnimationManager animationManager;

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
        // Получение ввода
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // Расчет скорости
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Движение и поворот
        if (direction.magnitude >= 0.1f)
        {
            Vector3 moveVector = direction * currentSpeed;
            characterController.SimpleMove(moveVector);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
            characterController.SimpleMove(Vector3.zero);
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