using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
	private enum State
	{
		IDLE, DRONING, INVESTIGATING, AGGRO, DEAD
	}
	[SerializeField] private State currentState = State.IDLE;

	public EnemyData enemyData;
	private EnemyVoice voice;
	public AudioSource audioSource;
	public SpriteRenderer spriteRenderer;

	public Rigidbody2D rigidBody;
	public EnemyHead head;

	private Health health;
	private GameObject playerTarget;

	private float attackTimer;
	public List<GameObject> attacks; // attacks from head/base
	public List<Limb> limbs; // will hold attacks performed by arms, those attacks are lost when limb is removed
	public List<Sprite> possibleSprites; // will hold attacks performed by arms, those attacks are lost when limb is removed
	public List<GameObject> bleedingParticles;
	[SerializeField] private int maxLimbs = 2;
	[SerializeField] private float turnSmoothing = 5;
	[SerializeField] private float changeDirectionCooldown = 5;
	private Vector2 wanderTarget = new Vector2(1, 1);
	private float currentMoveSpeed = 0;
	private float CurrentMoveSpeed
	{
		get { return currentMoveSpeed; }
		set {
			currentMoveSpeed = value;
            #region hat buff
            if(head && head.wornHat != null)
			{
				currentMoveSpeed *= head.wornHat.hatData.moveSpeedMod;
			}
            #endregion

        }
    }

	// Dead enemy removal
	[SerializeField] private float decayTime = 60.0f; // how long it takes for corpse to disappear
	private float decayTimer;

    public LayerMask AttackBlockersLm;
    public LayerMask MovementBlockersLm;
    public LayerMask FellowEnemyLm;
	public LayerMask WorldObstacleLm;
	public LayerMask SightBlockersLm;
	public LayerMask AttackableMovementBlockersLm;

	private Vector2 target = Vector2.zero;
	private Vector2 desiredTarget = Vector2.zero;
	private Vector2 investigaitonPoint = Vector2.zero;

	public static int sortOrder = 0;

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
				CurrentMoveSpeed = enemyData.droneSpeed;
				
				break;
			case State.INVESTIGATING:
				CurrentMoveSpeed = enemyData.investigateSpeed;
				
				break;
			case State.AGGRO:
				PlayAggroSound();
				CurrentMoveSpeed = enemyData.aggroSpeed;
				
				break;
			case State.DEAD:
				StopCoroutine(tickCoroutine);
				CurrentMoveSpeed = 0;
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
					
					limb.spriteRenderer.sortingLayerName = "DeadEnemy";
                    limb.spriteRenderer.color = new Color(limb.spriteRenderer.color.r * 0.5f, limb.spriteRenderer.color.r * 0.5f, limb.spriteRenderer.color.r * 0.5f, limb.spriteRenderer.color.a);
                }
				foreach(var bloodParticle in bleedingParticles)
				{
					var particleRenderer = bloodParticle.gameObject.GetComponent<ParticleSystemRenderer>();
					particleRenderer.sortingLayerName = "DeadEnemy";
					particleRenderer.sortingOrder = -1;
				}

				if(head)
				{
					head.spriteRenderer.sortingLayerName = "DeadEnemy";
					head.spriteRenderer.color = new Color(head.spriteRenderer.color.r * 0.5f, head.spriteRenderer.color.r * 0.5f, head.spriteRenderer.color.r * 0.5f, head.spriteRenderer.color.a);
				}

                break;
            default:
                break;
        }
        currentState = newState;
    }

    // use to handle timers only, put computationally heavy things in enemy tick
    private void Update()
    {
        LerpTarget(desiredTarget);

        switch (currentState)
        {
            case State.DRONING:
                changeDirectionCooldown -= Time.deltaTime;
                break;
            case State.INVESTIGATING:
                break;
            case State.AGGRO:
                if (attackTimer > 0) attackTimer -= Time.deltaTime;
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

    private void FixedUpdate()
    {
        if (currentState != State.DEAD)
        {
            Rotate(target);

            //if(rigidBody.linearVelocity.magnitude < 0.4f) // if not moving much see if something is in the way
            //{
            //             int hitCount = rigidBody.Cast(target, movementBlockerFilter, new RaycastHit2D[1], currentMoveSpeed * Time.fixedDeltaTime);
            //	if (hitCount > 0) return;       
            //         }

            float dist = Vector2.Distance(target, transform.position);
            if (dist > 1)
            {
                rigidBody.AddForce(target * CurrentMoveSpeed, ForceMode2D.Force);
            }
            else if (dist > 0.2)
            {
                rigidBody.AddForce(target * CurrentMoveSpeed * dist, ForceMode2D.Force);
            }
        }
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
                if (UnityEngine.Random.Range(0, 200) == 0) PlayDroneSound();
                desiredTarget = Drone();
                break;
            case State.INVESTIGATING:
                if (UnityEngine.Random.Range(0, 200) == 0) PlayDroneSound();
                desiredTarget = Investigate();
                break;
            case State.AGGRO:
                if (UnityEngine.Random.Range(0, 100) == 0) PlayAggroSound();
                desiredTarget = Aggro();
                break;

            default:
                break;
        }
    }

    private LayerMask playerLm;
    private LayerMask allPlayerLm;
    private LayerMask worldLm;
    private LayerMask windowLm;
    private LayerMask interactableLm;
    private void Awake()
    {
        SetSortOrder();
        spriteRenderer.sprite = possibleSprites[UnityEngine.Random.Range(0, possibleSprites.Count)];

        allPlayerLm = LayerMask.GetMask("Player", "DisguisedPlayer");
        playerLm = LayerMask.GetMask("Player");
        worldLm = LayerMask.GetMask("World");
        windowLm = LayerMask.GetMask("Window");
        interactableLm = LayerMask.GetMask("Interactable");
        health = GetComponent<Health>();
    }
    ContactFilter2D movementBlockerFilter = new ContactFilter2D();
    void Start()
    {
        movementBlockerFilter.SetLayerMask(~LayerMask.GetMask("Interactable"));// for doors, dont(~) include interactables like doors so they still want to walk through them
		movementBlockerFilter.useLayerMask = true;

		if(enemyData.possibleVoices.Count > 0)
			voice = enemyData.possibleVoices[UnityEngine.Random.Range(0, enemyData.possibleVoices.Count)];

		ChangeState(State.DRONING);

		limbs = new List<Limb>(GetComponentsInChildren<Limb>());
		foreach(var limb in limbs)
		{
			limb.AddAttacksToOwner();
		}
    }

	private Coroutine tickCoroutine;
	private float tickInterval = 0.5f;
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

	private void SetSortOrder()
	{
		spriteRenderer.sortingOrder = sortOrder;
		if (head) head.spriteRenderer.sortingOrder = sortOrder + 1;
		foreach (var limb in limbs)
		{
			limb.spriteRenderer.sortingOrder = sortOrder + 1;
		}
		sortOrder += 2;
	}

	public void StartInvestigating(Vector2 investigationPoint)
	{
		if (currentState != State.DEAD && currentState != State.AGGRO)
		{
			this.investigaitonPoint = investigationPoint;
			ChangeState(State.INVESTIGATING);
		}
	}

	[SerializeField] private float targetChangeSpeed = 10;
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

            float randomAngle = UnityEngine.Random.Range(minAngle, maxAngle);

            Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);

            wanderTarget = rotation * wanderTarget;
			wanderTarget *= enemyData.wanderDistance;
            changeDirectionCooldown = UnityEngine.Random.Range(1.0f, 5.0f);
		}

		return wanderTarget.normalized;
	}

	GameObject[] cohesionNeighbors; // for outside use in finding disguised player
	GameObject disguisedPlayer;
	Vector2 Cohesion()
	{
		bool foundPlayerNeighbor = false;
		Vector2 cohesionVector = new Vector2();
		int neighborCount = 0;
		cohesionNeighbors = GetNeighbors(enemyData.cohesionRadius);
		if (cohesionNeighbors.Length == 0) return cohesionVector;
		foreach (var neighbor in cohesionNeighbors)
		{
			if (isInFOV(neighbor.transform.position))
			{
				cohesionVector += (Vector2)neighbor.transform.position;
				neighborCount++;
				if(neighbor.CompareTag("Player"))
				{
					disguisedPlayer = neighbor;
					foundPlayerNeighbor = true;
				}
			}
		}

		if (!foundPlayerNeighbor) disguisedPlayer = null;

		if (neighborCount == 0) return cohesionVector;

		cohesionVector /= neighborCount;

		cohesionVector = cohesionVector - (Vector2)transform.position;
		return cohesionVector.normalized;
	}

	Vector2 Alignment()
	{
		Vector2 alignVector = new Vector2();
        GameObject[] neighbors = GetNeighbors(enemyData.alignmentRadius);
		if (neighbors.Length == 0) return alignVector;
		foreach (var neighbor in neighbors)
		{
			if (isInFOV(neighbor.transform.position))
			{
				Rigidbody2D neighborRb = neighbor.GetComponent<Rigidbody2D>();
				alignVector += new Vector2(neighborRb.linearVelocity.x, neighborRb.linearVelocity.y);
			}
		}

		return alignVector.normalized;
	}

	Vector2 Separation()
	{
		Vector2 separateVector = new Vector2();
        GameObject[] neighbors = GetNeighbors(enemyData.separationRadius);
		if (neighbors.Length == 0) return separateVector;

		foreach (var neighbor in neighbors)
		{
			if (isInFOV(neighbor.transform.position))
			{
				Vector2 movingTowards = transform.position - neighbor.transform.position;
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
		float fov = enemyData.fov;
		if (fov < 180) fov += 90;

        float angleBetweenRays = fov / (enemyData.perceptionRayCount - 1);

        int openDirectionsCount = 0;
		int hitCount = 0;
        float minObstruction = float.MaxValue;
        float selectedAngle = 0f;

        for (int i = 0; i < enemyData.perceptionRayCount; i++)
        {
            float angle = (transform.eulerAngles.z - (fov * 0.5f) + (angleBetweenRays * i) + 90) * Mathf.Deg2Rad;
            Vector2 rayDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, enemyData.obstacleAvoidDistance, MovementBlockersLm);
            Debug.DrawLine(transform.position, (Vector2)transform.position + rayDir * enemyData.obstacleAvoidDistance, Color.red);
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

	Vector2 InvestigateTarget()
	{
		float dist = Vector2.Distance(investigaitonPoint, transform.position);

        if (disguisedPlayer)
        {
			float playerToInvestigation = Vector2.Distance(disguisedPlayer.transform.position, investigaitonPoint);
			if (playerToInvestigation < 3)
			{
				playerTarget = disguisedPlayer;
				ChangeState(State.AGGRO);
			}
        }

        if (dist < 1)
		{
			if (loseInterestRoutine == null)
				loseInterestRoutine = StartCoroutine(LoseInterestTimer());
		}

		return (investigaitonPoint - (Vector2)transform.position).normalized;
	}

	private Coroutine loseInterestRoutine;
	private IEnumerator LoseInterestTimer()
	{
		for (int i = 0; i < 3; i++)
		{
			investigaitonPoint.x += UnityEngine.Random.Range(-2, 3);
			investigaitonPoint.y += UnityEngine.Random.Range(-2, 3);
			yield return new WaitForSeconds(3);
		}
		loseInterestRoutine = null;
		if(currentState != State.DEAD && currentState != State.AGGRO)
		{
			ChangeState(State.DRONING);

		}
	}

	private float GetPlayerSpotDistance()
	{
        float sightDistance = enemyData.perceptionDistance * GameManager.globalLight.intensity;
        if (PlayerController.isSneaking) sightDistance *= 0.5f;
		return sightDistance;
    }

	Vector2 Seek()
    {
		Vector2 seekTarget = Vector2.zero;
        float angleBetweenRays = enemyData.fov / (enemyData.perceptionRayCount - 1);

		float playerSpotDistance = GetPlayerSpotDistance();

        for (int i = 0; i < enemyData.perceptionRayCount; i++)
        {
            float angle = (transform.eulerAngles.z - (enemyData.fov * 0.5f) + (angleBetweenRays * i) + 90) * Mathf.Deg2Rad;
            Vector2 rayDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));


            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, playerSpotDistance, playerLm);
			if (hit)
			{
                RaycastHit2D worldHit = Physics2D.Raycast(transform.position, rayDir, playerSpotDistance, worldLm);
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
		Vector2 neededVelocity = ((Vector2)transform.position - target).normalized * CurrentMoveSpeed;
		return neededVelocity - new Vector2(rigidBody.linearVelocity.x, rigidBody.linearVelocity.y);
	}


	virtual protected Vector2 Drone()
	{
		return (enemyData.cohesionPriority * Cohesion() +
				enemyData.alignmentPriority * Alignment() + 
				enemyData.separationPriority * Separation() +
				enemyData.avoidancePriority * Avoidance() +
				enemyData.wanderPriority * Wander() + // Wander for droning
				Seek()).normalized;
	}

    virtual protected Vector2 Investigate()
    {
        return (enemyData.cohesionPriority * Cohesion() +
				enemyData.alignmentPriority * Alignment() +
                enemyData.separationPriority * Separation() +
                enemyData.avoidancePriority * Avoidance() +
				enemyData.investigatePriority * InvestigateTarget() + // Get target for investigation
				Seek()).normalized;
    }

    virtual protected Vector2 Aggro()
    {
		float playerDistance = Vector2.Distance(playerTarget.transform.position, transform.position);
		Vector2 direction = playerTarget.transform.position - transform.position;

		float playerSpotDistance = GetPlayerSpotDistance();

        if (playerDistance > playerSpotDistance || PlayerController.hiding)
		{
			loseSightOfPlayer();
            return Vector2.zero;
		}
		else if(playerDistance <= enemyData.attackRange && attackTimer <= 0)
		{
            float angleToPlayer = Vector2.Angle(transform.up, direction);
			if(angleToPlayer <= 30f)
			{
				RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, playerDistance, AttackBlockersLm);
				if (!hit.collider)
				{
					TryAttack();
				}
			}
		}

        RaycastHit2D sightHit = Physics2D.Raycast(transform.position, direction, playerDistance, SightBlockersLm);
        if (sightHit.collider)
		{
			loseSightOfPlayer();
            return Vector2.zero;
        }

        RaycastHit2D movementHit = Physics2D.Raycast(transform.position, direction, playerDistance, MovementBlockersLm);
		if(movementHit.collider)
		{
			
			return (enemyData.blockedTargetPriority * direction + 
					enemyData.blockedAvoidancePriority * Avoidance() + 
					enemyData.blockedSeparationPriority * Separation()).normalized;
		}
		else // straight shot to player, go for them
		{
			if(playerDistance < 1)
			{
				return direction;
			}


			return (enemyData.straightShotTargetPriority * direction +
					enemyData.straightShotAvoidancePriority * Avoidance() +
                    enemyData.straightShotSeparationPriority * Separation()).normalized;
        }

    }

	private void loseSightOfPlayer()
	{
        Vector2 lastSeenPosition = playerTarget.transform.position;
        playerTarget = null;
        this.investigaitonPoint = lastSeenPosition;
        if(currentState != State.DEAD) ChangeState(State.INVESTIGATING);
    }

	private void TryAttack()
	{
        if (attacks.Count > 0)
        {
            attackTimer = enemyData.attackCooldown;
			Vector3 randomVariation = Utils.RandomUnitVector3() * 0.2f;
            GameObject attackObject = Instantiate(attacks[UnityEngine.Random.Range(0, attacks.Count)], transform.position + (transform.up * enemyData.attackSpawnDistance) + randomVariation, Quaternion.identity);
            Attack attack = attackObject.GetComponent<Attack>();
            attack.damageMultiplier = enemyData.attackDamageMultiplier;
        }
    }

    bool isInFOV(Vector2 vec)
	{
		float a = Vector2.Angle(transform.up, vec - (Vector2)transform.position);
        return a <= enemyData.fov;
    }

	public GameObject[] GetNeighbors(float radius)
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, FellowEnemyLm);
		GameObject[] neighborsFound = new GameObject[colliders.Length-1];

		int neighborsIndex = 0;
		for(int i = 0; i < colliders.Length; i++) 
		{
			if (colliders[i].gameObject != this.gameObject)
			{
				try
				{
					neighborsFound[neighborsIndex] = colliders[i].gameObject;
				}
				catch (Exception e)
				{
					Debug.Log("----");
					Debug.Log(neighborsFound.Length);
					Debug.Log(neighborsIndex);
					Debug.Log(i);
				}
				neighborsIndex++;

			}
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
		Optimizer.list.Remove(gameObject);
		Optimizer.currentActiveEnemies--;
		if(head) head.RemoveHat();
		ChangeState(State.DEAD);
	}

	public void PlayDroneSound()
	{
		if(voice != null)
		{
			int index = UnityEngine.Random.Range(0, voice.droneSounds.Count);
			audioSource.PlayOneShot(voice.droneSounds[index]);
		}
	}

	public void PlayAggroSound()
	{
		if (voice != null)
		{
			int index = UnityEngine.Random.Range(0, voice.aggroSounds.Count);
			audioSource.PlayOneShot(voice.aggroSounds[index]);
		}
	}

	public void PlayHurtSound()
	{
		if (voice != null)
		{
			int index = UnityEngine.Random.Range(0, voice.hurtSounds.Count);
			audioSource.PlayOneShot(voice.hurtSounds[index]);
		}
		Utils.MakeSoundWave(transform.position, 5);
	}

	public void OnHit(int damage, float dismemberChance = 0, float decapitateChance = 0)
    {
		if (currentState != State.AGGRO)
		{
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, GetPlayerSpotDistance(), allPlayerLm);
			if (playerCollider != null)
			{
				playerTarget = playerCollider.gameObject;
				ChangeState(State.AGGRO);
			}
		}

		PlayHurtSound();
		DismemberLimb(damage, dismemberChance);
		Decapitate(damage, decapitateChance);
    }

	private float limbLaunchMod = .1f;
	private float limbSpinForce = 80f;


    public void DismemberLimb(int damage, float dismemberChance)
	{
        if (limbs.Count > 0)
        {
			float num = UnityEngine.Random.Range(0f, 100f);
			if (num < dismemberChance)
			{
				Limb victimLimb = limbs[UnityEngine.Random.Range(0, limbs.Count)];
				victimLimb.DetachFromOwner();
				victimLimb.rb.bodyType = RigidbodyType2D.Dynamic;
				victimLimb.rb.AddForce(Utils.RandomUnitVector2() * damage * limbLaunchMod, ForceMode2D.Impulse);
				victimLimb.rb.angularVelocity = limbSpinForce * damage * UnityEngine.Random.Range(0.7f, 1);
			}
        }
    }


    public void Decapitate(int damage, float decapitateChance)
    {
        if (head != null)
        {
            float num = UnityEngine.Random.Range(0f, 100f);
            if (num < decapitateChance)
            {
				head.DetachFromOwner();
				if (UnityEngine.Random.Range((int)0, (int)10) != 0)
				{
					head.rb.bodyType = RigidbodyType2D.Dynamic;
					head.rb.AddForce(Utils.RandomUnitVector2() * damage * limbLaunchMod, ForceMode2D.Impulse);
					head.rb.angularVelocity = limbSpinForce * damage * UnityEngine.Random.Range(0.7f, 1);

				}
				else // 1 in 10 to get zombie head
				{
					Destroy(head.gameObject);
					UnityEngine.Object zombieHeadPrefab =  Resources.Load("Zombie Head");
					GameObject zombieHead = Instantiate(zombieHeadPrefab, head.transform.position, head.transform.rotation) as GameObject;
					ThrowingWeapon tw = zombieHead.GetComponent<ThrowingWeapon>();
					tw.throwingWeaponData.icon = head.spriteRenderer.sprite;
					tw.itemRenderer.sprite = head.spriteRenderer.sprite;
				}
				head = null;


				OnDeath();
				//StartCoroutine(DeathTimer(Random.Range(0.2f, 1f)));
            }
        }
    }

	public IEnumerator DeathTimer(float time)
	{
		yield return new WaitForSeconds(time);
		OnDeath();
	}

    private void OnCollisionStay2D(Collision2D collision)
	{
		if((AttackableMovementBlockersLm.value & (1 << collision.gameObject.layer)) != 0 && attackTimer <= 0 && rigidBody.linearVelocity.magnitude < 0.2) // stopped by door or window
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
		else if(collision.gameObject.CompareTag("Player") && playerTarget == null)
		{
			if(PlayerController.currentState != PlayerController.PlayerState.SNEAKING)
			{
				playerTarget = collision.gameObject;
				ChangeState(State.AGGRO);
			}
		}

        else if(collidingVehicle != null && rigidBody.linearVelocity.magnitude > 0)
		{
            if ((WorldObstacleLm & (1 << collision.gameObject.layer)) != 0)
			{
				float damage = rigidBody.linearVelocity.magnitude * 12;

				Vector2 popupVec = (collision.transform.position - transform.position).normalized;
				health.TakeDamage(damage, Vector2.zero, 40, false, popupVec, false, 10);

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
