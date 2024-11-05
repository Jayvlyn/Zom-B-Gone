using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public GameObject enemyPrefab;
	public int numberOfEnemies;
	public float spawnRadius;
	public List<Enemy> enemies;
	private PlayerController player;
	public static Light2D globalLight;

	[HideInInspector] public static LootTable currentZoneLootTable;

	[Header("Interval Spawning")]
	[SerializeField] private bool intervalSpawning = false;
	[SerializeField,Min(0.1f)] private float intervalTime = 5.00f;
	[SerializeField] private int intervalSpawnAmount = 2;
	private float intervalTimer;

	[Header("MUST REFERENCE TO PRESERVE DATA")]
	public MarketData marketData;
	public FloorContainerData floorData;

	private void Start()
	{
		globalLight = GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<Light2D>();
		enemies = new List<Enemy>();
		player = FindFirstObjectByType<PlayerController>();

		Spawn(enemyPrefab, numberOfEnemies);

		enemies.AddRange(FindObjectsOfType<Enemy>());
	}

    private void Update()
    {
		if(intervalSpawning)IntervalSpawning();
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

    void Spawn(GameObject prefab, int count)
	{
		for (int i = 0; i < count; i++)
		{
			GameObject enemy = Instantiate(prefab, new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius)), Quaternion.identity);
			Optimizer.list.Add(enemy);
		}
	}


	// terrible method from tutorial
	//public List<Enemy> GetNeighbors(Enemy enemy, float radius)
	//{
	//	List<Enemy> neighborsFound = new List<Enemy>();
	//	foreach (var otherEnemy in enemies)
	//	{
	//		if (otherEnemy == enemy) continue;

	//		if (Vector3.Distance(enemy.transform.position, otherEnemy.transform.position) <= radius)
	//		{
	//			neighborsFound.Add(otherEnemy);
	//		}
	//	}

	//	return neighborsFound;
	//}

	// Add extra function to these bottom two later

	public void StartRun(string run)
	{
		if (marketData)
		{
			marketData.Day++;
		}
		SceneManager.LoadScene(run);
	}

	public void Extract()
	{
		SceneManager.LoadScene("Unit");
	}

	public static void SetLootTable(LootTable lootTable)
	{
		currentZoneLootTable = lootTable;
	}
}
