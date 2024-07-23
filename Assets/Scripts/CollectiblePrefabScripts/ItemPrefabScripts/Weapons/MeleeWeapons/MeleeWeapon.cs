using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Properties")]
    [SerializeField] protected Collider2D damageCollider;

    [HideInInspector] public MeleeWeaponData meleeWeaponData;

    private bool isSwinging = false;
    private bool returnSwing = false;
    private bool doDamage = false;

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
        else returnSwing = true; FlipX();

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform == transform.parent) return;
        else if (doDamage && !collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out Health targetHealth))
        {
            // Damage
            DealDamage(targetHealth);
        }
    }
}
