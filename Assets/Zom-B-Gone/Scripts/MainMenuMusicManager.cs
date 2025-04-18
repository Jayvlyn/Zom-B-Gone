using UnityEngine;

public class MainMenuMusicManager : MonoBehaviour
{
    public AudioSource introSource;
    public AudioSource loopSource;

	private void Start()
	{
		Invoke(nameof(PlaySecondAudio), introSource.clip.length - 0.1f);
	}

	private void PlaySecondAudio()
	{
		loopSource.Play();
	}
}