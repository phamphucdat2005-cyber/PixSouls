using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float spawnInterval = 5f;

    public int maxEnemies = 10;

    public float spawnRadius = 10f;

    private void Start()
    {
        InvokeRepeating(
            nameof(SpawnEnemy),
            1f,
            spawnInterval);
    }

    private void SpawnEnemy()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxEnemies)
            return;

        Vector2 randomPos =
            (Vector2)transform.position +
            Random.insideUnitCircle * spawnRadius;

        Instantiate(
            enemyPrefab,
            randomPos,
            Quaternion.identity);
    }
}