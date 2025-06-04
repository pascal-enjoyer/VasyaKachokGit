using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private List<EnemyData> enemyDataList;
    [SerializeField] private List<Transform> spawnPoints;

    public void SpawnEnemy()
    {
        if (enemyDataList.Count == 0 || spawnPoints.Count == 0) return;

        EnemyData data = enemyDataList[Random.Range(0, enemyDataList.Count)];
        Vector3 position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        enemyManager.SpawnEnemy(data, position);
    }
}