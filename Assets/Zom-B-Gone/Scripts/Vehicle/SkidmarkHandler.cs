using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidmarkHandler : MonoBehaviour
{
    public Vehicle vehicle;
    public TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer.emitting = false;
    }

    void Update()
    {
        if(vehicle.IsTireScreeching(out float lateralVelocity, out bool isBraking))
        {
            trailRenderer.emitting = true;
        }
        else
        {
            trailRenderer.emitting = false;
        }
    }
}
