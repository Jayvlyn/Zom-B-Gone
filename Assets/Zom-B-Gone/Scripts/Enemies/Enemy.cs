using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
	private enum State
	{
		IDLE, DRONING, INVESTIGATING, AGGRO, DEAD
	}
	private State currentState = State.IDLE;

	public EnemyData enemyData;

	public Rigidbody2D rigidBody;
	private GameManager gm;
	private Health health;
	private GameObject playerTarget;

	private float attackTimer;
	public List<GameObject> attacks; // attacks from head/base
	public List<Limb> limbs; // will hold attacks performed by arms, those attacks are lost when limb is removed
	public List<GameObject> bleedingParticles;
	[SerializeField] private int maxLimbs = 2;
	[SerializeField] private float turnSmoothing = 5;
	[SerializeField] private float changeDirectionCooldown = 5;
	private Vector2 wanderTarget = new Vector2(1, 1);
	private float currentMoveSpeed = 0;

	// Dead enemy removal
	[SerializeField] private float decayTime = 60.0f; // how long it takes for corpse to disappear
	private float decayTimer;

    public LayerMask AttackBlockersLm;
    public LayerMask MovementBlockersLm;
    public LayerMask FellowEnemyLm;

	private Vector2 target = Vector2.zero;


	private void ChangeState(State newState)
	{
		if(currentState == newState) return;

		//switch (currentState) // ON EXITS
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
				currentMoveSpeed = enemyData.droneSpeed;
				
				break;
			case State.INVESTIGATING:
				currentMoveSpeed = enemyData.investigateSpeed;
				
				break;
			case State.AGGRO:
				currentMoveSpeed = enemyData.aggroSpeed;
				
				break;
			case State.DEAD:
				currentMoveSpeed = 0;
				rigidBody.Sleep();
				GetComponent<Collider2D>().enabled = false;
				decayTimer = decayTime;
				transform.localScale = new Vector3(transform.localScale.x * 0.9f, transform.localScale.y * 0.9f, transform.localScale.z);
				SpriteRenderer renderer = GetComponent<SpriteRenderer>();
				renderer.sortingLayerName = "DeadEnemy";
                renderer.color = new Color(renderer.color.r * 0.5f, renderer.color.r * 0.5f, renderer.color.r * 0.5f, renderer.color.a);
				Limb[] attachedLimbs = GetComponentsInChildren<Limb>();
				foreach(var limb in attachedLimbs)
				{
					var limbRenderer = limb.gameObject.GetComponent<SpriteRenderer>();
					limbRenderer.sortingLayerName = "DeadEnemy";
                    limbRenderer.color = new Color(limbRenderer.color.r * 0.5f, limbRenderer.color.r * 0.5f, limbRenderer.color.r * 0.5f, limbRenderer.color.a);
                }
				foreach(var bloodParticle in bleedingParticles)
				{
					var particleRenderer = bloodParticle.gameObject.GetComponent<ParticleSystemRenderer>();
					particleRenderer.sortingLayerName = "DeadEnemy";
					particleRenderer.sortingOrder = -1;
				}
				break;
			default:
				
				break;
		}

		currentState = newState;
	}

	void Start()
    {
		health = GetComponent<Health>();
		gm = FindObjectOfType<GameManager>();
		ChangeState(State.DRONING);

		limbs = new List<Limb>(GetComponentsInChildren<Limb>());
		foreach(var limb in limbs)
		{
			limb.AddAttacksToOwner();
		}
    }


	private Coroutine tickCoroutine;
	private float tickInterval = 0.1f;
	private void OnEnable()
	{
		// Start the tick coroutine when the enemy is enabled
		tickCoroutine = StartCoroutine(TickCoroutine());
	}

	private void OnDisable()
	{
		// Stop the tick coroutine when the enemy is disabled to prevent memory leaks
		if (tickCoroutine != null) StopCoroutine(tickCoroutine);
	}

	private IEnumerator TickCoroutine()
	{
		while (true)
		{
			
			EnemyTick();

			yield return new WaitForSeconds(tickInterval);
		}
	}

	public void EnemyTick()
	{
		switch (currentState)
		{
			case State.DRONING:
				LerpTarget(Drone());
				//target = Drone();
				break;
			case State.INVESTIGATING:
				LerpTarget(Investigate());
				break;
			case State.AGGRO:
				LerpTarget(Aggro());
				break;
			case State.DEAD:
				decayTimer -= Time.deltaTime;
				if (decayTimer <= 0)
				{
					StartCoroutine(FadeOut());
					decayTimer = 1000;
				}
				break;
			default:
				break;
		}
	}

	// use to handle timers only, put computationally heavy things in enemy tick
	private void Update()
	{

		switch (currentState)
		{
			case State.DRONING:
				changeDirectionCooldown -= Time.deltaTime;
				break;
			case State.INVESTIGATING:
				break;
			case State.AGGRO:
				if (attackTimer > 0)
				{
					attackTimer -= Time.deltaTime;
				}
				break;
			case State.DEAD:
				break;
			default:
				break;
		}

	}

	private void FixedUpdate()
	{
		if (currentState != State.DEAD)
		{
			rigidBody.AddForce(target * currentMoveSpeed, ForceMode2D.Force);
			Rotate(target);
		}
	}

	[SerializeField] private float targetChangeSpeed = 40;
	private void LerpTarget(Vector2 newTarget)
	{
		target = Vector2.Lerp(target, newTarget, targetChangeSpeed * Time.deltaTime).normalized;
	}

	[SerializeField] private float fadeSpeed = 5;
	private IEnumerator FadeOut()
	{
		while(transform.localScale.x > 0.1)
		{
			transform.localScale = Vector2.Lerp(transform.localScale, Vector2.zero, Time.deltaTime * fadeSpeed);
			yield return null;
		}
		Destroy(gameObject);
	}

    #region FLOCKING

    protected Vector2 Wander()
	{
		if(changeDirectionCooldown <= 0)
		{
            float minAngle = -90f;
            float maxAngle = 90f;

            float randomAngle = Random.Range(minAngle, maxAngle);

            Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);

            wanderTarget = rotation * wanderTarget;
			wanderTarget *= enemyData.wanderDistance;
            changeDirectionCooldown = Random.Range(1.0f, 5.0f);
		}

		return wanderTarget.normalized;
	}

	Vector2 Cohesion()
	{
		Vector2 cohesionVector = new Vector2();
		int countEnemies = 0;
		List<Enemy> neighbors = GetNeighbors(this, enemyData.cohesionRadius);
		if (neighbors.Count == 0) return cohesionVector;
		foreach (var enemy in neighbors)
		{
			if (isInFOV(enemy.transform.position))
			{
				cohesionVector += (Vector2)enemy.transform.position;
				countEnemies++;
			}
		}
		if (countEnemies == 0) return cohesionVector;

		cohesionVector /= countEnemies;
		cohesionVector = cohesionVector - (Vector2)transform.position;

		return cohesionVector.normalized;
	}

	Vector2 Alignment()
	{
		Vector2 alignVector = new Vector2();
		var enemies = GetNeighbors(this, enemyData.alignmentRadius);
		if (enemies.Count == 0) return alignVector;
		foreach (var enemy in enemies)
		{
			if (isInFOV(enemy.transform.position) && enemy.rigidBody != null)
			{
				alignVector += new Vector2(enemy.rigidBody.linearVelocity.x, enemy.rigidBody.linearVelocity.y);
			}
		}

		return alignVector.normalized;
	}

	Vector2 Separation()
	{
		Vector2 separateVector = new Vector2();
		var enemies = GetNeighbors(this, enemyData.separationRadius);
		if (enemies.Count == 0) return separateVector;

		foreach (var enemy in enemies)
		{
			if (isInFOV(enemy.transform.position))
			{
				Vector2 movingTowards = transform.position - enemy.transform.position;
				if (movingTowards.magnitude > 0)
				{
					separateVector += movingTowards.normalized / movingTowards.magnitude;
				}
			}
		}

		return separateVector.normalized;
	}

    Vector2 Avoidance()
    {
        float angleBetweenRays = enemyData.fov / (enemyData.perceptionRayCount - 1);

        int openDirectionsCount = 0;
		int hitCount = 0;
        float minObstruction = float.MaxValue;
        float selectedAngle = 0f;

        for (int i = 0; i < enemyData.perceptionRayCount; i++)
        {
            float angle = (transform.eulerAngles.z - (enemyData.fov * 0.5f) + (angleBetweenRays * i) + 90) * Mathf.Deg2Rad;
            Vector2 rayDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, enemyData.obstacleAvoidDistance, MovementBlockersLm);
			if (hit) hitCount++;
            if (!hit)
            {
                openDirectionsCount++;

                float obstruction = Vector2.Distance(transform.position, (Vector2)transform.position + rayDir * enemyData.obstacleAvoidDistance);
                if (obstruction < minObstruction)
                {
                    minObstruction = obstruction;
                    selectedAngle = angle;
                }
            }
        }

        if (openDirectionsCount > 0 && hitCount > 0)
        {
            return new Vector2(Mathf.Cos(selectedAngle), Mathf.Sin(selectedAngle)).normalized;
        }

		return Vector2.zero;
    }

    #endregion

    Vector2 Seek()
    {
		Vector2 seekTarget = Vector2.zero;
        float angleBetweenRays = enemyData.fov / (enemyData.perceptionRayCount - 1);

        for (int i = 0; i < enemyData.perceptionRayCount; i++)
        {
            float angle = (transform.eulerAngles.z - (enemyData.fov * 0.5f) + (angleBetweenRays * i) + 90) * Mathf.Deg2Rad;
            Vector2 rayDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, enemyData.perceptionDistance, LayerMask.GetMask("Player"));
			if (hit)
			{
                RaycastHit2D worldHit = Physics2D.Raycast(transform.position, rayDir, enemyData.perceptionDistance, LayerMask.GetMask("World"));
				if(worldHit && worldHit.distance < hit.distance)
				{
					return Vector2.zero;
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

        return Vector2.zero;
    }

    Vector2 Flee(Vector2 target)
	{
		Vector2 neededVelocity = ((Vector2)transform.position - target).normalized * currentMoveSpeed;
		return neededVelocity - new Vector2(rigidBody.linearVelocity.x, rigidBody.linearVelocity.y);
	}


	virtual protected Vector2 Drone()
	{
		return (enemyData.cohesionPriority * Cohesion() +
				enemyData.wanderPriority * Wander() + 
				enemyData.alignmentPriority * Alignment() + 
				enemyData.separationPriority * Separation() +
				enemyData.avoidancePriority * Avoidance() + Seek());
	}

    virtual protected Vector2 Investigate()
    {
        return (enemyData.cohesionPriority * Cohesion() +
                enemyData.wanderPriority * Wander() +
				enemyData.alignmentPriority * Alignment() +
                enemyData.separationPriority * Separation() +
                enemyData.avoidancePriority * Avoidance() + Seek());
    }

    virtual protected Vector2 Aggro()
    {
		float playerDistance = Vector2.Distance(playerTarget.transform.position, transform.position);
		Vector2 direction = playerTarget.transform.position - transform.position;

        if (playerDistance > enemyData.perceptionDistance)
		{
			playerTarget = null;
			ChangeState(State.DRONING);
			return Vector2.zero;
		}
		else if(playerDistance <= enemyData.attackRange && attackTimer <= 0)
		{
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, playerDistance, AttackBlockersLm);
            if (!hit.collider)
            {
                TryAttack();
            }
		}


		RaycastHit2D wallHit = Physics2D.Raycast(transform.position, direction, playerDistance, MovementBlockersLm);
		if(wallHit.collider)
		{
			return direction + enemyData.avoidancePriority * 2 * Avoidance();
		}
		else // straight shot to player, go for them
		{
			return direction * 20;
		}

    }

	private void TryAttack()
	{
        if (attacks.Count > 0)
        {
            attackTimer = enemyData.attackCooldown;
			Vector3 randomVariation = Utils.RandomUnitVector3() * 0.2f;
            GameObject attackObject = Instantiate(attacks[Random.Range(0, attacks.Count)], transform.position + (transform.up * enemyData.attackSpawnDistance) + randomVariation, Quaternion.identity);
            Attack attack = attackObject.GetComponent<Attack>();
            attack.damageMultiplier = enemyData.attackDamageMultiplier;
        }
    }

    bool isInFOV(Vector2 vec)
	{
		return Vector2.Angle(rigidBody.linearVelocity, vec - (Vector2)transform.position) <= enemyData.fov;
	}

	public List<Enemy> GetNeighbors(Enemy enemy, float radius)
	{
		List<Enemy> neighborsFound = new List<Enemy>();

		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, FellowEnemyLm);
		if(TryGetComponent(out Enemy e))
		{
			neighborsFound.Add(e);
		}

		return neighborsFound;
	}

	void Rotate(Vector2 direction)
	{
		float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90;
		Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSmoothing);
	}

	public void OnDeath()
	{
		gm.enemies.Remove(this);
		ChangeState(State.DEAD);
	}

    public void OnHit(int damage, float dismemberChance = 0)
    {
		DismemberLimb(damage, dismemberChance);
    }

	private float limbLaunchMod = .3f;
	private float limbSpinForce = 100f;


    public void DismemberLimb(int damage, float dismemberChance)
	{
        if (limbs.Count > 0)
        {
			float num = Random.Range(0f, 100f);
			if (num < dismemberChance)
			{
				Limb victimLimb = limbs[Random.Range(0, limbs.Count)];
				victimLimb.DetachFromOwner();
				victimLimb.rb.bodyType = RigidbodyType2D.Dynamic;
				victimLimb.rb.AddForce(Utils.RandomUnitVector2() * damage * limbLaunchMod, ForceMode2D.Impulse);
				victimLimb.rb.angularVelocity = limbSpinForce * damage * Random.Range(0.7f, 1);
			}
        }
    }

	private void OnCollisionStay2D(Collision2D collision)
	{
		if((collision.gameObject.CompareTag("Door") || collision.gameObject.layer == LayerMask.NameToLayer("Window")) && attackTimer <= 0 && rigidBody.linearVelocity.magnitude < 0.2) // stopped by door or window
		{
			TryAttack();
		}
	}

	private GameObject collidingVehicle = null;
    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
		{
			collidingVehicle = collision.gameObject;
		}

        else if(collidingVehicle != null && rigidBody.linearVelocity.magnitude > 0)
		{
            int worldObstacleMask = LayerMask.GetMask("World", "Obstacle");
            if ((worldObstacleMask & (1 << collision.gameObject.layer)) != 0)
			{
				Debug.Log(rigidBody.linearVelocity.magnitude);
				health.TakeDamage(rigidBody.linearVelocity.magnitude * 5, Vector2.zero);
			}
		}
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
		if (collision.gameObject == collidingVehicle)
        {
			collidingVehicle = null;
        }
    }
}
