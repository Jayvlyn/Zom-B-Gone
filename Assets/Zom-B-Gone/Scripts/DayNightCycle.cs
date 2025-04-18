using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
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
        currentTime = NoonHour * 60; // start at noon
	}

    bool clockPaused = false;

	private void Update()
    {
        //Debug.Log("Current Hour:" + CurrentHour);
        if (Input.GetKeyDown(KeyCode.Period))
        {
            clockPaused = !clockPaused;
        }

        if (Input.GetKey(KeyCode.Comma))
        {
            float mult = 10;
            if (Input.GetKey(KeyCode.RightShift)) mult = 100;
            currentTime -= Time.deltaTime * mult;
        }
        else if (Input.GetKey(KeyCode.Slash))
        {
            float mult = 10;
            if (Input.GetKey(KeyCode.RightShift)) mult = 100;
            currentTime += Time.deltaTime * mult;
        }
        else if (!clockPaused)
        {
		    currentTime += Time.deltaTime;
        }

        //Debug.Log(currentTime);


		//if (CurrentHour > TotalHours) currentTime = 0;

		if (Input.GetKeyDown(KeyCode.L))
		{
			currentTime = NoonHour * 60;
		}
        else if (Input.GetKeyDown(KeyCode.K))
        {
            currentTime = MidnightHour * 60;
        }


        UpdateLightIntensity();
	}

    // I figured out how the phases needed to work with lerping,
    // just didn't know the math for calculating T in each phase,
    // so GPT 4o helped me with that
    //private void UpdateLightIntensity()
    //{
    //	if(globalLight == null) return;

    //	// Calculate the time progression within each phase, normalized to [0, 1]
    //	float t;

    //	if (CurrentHour < NoonHour) // Dawn to Noon
    //	{
    //		t = CurrentHour / NoonHour; // Ranges from 0 to 1 as time progresses from dawn to noon
    //		globalLight.intensity = Mathf.Lerp(DawnIntensity, middayIntensity, t);
    //		isNight = false;
    //	}
    //	else if (CurrentHour < MidnightHour) // Noon to Midnight
    //	{
    //		t = (CurrentHour - NoonHour) / (MidnightHour - NoonHour); // Ranges from 0 to 1 from noon to midnight
    //		globalLight.intensity = Mathf.Lerp(middayIntensity, midnightIntensity, t);
    //		if (t >= 0.5) isNight = true;
    //		else isNight = false;
    //	}
    //	else // Midnight to Dawn
    //	{
    //		t = (CurrentHour - MidnightHour) / (TotalHours - MidnightHour); // Ranges from 0 to 1 from midnight to dawn
    //		globalLight.intensity = Mathf.Lerp(midnightIntensity, DawnIntensity, t);
    //		isNight = true;
    //	}
    //}


    private void UpdateLightIntensity()
    {
        if (globalLight == null) return;

        float t;
        float sunriseStart = 0; // Start of sunrise
        float sunriseEnd = sunriseStart + (nightHours * 0.2f);   // End of sunrise
        float sunsetStart = NoonHour + (dayHours * 0.8f);        // Start of sunset
        float sunsetEnd = sunsetStart + (dayHours * 0.2f);       // End of sunset

        if (CurrentHour >= sunriseEnd && CurrentHour < sunsetStart) // Full daylight period
        {
            //Debug.Log("1");
            globalLight.intensity = middayIntensity;
            isNight = false;
        }
        else if (CurrentHour >= sunsetStart && CurrentHour < sunsetEnd) // Sunset transition
        {
            //Debug.Log("2");
            t = (CurrentHour - sunsetStart) / (sunsetEnd - sunsetStart);
            globalLight.intensity = Mathf.Lerp(middayIntensity, midnightIntensity, t);
            isNight = t >= 0.5f;
        }
        else if (CurrentHour >= sunsetEnd || CurrentHour < sunriseStart) // Full nighttime period
        {
            //Debug.Log("3");
            globalLight.intensity = midnightIntensity;
            isNight = true;
        }
        else if (CurrentHour >= sunriseStart && CurrentHour < sunriseEnd) // Sunrise transition
        {
            //Debug.Log("4");
            t = (CurrentHour - sunriseStart) / (sunriseEnd - sunriseStart);
            globalLight.intensity = Mathf.Lerp(midnightIntensity, middayIntensity, t);
            isNight = false;
        }
    }

}
