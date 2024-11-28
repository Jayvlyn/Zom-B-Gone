using System.Collections;
using UnityEngine;
using CodeMonkey;

public class EnemySpawner : MonoBehaviour
{
    private float currentEnemiesPerMinute = 1;
    private float spawnInterval;
    public static float spawnDistance = 16;
    public static float spawnRange = 30;

    private void Start()
    {
        StartCoroutine(SpawnTick());
    }

	private IEnumerator SpawnTick()
    {
        while (true)
        {
            if(Random.Range(0,8) == 0)
            {
                TrySpawnHorde(Utils.GetWeightedRandomNumber(3, 10));
            }
            else
            {
                TrySpawnEnemy();
            }

            CalculateInterval();
			yield return new WaitForSeconds(spawnInterval);
        }
    }

    public static void SpawnEnemiesAtPosition(Vector2 position)
    {
        float hordeRoll = 8;
        if(DayNightCycle.isNight)
        {
            hordeRoll *= 0.5f; // twice as likely for it to be larger horde at night
        }

        int adder = 0;
		if (Random.Range(0, (int)hordeRoll) == 0)
		{
            if(DayNightCycle.isNight)
            {
                adder = Random.Range(3, 6); // bigger hordes at night
            }
			TrySpawnHorde(Utils.GetWeightedRandomNumber(3 + adder, 10 + adder), position);
		}
		else
		{
			TrySpawnEnemy(position);
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

    public static void TrySpawnEnemy(Vector2 spawnPos = default)
    {
		if (Optimizer.currentActiveEnemies < Optimizer.maxActiveEnemies)
		{
			SpawnRandomEnemy(spawnPos);
		}
	}

    public static Vector2 FindRandomSpawnPosition()
    {
		float randomDistance = Random.Range(spawnDistance, spawnRange);
		Vector2 randomDirection = Random.insideUnitCircle.normalized;
		return (Vector2)PlayerController.instance.transform.position + new Vector2(randomDirection.x, randomDirection.y) * randomDistance;
	}

    public static void TrySpawnHorde(int count, Vector2 spawnPos = default)
    {
		Vector2 spawnPosition;
		if (spawnPos == default) spawnPosition = FindRandomSpawnPosition();
        else spawnPosition = spawnPos;
        

		for (int i = 0; i < count; i++)
        {
            TrySpawnEnemy(spawnPosition);
            spawnPosition += Random.insideUnitCircle.normalized;
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

    private static void SpawnRandomEnemy(Vector2 spawnPos = default)
    {
        GameObject enemyPrefab = Assets.i.enemyPrefabs[Random.Range(0, Assets.i.enemyPrefabs.Length)];

		Vector2 spawnPosition;
		if (spawnPos == default) spawnPosition = FindRandomSpawnPosition();
		else spawnPosition = spawnPos;

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

    private static bool IsPositionBlocked(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.3f);

        if (hit != null && hit.attachedRigidbody != null)
        {
            return true;
        }

        return false;
    }
}
