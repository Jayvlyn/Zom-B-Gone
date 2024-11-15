using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Explosion : MonoBehaviour
{
    public Light2D light;
    public CinemachineImpulseSource cis;
    public AudioSource audioSource;


	public void OnAnimEnd()
    {
        Destroy(gameObject);
    }

    public void ShrinkRadius()
    {
        shrinkRadius = true;
    }

    bool shrinkRadius = false;
    private void Update()
    {
        if(!shrinkRadius) light.pointLightOuterRadius += Time.deltaTime * 100;
        else              light.pointLightOuterRadius -= Time.deltaTime * 200;
    }
}
