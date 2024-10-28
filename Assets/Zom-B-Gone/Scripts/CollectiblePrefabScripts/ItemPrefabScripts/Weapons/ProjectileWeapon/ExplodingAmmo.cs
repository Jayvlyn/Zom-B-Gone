using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingAmmo : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 10f;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private int explosionDamage = 40;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Utils.CreateExplosion(transform.position, explosionRadius, explosionForce, explosionDamage);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Utils.CreateExplosion(transform.position, explosionRadius, explosionForce, explosionDamage);
    }
}
