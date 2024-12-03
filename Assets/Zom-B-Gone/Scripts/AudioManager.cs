using UnityEngine;
using Guymon;
using Guymon.DesignPatterns;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource audioSource;

    public void Play(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
        //audioSource.resource = audioClip;
        //audioSource.Play();
    }
}
