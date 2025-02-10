// PlayerMovementController.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AnchoredJoystick joystick;
    [SerializeField] private PlayerAnimationManager animationManager;
    [SerializeField] private CameraController thirdPersonCamera;
    [SerializeField] private StaminaManager staminaManager;

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
        staminaManager.OnStaminaDepleted.AddListener(HandleStaminaDepleted);
        staminaManager.OnStaminaRestored.AddListener(HandleStaminaRestored);
    }
    private void Update()
    {
        HandleMovement();
        UpdateAnimations();
        UpdateRunningState();
    }

    private void HandleMovement()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            Quaternion cameraRotation = thirdPersonCamera.GetCameraRotation();
            Vector3 moveDirection = cameraRotation * direction;
            moveDirection.y = 0;

            currentSpeed = isRunning && canRun ? runSpeed : walkSpeed;
            characterController.SimpleMove(moveDirection * currentSpeed);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            characterController.SimpleMove(Vector3.zero);
        }
    }

    private void UpdateRunningState()
    {
        if (isRunning && !staminaManager.CanRun)
        {
            isRunning = false;
            staminaManager.StopRunning();
        }
    }

    private void UpdateAnimations()
    {
        if (characterController.velocity.magnitude > 0.1f)
        {
            animationManager.ChangeAnimation(isRunning && canRun ? "Run" : "Walk");
        }
        else
        {
            animationManager.ChangeAnimation("Idle");
        }
    }

    private void HandleStaminaDepleted()
    {
        canRun = false;
        isRunning = false;
    }

    private void HandleStaminaRestored()
    {
        canRun = true;
    }

    public void OnRunButtonPressed()
    {
        if (staminaManager.CanRun)
        {
            isRunning = true;
            staminaManager.StartRunning();
        }
    }

    public void OnRunButtonReleased()
    {
        isRunning = false;
        staminaManager.StopRunning();
    }
}