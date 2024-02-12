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
    private Vector2 initialOffset;

    public override void Use()
    {
        if (!isSwinging)
        {
            StartCoroutine(PrepareSwing());
        }
    }

	public override void PickUp(Transform parent, bool rightHand)
	{
		base.PickUp(parent, rightHand);
        transform.RotateAround(pivotPoint.position, Vector3.forward, 180);
	}

	private IEnumerator Swing()
    {
        float elapsedTime = 0f;

        while (elapsedTime < swingSpeed)
        {

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isSwinging = false;
    }

    private IEnumerator PrepareSwing()
    {
        isSwinging = true;

        float preparationTime = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < preparationTime)
        {
            float t = elapsedTime / preparationTime;

            transform.RotateAround(transform.parent.position, Vector3.forward, -t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(Swing());
    }

}
