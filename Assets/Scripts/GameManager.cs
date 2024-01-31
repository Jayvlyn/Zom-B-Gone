using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public Transform enemyPrefab;
	public int numberOfEnemies;
	public float spawnRadius;
	private List<Enemy> enemies;
	private PlayerController player;

	private void Start()
	{
		enemies = new List<Enemy>();
		player = FindObjectOfType<PlayerController>();

		Spawn(enemyPrefab, numberOfEnemies);

		enemies.AddRange(FindObjectsOfType<Enemy>());
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
