using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Properties")]
    [SerializeField] private float swingArc = 90.0f;
    [SerializeField] private float swingSpeed = 1.0f; // seconds to complete swing

    [SerializeField] private Transform pivotPoint;

    private bool isSwinging = false;

    public override void Use()
    {
        if (!isSwinging) StartCoroutine(PrepareSwing());
    }

	public override void PickUp(Transform parent, bool rightHand)
	{
		base.PickUp(parent, rightHand);
        if (inRightHand) transform.RotateAround(pivotPoint.position, Vector3.forward, -130);
        else             transform.RotateAround(pivotPoint.position, Vector3.forward, 130);
    }

	private IEnumerator Swing()
    {
        float elapsedTime = 0f;

        while (elapsedTime < swingSpeed)
        {
            float t = elapsedTime / swingArc;

            transform.RotateAround(transform.parent.position, Vector3.forward, 1);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (isHeld) StartCoroutine(Swing());
        else isSwinging = false;
    }

    private IEnumerator PrepareSwing()
    {
        isSwinging = true;

        float preparationTime = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < preparationTime)
        {
            float t = elapsedTime / preparationTime;

            transform.RotateAround(transform.parent.position, Vector3.forward, 1);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(Swing());
    }
}
