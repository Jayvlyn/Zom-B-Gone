using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private PlayerController player;
	public static Light2D globalLight;

	[HideInInspector] public static LootTable currentZoneLootTable;

	[Header("MUST REFERENCE TO PRESERVE DATA")]
	public MarketData marketData;
	public FloorContainerData floorData;

	private void Start()
	{
		globalLight = GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<Light2D>();
		player = FindFirstObjectByType<PlayerController>();
	}


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
