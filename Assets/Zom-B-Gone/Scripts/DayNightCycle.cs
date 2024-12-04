using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
	[Header("References")]
	public Light2D globalLight;

	// game hour = minute
	[Header("Time Settings")]
    public int dayHours = 5;
    public int nightHours = 7;
    public int TotalHours => dayHours+nightHours;
	public float CurrentHour => (currentTime / 60) % TotalHours;
	public float NoonHour => dayHours * 0.5f;
	public float MidnightHour => dayHours + (nightHours * 0.5f);

	[Header("Color Settings")]
	public float middayIntensity = 1f;
	public float midnightIntensity = 0.13f;
	public float DawnIntensity => (middayIntensity + midnightIntensity) * 0.5f;

    [HideInInspector] public float currentTime;
	public static bool isNight;

	private void Start()
	{
        currentTime = 0;
	}

	private void Update()
	{
		currentTime += Time.deltaTime;
		if (CurrentHour > TotalHours) currentTime = 0;

		if (Input.GetKeyDown(KeyCode.L))
		{
			currentTime = 0;
		}

		UpdateLightIntensity();
	}

	// I figured out how the phases needed to work with lerping,
	// just didn't know the math for calculating T in each phase,
	// so GPT 4o helped me with that
	private void UpdateLightIntensity()
	{
		if(globalLight == null) return;

		// Calculate the time progression within each phase, normalized to [0, 1]
		float t;

		if (CurrentHour < NoonHour) // Dawn to Noon
		{
			t = CurrentHour / NoonHour; // Ranges from 0 to 1 as time progresses from dawn to noon
			globalLight.intensity = Mathf.Lerp(DawnIntensity, middayIntensity, t);
			isNight = false;
		}
		else if (CurrentHour < MidnightHour) // Noon to Midnight
		{
			t = (CurrentHour - NoonHour) / (MidnightHour - NoonHour); // Ranges from 0 to 1 from noon to midnight
			globalLight.intensity = Mathf.Lerp(middayIntensity, midnightIntensity, t);
			if (t >= 0.5) isNight = true;
			else isNight = false;
		}
		else // Midnight to Dawn
		{
			t = (CurrentHour - MidnightHour) / (TotalHours - MidnightHour); // Ranges from 0 to 1 from midnight to dawn
			globalLight.intensity = Mathf.Lerp(midnightIntensity, DawnIntensity, t);
			isNight = true;
		}
	}
}
