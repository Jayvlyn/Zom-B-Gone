using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemies/New Enemy Data")]
public class EnemyData : ScriptableObject
{
	[Header("Stats")]
	public float droneSpeed = 3;
	public float investigateSpeed = 4;
	public float aggroSpeed = 7;
	public float attackDamageMultiplier = 1;
	public float attackRange = 1;
	public float attackCooldown = 1;
	public float obstacleAvoidDistance = 3;
	public float attackSpawnDistance = 0.6f;

	[Header("Perception")]
	public float fov = 180;
	public float perceptionDistance = 30;
	public int perceptionRayCount = 7;


	[Header("Hoarding")]
	public bool doHoarding = true;

	public float wanderDistance;
	[Range(0, 100)] public float wanderPriority;

	public float cohesionRadius;
	[Range(0, 100)] public float cohesionPriority;

	public float alignmentRadius;
	[Range(0, 100)] public float alignmentPriority;

	public float separationRadius;
	[Range(0, 100)] public float separationPriority;

	[Range(0, 100)] public float avoidancePriority;

	[Header("Audio")]
	public List<EnemyVoice> possibleVoices;
}
