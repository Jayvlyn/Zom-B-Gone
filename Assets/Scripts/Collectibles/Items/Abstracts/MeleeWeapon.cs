using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Properties")]
    [SerializeField] protected Collider2D damageCollider;
    [SerializeField] private float staminaCost = 1;
    [SerializeField] private AnimationCurve swingCurve;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private AnimationCurve prepSwingCurve;
    [SerializeField] private AnimationCurve prepRotationCurve;
    [SerializeField,Tooltip("Time in seconds to complete a swing")] private float swingSpeed;
    [SerializeField,Tooltip("Time in seconds to prepare swing")] private float prepSpeed;

    private bool isSwinging = false;
    private bool returnSwing = false;
    private bool doDamage = false;

    public override void Use()
    {
        if (!isSwinging && !moveToHand && StaminaCheck())
        {
            StartCoroutine(PrepareSwing());
        }
    }

	public override void PickUp(Transform parent, bool rightHand)
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
        return playerController.currentStamina >= staminaCost;
    }

    private IEnumerator PrepareSwing()
    {
        isSwinging = true;

        float elapsedTime = 0f;

        while (elapsedTime < prepSpeed)
        {
            float t = elapsedTime / prepSpeed;

            float swingValue = prepSwingCurve.Evaluate(t);
            float rotationValue = prepRotationCurve.Evaluate(t);

            MoveSword(swingValue, rotationValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(Swing());
    }

	private IEnumerator Swing()
    {
        DrainStamina();

        float elapsedTime = 0f;

        float swingTime = swingSpeed;
		#region hat buff
		if (playerHead.wornHat != null)
        {
            swingTime *= playerHead.wornHat.swingTimeMultiplier;
        }
		#endregion

		while (elapsedTime < swingTime)
        {
            float t = elapsedTime / swingTime;

            float swingValue = swingCurve.Evaluate(t);
            float rotationValue = rotationCurve.Evaluate(t);

            #region hat buff
            if (playerHead.wornHat != null)
            {
                swingValue *= 2 - playerHead.wornHat.swingTimeMultiplier;
                rotationValue *= 2 - playerHead.wornHat.swingTimeMultiplier;
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
        else returnSwing = true; FlipX();

        if (_useHeld && StaminaCheck())
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
		if (_inRightHand)
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
		playerController.currentStamina -= staminaCost;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform == transform.parent) return;
        else if (doDamage && !collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out Health targetHealth))
        {
            // Damage
            DealDamage(targetHealth);

            // Knockback
            if (targetHealth.gameObject.TryGetComponent(out Rigidbody2D hitRb)) hitRb.AddForce(transform.parent.up * knockbackPower, ForceMode2D.Impulse);
        }
    }
}
