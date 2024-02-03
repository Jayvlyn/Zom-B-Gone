using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Properties")]
    [SerializeField] private float swingRadius = 90f;
    public float swingSpeed = 5f;

    public override void Use()
    {
        StartCoroutine(Swing());
    }


    private bool isSwinging = false;
    private Transform playerTransform;
    private Quaternion initialRotation;

    void Start()
    {
        playerTransform = transform.parent;
        initialRotation = transform.rotation;
    }

    IEnumerator Swing()
    {
        isSwinging = true;

        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * swingSpeed;

            // Calculate the rotation based on the swing radius
            Quaternion targetRotation = initialRotation * Quaternion.Euler(0, 0, swingRadius * Mathf.Sin(elapsedTime * Mathf.PI));

            // Apply the rotation
            transform.rotation = targetRotation;

            yield return null;
        }

        // Reset rotation when the swing is complete
        transform.rotation = initialRotation;
        isSwinging = false;
    }
}
