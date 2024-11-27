using System.Collections;
using System.Drawing.Text;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;
    public GameObject[] enemyPrefabs;
    private float currentEnemiesPerMinute = 1;
    private float spawnInterval;
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
            TrySpawnEnemy();
            CalculateInterval();
			yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void CalculateInterval()
    {
		float gameTimeInMinutes = Time.timeSinceLevelLoad / 60;
		if (gameTimeInMinutes <= 10)
		{
            currentEnemiesPerMinute = GameManager.currentZone.spawnRate.Evaluate(gameTimeInMinutes);
			spawnInterval = 60 / currentEnemiesPerMinute;
            if (DayNightCycle.isNight) spawnInterval *= 2;
		}
	}

    private void TrySpawnEnemy()
    {
		if (Optimizer.currentActiveEnemies < Optimizer.maxActiveEnemies)
		{
			SpawnRandomEnemy();
		}
	}

    private void SpawnWave()
    {
        int enemiesPerWave = 4;
        for (int i = 0; i < enemiesPerWave; i++)
        {
            if (Optimizer.currentActiveEnemies < Optimizer.maxActiveEnemies)
            {
                SpawnRandomEnemy();
            }
            else
            {
                return;
            }
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

        Optimizer.list.Add(Instantiate(enemyPrefab, spawnPosition, Quaternion.identity));
        Optimizer.currentActiveEnemies++;

        
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
