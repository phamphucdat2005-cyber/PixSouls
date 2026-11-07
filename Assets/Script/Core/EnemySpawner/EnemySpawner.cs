using UnityEngine;
using System;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public string prefabID;

    public float spawnInterval = 5f;

    public int maxEnemies = 10;

    public float spawnRadius = 10f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    private void SpawnEnemy()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxEnemies)
            return;

        Vector2 randomPos =
            (Vector2)transform.position +
            UnityEngine.Random.insideUnitCircle * spawnRadius;

        GameObject enemy = Instantiate(
            enemyPrefab,
            randomPos,
            Quaternion.identity);

        EnemyHealth health = enemy.GetComponent<EnemyHealth>();

        if (health != null)
        {
            health.enemyID = Guid.NewGuid().ToString();
            health.prefabID = prefabID;
        }
    }
}