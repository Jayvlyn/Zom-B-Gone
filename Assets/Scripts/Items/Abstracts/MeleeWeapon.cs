using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Properties")]
    [SerializeField] private float swingArc = 90.0f;
    [SerializeField] private float swingSpeed = 1.0f; // seconds to complete swing

    private bool isSwinging = false;
    private Vector2 initialOffset;

    public override void Use()
    {
        if (!isSwinging)
        {
            StartCoroutine(PrepareSwing());
        }
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

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(Swing());
    }

}
