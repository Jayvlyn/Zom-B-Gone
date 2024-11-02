using UnityEngine;
using GameEvents;

[CreateAssetMenu(fileName = "New Vehicle Data", menuName = "New Vehicle Data")]
public class VehicleData : ScriptableObject
{
	public VoidEvent enterEvent;
	public VoidEvent exitEvent;

	public float accelerationSpeed = 30000;
	public float brakeSpeed = 10000;
	public float steeringSpeed = 100;
	public float maxTurnAngle = 45;
	public float maxSpeed = 500;
	public float exitDistance = 1.3f;


	public float baseDriftFactor = 0.1f;
	public float driftingDriftFactor = 0.99f;
	public float driftDrag = 0.1f;
	public float driveDrag = 1f;

	[Header("Audio Refs")]
	public AudioClip enterSound;
	public AudioClip exitSound;
	public AudioClip tireScreechSound;
	public AudioClip[] engineSounds = new AudioClip[6]; // 0 - 5 depending on speed
}
