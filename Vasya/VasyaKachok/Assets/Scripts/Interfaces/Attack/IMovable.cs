using UnityEngine;

// Интерфейс для движения
public interface IMovable
{
    void Move(Vector3 direction, float speed);
}
