using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
	private enum State
	{
		DRONING, INVESTIGATING, AGGRO, DEAD
	}
	private State _currentState;

	private Rigidbody2D _rigidBody;
    private HoardingConfig _config;
	private GameManager _gm;
	private Health health;
	private GameObject playerTarget;

    [Header("Enemy Properties")]
    [SerializeField] private float _droneSpeed = 1;
    [SerializeField] private float _investigateSpeed = 1.2f;
    [SerializeField] private float _aggroSpeed = 4;
    [SerializeField] private float attackDamageMultiplier = 1;
    [SerializeField] private float attackRange = 1;
    [SerializeField] private float attackCooldown = 1;
    private float attackTimer;
	[SerializeField] private float attackSpawnDistance = 0.6f;
	[SerializeField] public List<GameObject> attacks;
	public List<Limb> limbs;
	[SerializeField] private int maxLimbs = 2;
	[SerializeField] private float _turnSmoothing = 5;
	[SerializeField] private float _changeDirectionCooldown = 5;
	[SerializeField] private Vector3 _wanderTarget = new Vector3(1,1,1);
	[SerializeField] private float _moveSpeed = 1.05f;
	[SerializeField] private float friction = 0.92f;
	[SerializeField] private float decayTime = 60.0f; // how long it takes for corpse to disappear
	private float decayTimer;
	[Header("Enemy Perception")]
	[SerializeField] private float _obstacleAvoidDistance = 3;
	[SerializeField] private float _perceptionDistance = 30;
    [SerializeField] int _perceptionRayCount = 7;
    [SerializeField] float _fov = 120;


    private void ChangeState(State newState)
	{
		if(_currentState == newState) return;

		//switch (_currentState) // ON EXITS
  //      {
  //          case State.DRONING:
  //              break;
  //          case State.INVESTIGATING:
  //              break;
  //          case State.AGGRO:
  //              break;
  //          default:
  //              break;
  //      }

		switch (newState)
		{
			case State.DRONING:
				_moveSpeed = _droneSpeed;
				
				break;
			case State.INVESTIGATING:
				_moveSpeed = _investigateSpeed;
				
				break;
			case State.AGGRO:
				_moveSpeed = _aggroSpeed;
				
				break;
			case State.DEAD:
				_moveSpeed = 0;
				_rigidBody.Sleep();
				GetComponent<Collider2D>().enabled = false;
				decayTimer = decayTime;
				transform.localScale = new Vector3(transform.localScale.x * 0.9f, transform.localScale.y * 0.9f, transform.localScale.z);
				SpriteRenderer renderer = GetComponent<SpriteRenderer>();
				renderer.sortingOrder = -10;
				renderer.color = new Color(renderer.color.r * 0.5f, renderer.color.r * 0.5f, renderer.color.r * 0.5f, renderer.color.a);
				Limb[] attachedLimbs = GetComponentsInChildren<Limb>();
				foreach(var limb in attachedLimbs)
				{
					var limbRenderer = limb.gameObject.GetComponent<SpriteRenderer>();
					limbRenderer.sortingOrder = -9;
                    limbRenderer.color = new Color(limbRenderer.color.r * 0.5f, limbRenderer.color.r * 0.5f, limbRenderer.color.r * 0.5f, limbRenderer.color.a);
                }
				break;
			default:
				
				break;
		}

		_currentState = newState;
	}

	private Vector3 wanderTarget;

	void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
		health = GetComponent<Health>();	
        _config = FindObjectOfType<HoardingConfig>();
		_gm = FindObjectOfType<GameManager>();
		ChangeState(State.DRONING);

		limbs = new List<Limb>(GetComponentsInChildren<Limb>());
		foreach(var limb in limbs)
		{
			limb.AddAttacksToOwner();
		}
    }

    private void FixedUpdate()
	{
		Vector3 target = Vector3.zero;
        switch (_currentState)
        {
            case State.DRONING:
                target = Drone();
                break;
            case State.INVESTIGATING:
                target = Investigate();
                break;
            case State.AGGRO:
                target = Aggro();
				break;
			case State.DEAD:
				decayTimer -= Time.deltaTime;
				if(decayTimer <= 0)
				{
					StartCoroutine(FadeOut());
					decayTimer = 1000;
				}
                break;
			default:
				break;
        }
		if(_currentState != State.DEAD)
		{
			_rigidBody.AddForce(target * _moveSpeed * Time.deltaTime, ForceMode2D.Force);
			Rotate(target);
		}
		_rigidBody.velocity = _rigidBody.velocity * friction;
	}

	[SerializeField] private float fadeSpeed = 5;
	private IEnumerator FadeOut()
	{
		while(transform.localScale.x > 0.1)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * fadeSpeed);
			yield return null;
		}
		Destroy(gameObject);
	}

    #region FLOCKING

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
                }
            }
        }

        if (openDirectionsCount > 0 && hitCount > 0)
        {
            return (new Vector3(Mathf.Cos(selectedAngle), Mathf.Sin(selectedAngle), 0)).normalized;
        }

		return Vector3.zero;
    }

    #endregion

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
				else
				{
					playerTarget = hit.collider.gameObject;
					ChangeState(State.AGGRO);
					seekTarget += rayDir.normalized * 100;
					return seekTarget;
				}
			}
        }

        return Vector3.zero;
    }

    Vector3 Flee(Vector3 target)
	{
		Vector3 neededVelocity = (transform.position - target).normalized * _moveSpeed;
		return neededVelocity - new Vector3(_rigidBody.velocity.x, _rigidBody.velocity.y, 0);
	}


	virtual protected Vector3 Drone()
	{
		return (_config.cohesionPriority * Cohesion() + 
				_config.wanderPriority * Wander() + 
				_config.alignmentPriority * Alignment() + 
				_config.separationPriority * Separation() +
				_config.avoidancePriority * Avoidance() + Seek());
	}

    virtual protected Vector3 Investigate()
    {
        return (_config.cohesionPriority * Cohesion() +
                _config.wanderPriority * Wander() +
                _config.alignmentPriority * Alignment() +
                _config.separationPriority * Separation() +
                _config.avoidancePriority * Avoidance() + Seek());
    }

    virtual protected Vector3 Aggro()
    {
		float playerDistance = Vector3.Distance(playerTarget.transform.position, transform.position);

        if (playerDistance > _perceptionDistance)
		{
			playerTarget = null;
			ChangeState(State.DRONING);
			return Vector3.zero;
		}
		else if(playerDistance <= attackRange && attackTimer <= 0)
		{
			TryAttack();
		}

		if(attackTimer > 0)
		{
			attackTimer -= Time.deltaTime;
		}

		return ((playerTarget.transform.position - transform.position) * 50) + _config.avoidancePriority * 2 * Avoidance();
    }

	private void TryAttack()
	{
        if (attacks.Count > 0)
        {
            attackTimer = attackCooldown;
			Vector3 randomVariation = Utils.RandomUnitVector3() * 0.2f;
            GameObject attackObject = Instantiate(attacks[Random.Range(0, attacks.Count)], transform.position + (transform.up * attackSpawnDistance) + randomVariation, Quaternion.identity);
            Attack attack = attackObject.GetComponent<Attack>();
            attack.damageMultiplier = attackDamageMultiplier;
        }
    }

    bool isInFOV(Vector3 vec)
	{
		return Vector3.Angle(_rigidBody.velocity, vec - transform.position) <= _config.maxFOV;
	}

	void Rotate(Vector2 direction)
	{
		float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90;
		Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _turnSmoothing);
	}

	public void OnDeath()
	{
		_gm.enemies.Remove(this);
		ChangeState(State.DEAD);
	}

	[SerializeField] private float limbLaunchMod = 0.1f;
    public void OnHit(int damage, int maxHealth)
    {
        if(limbs.Count > 0 && damage >= (maxHealth / (maxLimbs+1))) // without +1, no enemy could be alive with no limbs
		{
			Limb victimLimb = limbs[Random.Range(0, limbs.Count)];
			victimLimb.DetachFromOwner();
			victimLimb.rb.bodyType = RigidbodyType2D.Dynamic;
			victimLimb.rb.AddForce(Utils.RandomUnitVector2() * damage * limbLaunchMod, ForceMode2D.Impulse);
			victimLimb.rb.angularVelocity = 200 * damage * Random.Range(0.7f, 1);
        }
    }

}
