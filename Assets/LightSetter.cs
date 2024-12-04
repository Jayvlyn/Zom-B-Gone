using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSetter : MonoBehaviour
{
    public Light2D globalLight;
    public DayNightCycle dnc;

    void Start()
    {
        globalLight.color = GameManager.currentZone.globalLightColor;
        dnc.middayIntensity = GameManager.currentZone.middayIntensity;
        dnc.midnightIntensity = GameManager.currentZone.midnightIntensity;
    }
}
