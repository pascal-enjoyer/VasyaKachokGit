using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStaminaData", menuName = "Scriptable Objects/PlayerStaminaData")]
public class PlayerStaminaData : ScriptableObject
{
    
    [Header("Стамина")]
    public float maxStamina = 100f;
    public float staminaRegenRate = 15f;
    public float staminaDrainRate = 25f;
    [HideInInspector] public float currentStamina;
}
