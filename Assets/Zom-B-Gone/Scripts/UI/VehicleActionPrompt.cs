using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VehicleActionPrompt : MonoBehaviour
{
	[SerializeField] TMP_Text prompt;
	[SerializeField] Slider progressBar;
	private void OnEnable()
	{
		string sceneName = SceneManager.GetActiveScene().name;

		if (sceneName == "Unit") prompt.text = "Hold E to START RUN";
		else prompt.text = "Hold E to EXTRACT";
	}

	private void Update()
	{
		if (VehicleDriver.travelPercent > 0)
		{
			progressBar.gameObject.SetActive(true);
			progressBar.value = VehicleDriver.travelPercent;
		}
		else progressBar.gameObject.SetActive(false);


	}
}
