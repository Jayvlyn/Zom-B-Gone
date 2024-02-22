using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Properties")]
    [SerializeField] private AnimationCurve swingCurve;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private AnimationCurve prepSwingCurve;
    [SerializeField] private AnimationCurve prepRotationCurve;
    [SerializeField,Tooltip("Time in seconds to complete a swing")] private float swingSpeed;
    [SerializeField,Tooltip("Time in seconds to prepare swing")] private float prepSpeed;

    private bool isSwinging = false;
    private bool returnSwing = false;

    public override void Use()
    {
        if (!isSwinging) StartCoroutine(PrepareSwing());
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
        _collider.isTrigger = false;
        returnSwing = false;
        isSwinging = false;
        StopAllCoroutines();
        base.RemoveFromHand();
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
        float elapsedTime = 0f;

        while (elapsedTime < swingSpeed)
        {
            float t = elapsedTime / swingSpeed;

            float swingValue = swingCurve.Evaluate(t);
            float rotationValue = rotationCurve.Evaluate(t);

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

        if (_useHeld) StartCoroutine(Swing());
        else StartCoroutine(FinishSwings());
    }

    private IEnumerator FinishSwings()
    {
        float returnTime = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < returnTime)
        {
            float t = elapsedTime / returnTime;

            // bring sword back to default holding position
            

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
			transform.Rotate(0, 0, -rotationIncrement * Time.deltaTime * 100);
		}
		else
		{
			transform.RotateAround(transform.parent.position, Vector3.forward, t * Time.deltaTime * 100);
			transform.Rotate(0, 0, rotationIncrement * Time.deltaTime * 100);
		}
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == transform.parent) return;
        if (isSwinging && collision.gameObject.TryGetComponent(out Health targetHealth))
        {
            targetHealth.TakeDamage(_damage);
        }
    }
}
