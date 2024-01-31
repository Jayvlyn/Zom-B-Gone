using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
	private enum State
	{
		IDLE, WALKING, RUNNING, CRAWLING
	}
	private State _currentState;

	private Rigidbody2D _rigidBody;
    private HoardingConfig _config;
	private GameManager _gm;

    [Header("Enemy Properties")]
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 7;
    [SerializeField] private float _crawlSpeed = 2;
    [SerializeField] private float _attackDamage = 10;
    [SerializeField] private float _attacksPerSecond = 1;
	[SerializeField] private float _turnSmoothing = 5;
	[SerializeField] private float _perceptionDistance = 10;

	private void ChangeState(State newState)
	{
		switch (newState)
		{
			case State.WALKING:
				
				break;
			case State.RUNNING:
				
				break;
			case State.CRAWLING:
				
				break;
			default: // IDLE:
				
				break;
		}

		_currentState = newState;
	}

	private Vector3 wanderTarget;

	void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _config = FindObjectOfType<HoardingConfig>();
		_gm = FindObjectOfType<GameManager>();
    }

	private void FixedUpdate()
	{
		_rigidBody.AddForce(Combine() * _walkSpeed * Time.deltaTime, ForceMode2D.Force);
		_rigidBody.velocity = Vector3.ClampMagnitude(_rigidBody.velocity, _walkSpeed);
		Rotate();
	}

	protected Vector3 Wander()
	{
		float jitter = _config.wanderJitter * Time.deltaTime;
		wanderTarget += new Vector3(Utils.RandomBinomial() * jitter, Utils.RandomBinomial() * jitter, 0);
		wanderTarget = wanderTarget.normalized;
		wanderTarget *= _config.wanderRadius;
		Vector3 targetInLocalSpace = wanderTarget + new Vector3(_config.wanderDistance, _config.wanderDistance, 0);
		Vector3 targetInWorldSpace = transform.TransformPoint(targetInLocalSpace);
		targetInWorldSpace -= transform.position;
		return targetInWorldSpace.normalized;
	}

	Vector3 Cohesion()
	{
		Vector3 cohesionVector = new Vector3();
		int countEnemies = 0;
		List<Enemy> neighbors = _gm.GetNeighbors(this, _config.cohesionRadius);
		if (neighbors.Count == 0) return cohesionVector;
		foreach (var enemy in neighbors)
		{
			if (isInFOV(enemy.transform.position))
			{
				cohesionVector += enemy.transform.position;
				countEnemies++;
			}
		}
		if (countEnemies == 0) return cohesionVector;

		cohesionVector /= countEnemies;

		cohesionVector = cohesionVector - transform.position;
		cohesionVector = Vector3.Normalize(cohesionVector);
		return cohesionVector;
	}

	Vector3 Alignment()
	{
		Vector3 alignVector = new Vector3();
		var enemies = _gm.GetNeighbors(this, _config.alignmentRadius);
		if (enemies.Count == 0) return alignVector;
		foreach (var enemy in enemies)
		{
			if (isInFOV(enemy.transform.position) && enemy._rigidBody != null)
			{
				alignVector += new Vector3(enemy._rigidBody.velocity.x, enemy._rigidBody.velocity.y, 0);
				//alignVector += new Vector3(1, 1, 0);
			}
		}

		return alignVector.normalized;
	}

	Vector3 Separation()
	{
		Vector3 separateVector = new Vector3();
		var enemies = _gm.GetNeighbors(this, _config.separationRadius);
		if (enemies.Count == 0) return separateVector;

		foreach (var enemy in enemies)
		{
			if (isInFOV(enemy.transform.position))
			{
				Vector3 movingTowards = transform.position - enemy.transform.position;
				if (movingTowards.magnitude > 0)
				{
					separateVector += movingTowards.normalized / movingTowards.magnitude;
				}
			}
		}

		return separateVector.normalized;
	}

	Vector3 Avoidance()
	{
		Vector3 avoidVector = new Vector3();
		RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, _perceptionDistance, LayerMask.GetMask("World"));
		if(hit)
		{
			avoidVector = Flee(hit.point);
		}
		return avoidVector.normalized;
	}

	Vector3 Flee(Vector3 target)
	{
		Vector3 neededVelocity = (transform.position - target).normalized * _walkSpeed;
		return neededVelocity - new Vector3(_rigidBody.velocity.x, _rigidBody.velocity.y, 0);
	}


	virtual protected Vector3 Combine()
	{
		return (_config.cohesionPriority * Cohesion() + 
				_config.wanderPriority * Wander() + 
				_config.alighnmentPriority * Alignment() + 
				_config.separationPriority * Separation() +
				_config.avoidancePriority * Avoidance());
	}

	bool isInFOV(Vector3 vec)
	{
		return Vector3.Angle(_rigidBody.velocity, vec - transform.position) <= _config.maxFOV;
	}

	void Rotate()
	{
		Vector2 direction = _rigidBody.velocity.normalized;
		float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90;
		Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _turnSmoothing);
	}

}
