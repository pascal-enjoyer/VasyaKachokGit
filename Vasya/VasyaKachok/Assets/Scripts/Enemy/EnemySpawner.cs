using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    EnemyManager enemyManager;

    public List<EnemyData> enemyVariables;

    public void SpawnEnemy()
    {
        Instantiate(enemyVariables[Random.Range(0, enemyVariables.Count)].enemyPrefab, 
            new Vector3(Random.Range(-10, 10), 0.1f, Random.Range(-10, 10)), 
            Quaternion.identity, transform);
    }
}
