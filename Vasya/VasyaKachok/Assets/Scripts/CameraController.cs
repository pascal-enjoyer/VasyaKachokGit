using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float followSharpness = 0.1f;
    [SerializeField] private float verticalOffset = 10f;
    [SerializeField] private float horizontalDistance = 5f;

    private Vector3 currentVelocity;
    private Vector3 baseOffset;
    private Vector3 targetOffset;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        // ������������ ��������� �������� �� ������ ������� ������� ������
        baseOffset = mainCamera.transform.position - target.position;
        targetOffset = baseOffset;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // ������� ��������� �������� ��� ��������
        targetOffset = Vector3.SmoothDamp(
            targetOffset,
            baseOffset.normalized * horizontalDistance + Vector3.up * verticalOffset,
            ref currentVelocity,
            followSharpness
        );

        // ������� ������� ������� � ������������ ���������
        Vector3 targetPosition = target.position + targetOffset;

        // ������� ����������� ������
        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            targetPosition,
            followSharpness * Time.deltaTime * 20f
        );
    }

    public void SetCameraParams(Transform newTarget, float newSharpness, float newVerticalOffset, float newHorizontalDistance)
    {
        target = newTarget;
        followSharpness = newSharpness;
        verticalOffset = newVerticalOffset;
        horizontalDistance = newHorizontalDistance;
    }
}