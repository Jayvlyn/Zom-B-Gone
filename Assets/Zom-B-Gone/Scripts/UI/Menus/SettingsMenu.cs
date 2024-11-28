using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public MainMenu mainMenu;
    public AudioMixer sfxMixer;
    public AudioMixer musicMixer;
    public TMPro.TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    private Resolution[] _resolutions;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;

    private bool settingsJustOpened = false;

    // Settings data
    [NonSerialized] public float sfxVolume;
    [NonSerialized] public float musicVolume;

    private void OnEnable()
    {
        StartCoroutine(openTimer());

        SettingsData settingsData = SaveSystem.LoadSettings();
        if (settingsData != null)
        {
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = settingsData.sfxVolume;
            }
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = settingsData.musicVolume;
            }
        }

        fullscreenToggle.isOn = Screen.fullScreen;

        int currentResIndex = 0;
        _resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> stringRes = new List<string>();
        for (int i = 0; i < _resolutions.Length; i++)
        {
            // Check if the resolution has a 16:9 aspect ratio
            if (Mathf.Approximately((float)_resolutions[i].width / _resolutions[i].height, 16f / 9f))
            {
                stringRes.Add($"{_resolutions[i].width} X {_resolutions[i].height} {(int)(_resolutions[i].refreshRateRatio.value)}hz");
                if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResIndex = i;
                }
            }
        }
        resolutionDropdown.AddOptions(stringRes);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetRes(int index)
    {
        if (!settingsJustOpened)
        {
            Resolution resolution = _resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
    public void OnBack()
    {
        gameObject.SetActive(false);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        if (value == sfxVolumeSlider.minValue) sfxMixer.SetFloat("Volume", -80);
        else sfxMixer.SetFloat("Volume", value);
        SaveSystem.SaveSettings(this);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        if (value == musicVolumeSlider.minValue) musicMixer.SetFloat("Volume", -80);
        else musicMixer.SetFloat("Volume", value);
        SaveSystem.SaveSettings(this);
    }

    public void SetFullScreen(bool toggle)
    {
        Screen.fullScreen = toggle;
    }

    private IEnumerator openTimer()
    {
        settingsJustOpened = true;
        yield return new WaitForSecondsRealtime(0.2f);
        settingsJustOpened = false;
    }
}

