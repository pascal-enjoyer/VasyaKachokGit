using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int health;
    public int baseDamage;
    public string playerName;

    public PlayerStaminaData playerStamina;


}
