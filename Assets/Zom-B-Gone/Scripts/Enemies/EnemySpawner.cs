using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;
    public GameObject[] enemyPrefabs;
    public float spawnIntervalTime = 5;
    public float enemiesPerWave = 3;
    public float spawnDistance = 16;
    public float spawnRange = 30;

    private void Start()
    {
        StartCoroutine(SpawnTick());
    }

    private IEnumerator SpawnTick()
    {
        while (true)
        {
            SpawnWave();

            yield return new WaitForSeconds(spawnIntervalTime);
        }
    }

    private void SpawnWave()
    {
        for(int i = 0; i < enemiesPerWave; i++)
        {
            SpawnRandomEnemy();
        }
    }

    private void SpawnRandomEnemy()
    {
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        float randomDistance = Random.Range(spawnDistance, spawnRange);

        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        Vector2 spawnPosition = (Vector2)player.position + new Vector2(randomDirection.x, randomDirection.y) * randomDistance;
        if (IsPositionBlocked(spawnPosition))
        {
            for (int i = 0; i < 11; i++)
            {
                spawnPosition.y += 1;

                if (!IsPositionBlocked(spawnPosition))
                {
                    break; 
                }
            }
            return;
        }

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private bool IsPositionBlocked(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.5f);

        if (hit != null)// && hit.attachedRigidbody != null)
        {
            return true;
        }

        return false;
    }
}
