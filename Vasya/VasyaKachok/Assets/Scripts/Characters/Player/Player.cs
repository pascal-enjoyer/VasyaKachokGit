using UnityEngine;



// Пример реализации игрока как цели
public class Player : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    private float health;

    private void Awake()
    {
        health = maxHealth;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Max(0, health - damage);
        if (health <= 0)
        {
            Debug.Log("Player defeated!");
        }
    }


}