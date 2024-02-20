using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Properties")]
    [SerializeField] private float swingArc = 90.0f;
    [SerializeField] private float swingSpeed = 1.0f; // seconds to complete swing

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
        StopAllCoroutines();
        base.Drop();
    }

    public override void Throw()
    {
        StopAllCoroutines();
        base.Throw();
    }

    private IEnumerator PrepareSwing()
    {
        isSwinging = true;

        float preparationTime = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < preparationTime)
        {
            float t = elapsedTime / preparationTime;

            MoveSword(t, 1);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(Swing());
    }

	private IEnumerator Swing()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1)
        {
            float t = elapsedTime / 1;

            if(returnSwing)
            {
                MoveSword(t, swingSpeed);
            }
            else
            {
                MoveSword(-t, -swingSpeed);
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
			transform.RotateAround(transform.parent.position, Vector3.forward, -t);
			transform.Rotate(0, 0, -rotationIncrement);
		}
		else
		{
			transform.RotateAround(transform.parent.position, Vector3.forward, t);
			transform.Rotate(0, 0, rotationIncrement);
		}
	}
}
