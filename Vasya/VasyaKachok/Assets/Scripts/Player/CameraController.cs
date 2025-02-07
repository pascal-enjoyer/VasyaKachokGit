
using UnityEngine;


public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target; // ���� (�����)
    [SerializeField] private Transform cameraTransform; // ��������� ������

    [Header("Settings")]
    [SerializeField] private float distance = 5f; // ��������� �� ������ �� ������
    [SerializeField] private float minDistance = 2f; // ����������� ���������
    [SerializeField] private float maxDistance = 10f; // ������������ ���������
    [SerializeField] private float rotationSpeed = 5f; // �������� �������� ������
    [SerializeField] private float followSpeed = 10f; // �������� ���������� ������
    [SerializeField] private float heightOffset = 2f; // ������ ������ ������������ ������

    private float currentX = 0f; // ������� ���� �������� �� �����������
    private float currentY = 30f; // ������� ���� �������� �� ���������

    private void LateUpdate()
    {
        if (target == null) return;
        
        // �������� ������ ������ ������
        HandleCameraRotation();
        // ���������� ������� ������
        UpdateCameraPosition();
    }

    private void HandleCameraRotation()
    {
        // �������������� �������� ������, ���� ����� ��������
        if (target.GetComponent<CharacterController>().velocity.magnitude > 0.1f)
        {
            // ��������� ����������� �������� ������
            Vector3 movementDirection = target.GetComponent<CharacterController>().velocity.normalized;
            // ��������� ������� ���� �������� ������
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
            // ������ ������������� ������� ���� � ��������
            currentX = Mathf.LerpAngle(currentX, targetAngle, rotationSpeed * Time.deltaTime);
        }

        
    }

    private void UpdateCameraPosition()
    {
        // ��������� ������� ������ �� ������ ����� � ���������
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 offset = new Vector3(0, heightOffset, -distance);
        Vector3 desiredPosition = target.position + rotation * offset;

        // ������� ����������� ������
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPosition, followSpeed * Time.deltaTime);
        // ������ ������ ������� �� ������
        cameraTransform.LookAt(target.position + Vector3.up * heightOffset);
    }

    // ����� ��� ��������� �������� ���� �������� ������ (��� ���������� �������)
    public Quaternion GetCameraRotation()
    {
        return Quaternion.Euler(0, currentX, 0);
    }
}