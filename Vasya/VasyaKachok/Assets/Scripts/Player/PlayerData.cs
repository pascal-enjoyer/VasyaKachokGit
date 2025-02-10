using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int health;
    public int baseDamage;
    public string playerName;

    [Header("Стамина")]
    public float maxStamina = 100f;
    public float staminaRegenRate = 15f;
    public float staminaDrainRate = 25f;
    [HideInInspector] public float currentStamina;
}
