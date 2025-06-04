using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public enum EnemyType { Melee, Ranged, Special }

    [Header("General")]
    public string enemyName;
    public EnemyType enemyType;
    public GameObject enemyPrefab;
    public int maxHealth = 100;
    public float moveSpeed = 3f;
    public float chaseRange = 10f;

    [Header("Combat")]
    public int baseDamage = 10;
    public float attackRange = 2f;
    public float attackSpeed = 1f;
    public WeaponData startWeapon;
    public float preferredDistance = 2f;
}