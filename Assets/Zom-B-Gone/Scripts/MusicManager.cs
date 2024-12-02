using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager instance;

    public ZoneTrack currentZoneTrack;
    public AudioSource melody1Source;
    public AudioSource melody2Source;
    public AudioSource melody3Source;
    public AudioSource drum1Source;
    public AudioSource drum2Source;
    public AudioSource drum3Source;

	public int currentIntensity = -1;
	private int aggroEnemies = 0;
	public int AggroEnemies
	{
		get { return aggroEnemies; }
		set
		{
			aggroEnemies = value;

			if(aggroEnemies < 3)
			{
				ChangeIntensity(1);
			}
			else if (aggroEnemies < 10)
			{
				ChangeIntensity(2);
			}
			else
			{
				ChangeIntensity(3);
			}
		}
	}

	private void Start()
	{
		instance = this;
		currentIntensity = -1;
		aggroEnemies = 0;
		UpdateAudioSources();
	}


	public void SetZoneTrack(ZoneTrack track)
	{
		currentZoneTrack = track;
		UpdateAudioSources();
	}

	private void PlayAudioSources()
	{
		melody1Source.Play();
		melody2Source.Play();
		melody3Source.Play();

		drum1Source.Play(); 
		drum2Source.Play();
		drum3Source.Play();
		ChangeIntensity(1, 0.1f);
	}

    private void UpdateAudioSources()
    {
		UpdateSource(melody1Source, currentZoneTrack.melodyIntensity1);
		UpdateSource(melody2Source, currentZoneTrack.melodyIntensity2);
		UpdateSource(melody3Source, currentZoneTrack.melodyIntensity3);
		UpdateSource(drum1Source, currentZoneTrack.drumsIntensity1);
		UpdateSource(drum2Source, currentZoneTrack.drumsIntensity2);
		UpdateSource(drum3Source, currentZoneTrack.drumsIntensity3);
		PlayAudioSources();
    }

    private void UpdateSource(AudioSource source, AudioClip clip)
    {
		if (clip != null)
		{
			source.resource = clip;
		}
		else
		{
			source.resource = null;
		}
	}

	/// <summary>
	/// int 1-3 for intensity
	/// </summary>
	/// <param name="level"></param>
	public void ChangeIntensity(int level, float changeTime = 5)
	{
		if(level == currentIntensity)
		{
			//Debug.Log("Same intensity given");
			return;
		}
		switch(level)
		{
			case 1:
				StartCoroutine(LerpIntensity(1, changeTime));
				break;
			case 2:
				StartCoroutine(LerpIntensity(2, changeTime));
				break;
			case 3:
				StartCoroutine(LerpIntensity(3, changeTime));
				break;
			default:
				//Debug.Log("Invalid intesntiy level");
				return;

		}
	}

	private IEnumerator LerpIntensity(int level, float duration)
	{
		AudioSource muting1Source = null;
		AudioSource muting2Source = null;
		AudioSource unmuting1Source = null;
		AudioSource unmuting2Source = null;

		// set the sources that will mute
		switch(currentIntensity)
		{
			case 1:
				muting1Source = melody1Source;
				muting2Source = drum1Source;


				break;
			case 2:
				muting1Source = melody2Source;
				muting2Source = drum2Source;

				break;
			case 3:
				muting1Source = melody3Source;
				muting2Source = drum3Source;

				break;
			default:
				break;
		}

		// set the sources that will unmute
		switch (level)
		{
			case 1:
				unmuting1Source = melody1Source;
				unmuting2Source = drum1Source;


				break;
			case 2:
				unmuting1Source = melody2Source;
				unmuting2Source = drum2Source;

				break;
			case 3:
				unmuting1Source = melody3Source;
				unmuting2Source = drum3Source;

				break;
			default:
				Debug.Log("Invalid intesntiy level");
				yield break;

		}


		float elapsedTime = 0;
		while (elapsedTime < duration)
		{
			if(muting1Source != null) muting1Source.volume = Mathf.Lerp(muting1Source.volume, 0, elapsedTime / duration);
			if(muting2Source != null) muting2Source.volume = Mathf.Lerp(muting2Source.volume, 0, elapsedTime / duration);

			if(unmuting1Source != null) unmuting1Source.volume = Mathf.Lerp(unmuting1Source.volume, 1, elapsedTime / duration);
			if(unmuting2Source != null) unmuting2Source.volume = Mathf.Lerp(unmuting2Source.volume, 1, elapsedTime / duration);

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		if (muting1Source != null) muting1Source.volume = 0;
		if (muting2Source != null) muting2Source.volume = 0;

		if (unmuting1Source != null) unmuting1Source.volume = 1;
		if (unmuting2Source != null) unmuting2Source.volume = 1;

		currentIntensity = level;
	}



}
