using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSmokeHandler : MonoBehaviour
{
    private float particleEmissionRate = 0;

    public Vehicle vehicle;
    public ParticleSystem particleSystemSmoke;
    private ParticleSystem.EmissionModule particleSystemEmissionModule;

    private void Awake()
    {
        particleSystemEmissionModule = particleSystemSmoke.emission;
        particleSystemEmissionModule.rateOverTime = 0;
    }

    private void Update()
    {
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        particleSystemEmissionModule.rateOverTime = particleEmissionRate;

        if(vehicle.IsTireScreeching(out float lateralVelocity, out bool isBraking))
        {
            if (isBraking) particleEmissionRate = 30;

            else particleEmissionRate = Mathf.Abs(lateralVelocity) * 3;
        }
    }
}
