using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public Transform enemyPrefab;
	public int numberOfEnemies;
	public float spawnRadius;
	public List<Enemy> enemies;
	private PlayerController player;

	[Header("Interval Spawning")]
	[SerializeField] private bool intervalSpawning = false;
	[SerializeField,Min(0.1f)] private float intervalTime = 5.00f;
	[SerializeField] private int intervalSpawnAmount = 2;
	private float intervalTimer;

	private void Start()
	{
		enemies = new List<Enemy>();
		player = FindObjectOfType<PlayerController>();

		Spawn(enemyPrefab, numberOfEnemies);

		enemies.AddRange(FindObjectsOfType<Enemy>());
	}

    private void Update()
    {
		IntervalSpawning();
    }

	void IntervalSpawning()
	{
        if (intervalTimer > 0)
        {
            intervalTimer -= Time.deltaTime;
        }
        else
        {
            Spawn(enemyPrefab, intervalSpawnAmount);
			intervalTimer = intervalTime;
        }
    }

    void Spawn(Transform prefab, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Instantiate(prefab, new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius)), Quaternion.identity);
		}
	}

	public List<Enemy> GetNeighbors(Enemy enemy, float radius)
	{
		List<Enemy> neighborsFound = new List<Enemy>();
		foreach (var otherEnemy in enemies)
		{
			if (otherEnemy == enemy) continue;

			if (Vector3.Distance(enemy.transform.position, otherEnemy.transform.position) <= radius)
			{
				neighborsFound.Add(otherEnemy);
			}
		}

		return neighborsFound;
	}
}
