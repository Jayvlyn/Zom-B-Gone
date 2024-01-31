using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberConfig : MonoBehaviour
{
    public float maxFOV = 180;
    public float maxAcceleration;
    public float maxVelocity;

    // Wander
    public float wanderJitter;
    public float wanderRadius;
    public float wanderDistance;
    public float wanderPriority;

    // Cohesion
    public float cohesionRadius;
    public float cohesionPriority;

    // Alignment
    public float alignmentRadius;
    public float alighnmentPriority;

    // Separation
    public float separationRadius;
    public float separationPriority;

    // Avoidance
    public float avoidanceRadius;
    public float avoidancePriority;
}
