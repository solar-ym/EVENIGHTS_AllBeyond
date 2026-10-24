using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject projectileHolder;
    public GameObject enemyToSpawn;
    public GameObject spawnPoint;
    public float spawnTimer = 10.0f;

    // Timers
    private float time = 0.0f;

    public void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyToSpawn, this.transform);

        enemy.GetComponent<Enemy>().spawnPoint = spawnPoint;
        enemy.GetComponent<Enemy>().projectileHolder = projectileHolder;
    }

    private void Update()
    {
        time += Time.deltaTime; 

        if (time >= spawnTimer)
        {
            SpawnEnemy();
            time = 0.0f;
        }
    }
}
