using System.Collections;
using System.Collections.Generic;
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
	}


	public void StartRun(string run)
	{
		if (marketData)
		{
			marketData.Day++;
		}
		circleAnimator.SetTrigger("CloseCircle");
		StartCoroutine(sceneChangeDelay(run));	
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
