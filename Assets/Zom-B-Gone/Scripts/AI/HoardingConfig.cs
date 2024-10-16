using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoardingConfig : MonoBehaviour
{
	public float maxFOV = 180;

	// Wander
	public float wanderDistance;
	[Range(0, 100)] public float wanderPriority;

	// Cohesion
	public float cohesionRadius;
	[Range(0, 100)] public float cohesionPriority;

	// Alignment
	public float alignmentRadius;
	[Range(0, 100)] public float alignmentPriority;

	// Separation
	public float separationRadius;
	[Range(0, 100)] public float separationPriority;

	// Avoidance
	[Range(0, 100)] public float avoidancePriority;

}
