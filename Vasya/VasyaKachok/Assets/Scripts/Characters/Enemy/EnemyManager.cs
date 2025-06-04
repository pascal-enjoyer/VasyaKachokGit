using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    private readonly List<EnemyBase> activeEnemies = new List<EnemyBase>();

    public void RegisterEnemy(EnemyBase enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyBase enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public void SpawnEnemy(EnemyData data, Vector3 position)
    {
        if (data.enemyPrefab == null) return;

        GameObject enemyGO = Instantiate(data.enemyPrefab, position, Quaternion.identity, transform);
        EnemyBase enemy = enemyGO.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.SetData(data);
            RegisterEnemy(enemy);
        }
    }
}