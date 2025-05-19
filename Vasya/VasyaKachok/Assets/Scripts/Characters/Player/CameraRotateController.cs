using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;

public class CameraRotateController : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public CameraController cameraController;

    public void OnPointerDown(PointerEventData eventData)
    {
        cameraController.OnPointerDown(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        cameraController.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cameraController.OnEndDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cameraController.OnPointerUp(eventData);
    }
}
