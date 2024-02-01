using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
	[SerializeField] private float _changeDirectionCooldown = 5;
	[SerializeField] private Vector3 _wanderTarget = new Vector3(1,1,1);
	[Header("Enemy Perception")]
	[SerializeField] private float _obstacleAvoidDistance = 10;
	[SerializeField] private float _perceptionDistance = 10;
    [SerializeField] int _perceptionRayCount = 5;
    [SerializeField] float _fov = 90;


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
		_rigidBody.AddForce(Combine() * _walkSpeed * Time.deltaTime, ForceMode2D.Impulse);
		_rigidBody.velocity = Vector3.ClampMagnitude(_rigidBody.velocity, _walkSpeed);
		Rotate();
	}

	protected Vector3 Wander()
	{
		_changeDirectionCooldown -= Time.deltaTime;
		if(_changeDirectionCooldown <= 0)
		{
            float minAngle = -90f;
            float maxAngle = 90f;

            float randomAngle = Random.Range(minAngle, maxAngle);

            Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);

            _wanderTarget = rotation * _wanderTarget;
			_wanderTarget *= _config.wanderDistance;
            _changeDirectionCooldown = Random.Range(1.0f, 5.0f);
		}

		return _wanderTarget.normalized;
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
		return cohesionVector.normalized;
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
        float angleBetweenRays = _fov / (_perceptionRayCount - 1);

        int openDirectionsCount = 0;
		int hitCount = 0;
        float minObstruction = float.MaxValue;
        float selectedAngle = 0f;

        for (int i = 0; i < _perceptionRayCount; i++)
        {
            float angle = (transform.eulerAngles.z - (_fov * 0.5f) + (angleBetweenRays * i) + 90) * Mathf.Deg2Rad;
            Vector3 rayDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, _obstacleAvoidDistance, LayerMask.GetMask("World"));
			if (hit) hitCount++;
            if (!hit)
            {
                openDirectionsCount++;

                float obstruction = Vector3.Distance(transform.position, transform.position + rayDir * _obstacleAvoidDistance);
                if (obstruction < minObstruction)
                {
                    minObstruction = obstruction;
                    selectedAngle = angle;
					Debug.Log(selectedAngle);
                }
            }
        }

        if (openDirectionsCount > 0 && hitCount > 0)
        {
            return (new Vector3(Mathf.Cos(selectedAngle), Mathf.Sin(selectedAngle), 0)).normalized;
        }

		return Vector3.zero;
    }

    Vector3 Seek()
    {
		Vector3 seekTarget = Vector3.zero;
        float angleBetweenRays = _fov / (_perceptionRayCount - 1);

        for (int i = 0; i < _perceptionRayCount; i++)
        {
            float angle = (transform.eulerAngles.z - (_fov * 0.5f) + (angleBetweenRays * i) + 90) * Mathf.Deg2Rad;
            Vector3 rayDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, _perceptionDistance, LayerMask.GetMask("Player"));
			if (hit)
			{
                RaycastHit2D worldHit = Physics2D.Raycast(transform.position, rayDir, _perceptionDistance, LayerMask.GetMask("World"));
				if(worldHit && worldHit.distance < hit.distance)
				{
					return Vector3.zero;
				}
				seekTarget += rayDir.normalized * 1000;
				return seekTarget;
			}
        }

        return Vector3.zero;
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
				_config.alignmentPriority * Alignment() + 
				_config.separationPriority * Separation() +
				_config.avoidancePriority * Avoidance() + Seek());
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
