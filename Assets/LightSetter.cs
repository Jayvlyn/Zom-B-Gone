using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSetter : MonoBehaviour
{
    public Light2D globalLight;

    void Start()
    {
        globalLight.color = GameManager.currentZone.globalLightColor;
    }
}
