using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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


	public TMP_Text text;

	bool gate0 = false;
	bool gate1 = false;
	bool gate2 = false;
	bool gate3 = false;
	bool gate4 = false;

    //private void Update()
    //{
    //	if (text) text.text = gate0 + " " + gate1 + " " + gate2 + " " + gate3 + " " + gate4;
    //	Debug.Log(melody1Source.volume + " " + melody2Source.volume + " " + melody3Source.volume);

    //}

  //  private void Update()
  //  {
		//Debug.Log(melody1Source.volume);
  //  }

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
				ChangeIntensity(1, 5);
			}
			else if (aggroEnemies < 5)
			{
				ChangeIntensity(2, 5);
			}
			else
			{
				ChangeIntensity(3, 5);
			}
		}
	}

	private void Start()
	{
		gate0 = true;
		instance = this;
		currentIntensity = -1;
		aggroEnemies = 0;
		if (SceneManager.GetActiveScene().name == "Game")
		{
			SetZoneTrack(GameManager.currentZone.track);
		}
		else
		{
			UpdateAudioSources();
		}
	}


	public void SetZoneTrack(ZoneTrack track)
	{
		gate1 = true;
		currentZoneTrack = track;
		UpdateAudioSources();
	}

	private void PlayAudioSources()
	{
		gate3 = true;
		melody1Source.Play();
		melody2Source.Play();
		melody3Source.Play();

		drum1Source.Play(); 
		drum2Source.Play();
		drum3Source.Play();
		gate4 = true;
		ChangeIntensity(1, 0.1f);
	}

    private void UpdateAudioSources()
    {
		gate2 = true;
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

		if(lerpRoutine != null)
		{
			StopCoroutine(lerpRoutine);
		}

		switch(level)
		{
			case 1:
				lerpRoutine = StartCoroutine(LerpIntensity(1, changeTime));
				//InstantChangeIntensity(1);
				break;
			case 2:
				lerpRoutine = StartCoroutine(LerpIntensity(2, changeTime));
				//InstantChangeIntensity(2);
				break;
			case 3:
				lerpRoutine = StartCoroutine(LerpIntensity(3, changeTime));
				//InstantChangeIntensity(3);
				break;
			default:
				//Debug.Log("Invalid intesntiy level");
				return;

		}
	}

	private void OnDisable()
	{
		if (lerpRoutine != null)
		{
			StopCoroutine(lerpRoutine);
		}
	}

	private Coroutine lerpRoutine;

	private IEnumerator LerpIntensity(int level, float duration)
	{
		AudioSource muting1Source = null;
		AudioSource muting2Source = null;		
		AudioSource muting3Source = null;
		AudioSource muting4Source = null;
		AudioSource unmuting1Source = null;
		AudioSource unmuting2Source = null;

		// set the sources that will unmute
		switch (level)
		{
			case 1:
				unmuting1Source = melody1Source;
				unmuting2Source = drum1Source;

				muting1Source = melody2Source;
				muting2Source = drum2Source;
				muting3Source = melody3Source;
				muting4Source = drum3Source;


				break;
			case 2:
				unmuting1Source = melody2Source;
				unmuting2Source = drum2Source;

				muting1Source = melody1Source;
				muting2Source = drum1Source;
				muting3Source = melody3Source;
				muting4Source = drum3Source;

				break;
			case 3:
				unmuting1Source = melody3Source;
				unmuting2Source = drum3Source;

				muting1Source = melody1Source;
				muting2Source = drum1Source;
				muting3Source = melody2Source;
				muting4Source = drum2Source;

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
			if(muting3Source != null) muting3Source.volume = Mathf.Lerp(muting3Source.volume, 0, elapsedTime / duration);
			if(muting4Source != null) muting4Source.volume = Mathf.Lerp(muting4Source.volume, 0, elapsedTime / duration);

			float volume = 0.5f;
			if(level != 1 || SceneManager.GetActiveScene().name != "Game")
			{
				volume = 1;
			}
			if(unmuting1Source != null) unmuting1Source.volume = Mathf.Lerp(unmuting1Source.volume, volume, elapsedTime / duration);
			if(unmuting2Source != null) unmuting2Source.volume = Mathf.Lerp(unmuting2Source.volume, volume, elapsedTime / duration);

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		if (muting1Source != null) muting1Source.volume = 0;
		if (muting2Source != null) muting2Source.volume = 0;
		if (muting3Source != null) muting3Source.volume = 0;
		if (muting4Source != null) muting4Source.volume = 0;

		if (unmuting1Source != null) unmuting1Source.volume = 1;
		if (unmuting2Source != null) unmuting2Source.volume = 1;

		currentIntensity = level;
	}

	private void InstantChangeIntensity(int level)
	{
		AudioSource muting1Source = null;
		AudioSource muting2Source = null;
		AudioSource unmuting1Source = null;
		AudioSource unmuting2Source = null;


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
				return;

		}

		if (muting1Source != null) muting1Source.volume = 0;
		if (muting2Source != null) muting2Source.volume = 0;

		if (unmuting1Source != null) unmuting1Source.volume = 1;
		if (unmuting2Source != null) unmuting2Source.volume = 1;

		currentIntensity = level;
	}

}
