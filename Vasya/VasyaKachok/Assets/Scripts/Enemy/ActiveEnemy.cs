using UnityEngine;

public class ActiveEnemy : MonoBehaviour
{
    public EnemyData enemyData;

    public void InitializeEnemy(EnemyData enemyData)
    {
        this.enemyData = enemyData;
    }

    private void Update()
    {
        if (enemyData != null)
        {

            DoSomething();
        }
    }

    public void DoSomething()
    {
        Debug.Log(enemyData.enemyName);
        transform.position = new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10));
    }
}
