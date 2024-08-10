using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public float sfxVolume;
    public float musicVolume;

    public SettingsData(SettingsMenu settings)
    {
        sfxVolume = settings.sfxVolume;
        musicVolume = settings.musicVolume;
    }

    public SettingsData() {}
}
