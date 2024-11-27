using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingAmmo : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 10f;
    [SerializeField] private float explosionForce = 100f;
    [SerializeField] private int explosionDamage = 120;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Utils.CreateExplosion(transform.position, explosionRadius, explosionForce, explosionDamage);
    }
}
