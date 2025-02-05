using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int health;
    public int baseDamage;
    public GameObject enemyPrefab;

}
