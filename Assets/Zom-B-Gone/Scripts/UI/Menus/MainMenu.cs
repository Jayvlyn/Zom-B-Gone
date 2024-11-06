using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsUI;
    public GameObject title;

    public AudioMixer sfxMixer;
    public AudioMixer musicMixer;

    private void Start()
	{
        StartCoroutine(TitleDelay());
        SettingsData settingsData = SaveSystem.LoadSettings();
        if (settingsData != null)
        {
            musicMixer.SetFloat("Volume", settingsData.musicVolume);
            sfxMixer.SetFloat("Volume", settingsData.sfxVolume);
        }
        else
        {
            musicMixer.SetFloat("Volume", -10);
            sfxMixer.SetFloat("Volume", -10);
        }
    }

    private IEnumerator TitleDelay()
    {
        yield return new WaitForSeconds(0.2f);
        title.SetActive(true);
    }

	#region BUTTON EVENTS
	public void OnPlay()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnSettings()
    {
        settingsUI.SetActive(true);
    }

    public void OnQuit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    #endregion
}
