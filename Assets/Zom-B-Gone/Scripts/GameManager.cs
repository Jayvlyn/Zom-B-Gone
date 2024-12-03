using CodeMonkey;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private PlayerController player;
	public static Light2D globalLight;
	public static bool checkZoneUnlock;
	public Animator circleAnimator;

	public static ZoneData currentZone;

	public TMP_Text marketDaysText;

	[Header("MUST REFERENCE TO PRESERVE DATA")]
	public MarketData marketData;
	public FloorContainerData floorData;
	public PlayerData activePlayerData;

	private void Start()
	{
		globalLight = GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<Light2D>();
		player = FindFirstObjectByType<PlayerController>();
		Time.timeScale = 1f;
		PauseMenu.paused = false;

		UpdateMarketDaysText();
	}


	public void StartRun(string run)
	{
		if (marketData)
		{
			marketData.Day++;
			UpdateMarketDaysText();
		}
		circleAnimator.SetTrigger("CloseCircle");
		StartCoroutine(sceneChangeDelay(run));	
	}

	public void UpdateMarketDaysText()
	{
		if (marketDaysText != null)
		{
			marketDaysText.text = "Day: " + marketData.TotalDays;
			if (marketData.Day == marketData.daysPerCycle) marketDaysText.color = marketData.OneDayLeftColor;
			else if (marketData.Day + 1 == marketData.daysPerCycle) marketDaysText.color = marketData.TwoDaysLeftColor;
			else if (marketData.Day + 2 == marketData.daysPerCycle) marketDaysText.color = marketData.ThreeDaysLeftColor;
			else marketDaysText.color = Color.white;

		}
	}

	public void Extract()
	{
		circleAnimator.SetTrigger("CloseCircle");
		StartCoroutine(sceneChangeDelay("Unit"));
		checkZoneUnlock = true;
	}

	public static void SetZone(ZoneData zoneData)
	{
		currentZone = zoneData;
	}

	public IEnumerator sceneChangeDelay(string scene)
	{
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(scene);
	}
}
