using System.Collections;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Properties")]
    [SerializeField] protected bool flipOnReturnSwing = true;
    [SerializeField] protected Collider2D damageCollider;
    [SerializeField] protected ParticleSystem bloodTrail;

    [HideInInspector] public MeleeWeaponData meleeWeaponData;

    private bool isSwinging = false;
    public bool IsSwinging { get => isSwinging; set => isSwinging = value; }

    private bool returnSwing = false;

    private bool doDamage = false;

    private int currentHitCount = 0;

    private void Awake()
	{
        base.Awake();
		if (itemData as MeleeWeaponData != null)
		{
			meleeWeaponData = (MeleeWeaponData)itemData;
		}
		else Debug.Log("Invalid Data & Class Matchup");
	}

    public override void Use()
    {
        if (!isSwinging && !moveToHand && StaminaCheck())
        {
            StartCoroutine(PrepareSwing());
        }
    }

	public override void PickUp(Transform parent, bool rightHand, bool adding = false)
	{
		base.PickUp(parent, rightHand);
    }

    public override void Drop()
    {
        base.Drop();
        RemoveFromHand();
    }

    public override void Throw()
    {
        base.Throw();
        RemoveFromHand();
    }

    protected override void RemoveFromHand()
    {
        returnSwing = false;
        isSwinging = false;
        StopAllCoroutines();
        base.RemoveFromHand();
    }

    private bool StaminaCheck()
    {
        return playerController.currentStamina >= meleeWeaponData.staminaCost;
    }

    private IEnumerator PrepareSwing()
    {
        isSwinging = true;

        float elapsedTime = 0f;

        while (elapsedTime < meleeWeaponData.prepSpeed)
        {
            float t = elapsedTime / meleeWeaponData.prepSpeed;

            float swingValue = meleeWeaponData.prepSwingCurve.Evaluate(t);
            float rotationValue = meleeWeaponData.prepRotationCurve.Evaluate(t);

            MoveSword(swingValue, rotationValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(Swing());
    }

	private IEnumerator Swing()
    {
        DrainStamina();
        currentHitCount = 0;

        PlaySwingSound();
        
        float elapsedTime = 0f;

        float swingTime = weaponData.attackSpeed;
		#region hat buff
		if (playerHead.wornHat != null)
        {
            swingTime *= playerHead.wornHat.hatData.swingTimeMultiplier;
        }
		#endregion

		while (elapsedTime < swingTime)
        {
            float t = elapsedTime / swingTime;

            float swingValue = meleeWeaponData.swingCurve.Evaluate(t);
            float rotationValue = meleeWeaponData.rotationCurve.Evaluate(t);

            #region hat buff
            if (playerHead.wornHat != null)
            {
                swingValue *= 2 - playerHead.wornHat.hatData.swingTimeMultiplier;
                rotationValue *= 2 - playerHead.wornHat.hatData.swingTimeMultiplier;
            }
            #endregion

            if (swingValue > 1) doDamage = true;
            else doDamage = false;

            if(returnSwing)
            {
                MoveSword(swingValue, rotationValue);
            }
            else
            {
                MoveSword(-swingValue, -rotationValue);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (returnSwing) returnSwing = false;
        else returnSwing = true; 
        
        if (flipOnReturnSwing) FlipX();

        if (useHeld && StaminaCheck())
        {
            StartCoroutine(Swing());
        }
        else
        {
            StartCoroutine(FinishSwings());

        }
    }

    private IEnumerator FinishSwings()
    {
        playerController.RecoverStamina();

        float returnTime = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < returnTime)
        {
            float t = elapsedTime / returnTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        PositionInHand();

        returnSwing = false;
        isSwinging = false;
    }

    private void MoveSword(float t, float rotationIncrement)
    {
		if (inRightHand)
		{
			transform.RotateAround(transform.parent.position, Vector3.forward, -t * Time.deltaTime * 100);
			//transform.Rotate(0, 0, -rotationIncrement * Time.deltaTime * 100);
            transform.RotateAround(pivotPoint.position, Vector3.forward, -rotationIncrement * Time.deltaTime * 100);
		}
		else
		{
			transform.RotateAround(transform.parent.position, Vector3.forward, t * Time.deltaTime * 100);
            //transform.Rotate(0, 0, rotationIncrement * Time.deltaTime * 100);
            transform.RotateAround(pivotPoint.position, Vector3.forward, rotationIncrement * Time.deltaTime * 100);
        }
	}

    private void DrainStamina()
    {
        playerController.recoverStamina = false;
		playerController.currentStamina -= meleeWeaponData.staminaCost;
	}

    private void PlaySwingSound()
    {
        if (meleeWeaponData.swingSounds.Count > 0)
        {
            int roll = Random.Range(0, meleeWeaponData.swingSounds.Count);
            audioSource.PlayOneShot(meleeWeaponData.swingSounds[roll]);
        }
    }


    private void PlayHitSound()
    {
        if (meleeWeaponData.hitSounds.Count > 0)
        {
            int roll = Random.Range(0, meleeWeaponData.hitSounds.Count);
            audioSource.PlayOneShot(meleeWeaponData.hitSounds[roll]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform == transform.parent) return;
        else if (doDamage && !collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out Health targetHealth))
        {
            Vector2 pos = playerController.transform.position;
            if (inRightHand) pos = pos + (Vector2)(playerController.transform.rotation * new Vector2(holdOffset.x, 0));
            else pos = pos + (Vector2)(playerController.transform.rotation * new Vector2(-holdOffset.x, 0));

            Vector2 dir = (Vector2)collision.transform.position - pos;

            RaycastHit2D hit = Physics2D.Raycast(pos, dir.normalized, dir.magnitude, useBlockersLm);
            if (hit.collider != null) return;


            bool hitIsEnemy = collision.gameObject.CompareTag("Enemy");

			// sound
			PlayHitSound();
            // blood particles
            if(bloodTrail)
            {
                if ((!bloodTrail.isPlaying || bloodTrail.time > bloodTrail.totalTime*0.5f) && hitIsEnemy)
                {
                    bloodTrail.Play();
                }
            }
            // effect
            if(hitIsEnemy && meleeWeaponData.effect != null)
            {
                Utils.ApplyEffect(meleeWeaponData.effect, collision.gameObject.GetComponent<Enemy>());
            }

            currentHitCount++;
			bool crit = Random.Range(0, currentHitCount + 2) == 0; // less likely to crit the more hits in you are

            float denominator = (currentHitCount + 1) * 0.5f;
            if(denominator < 1) denominator = 1;

            float damage = (weaponData.damage * 0.25f) + ((weaponData.damage * 0.75f) / denominator);
            DealDamage(targetHealth, damage, crit);
        }
    }
}
