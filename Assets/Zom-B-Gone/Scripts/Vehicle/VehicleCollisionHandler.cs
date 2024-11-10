using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCollisionHandler : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vehicle v;

    public float bufferTime = 2;
    private List<GameObject> damageBuffer = new List<GameObject>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject otherObject = collision.gameObject;

        if(!damageBuffer.Contains(otherObject))
        {
            if(otherObject.CompareTag("Enemy"))
            {
                if (rb.linearVelocity.magnitude > 2)
                {
                    if (otherObject.TryGetComponent(out Health health))
                    {
                        StartCoroutine(BufferTimer(otherObject, bufferTime));
                        DealVehicleDamage(health);
                    }
                }
            }
        }
    }

    private void DealVehicleDamage(Health health)
    {
        int damage = Mathf.FloorToInt(rb.linearVelocity.magnitude * 5);

        float lateralVel = 0;
        bool braking = false;
        if (v.IsTireScreeching(out lateralVel, out braking))
        {
            if (!braking)
            {
                damage += Mathf.FloorToInt(Mathf.Pow(lateralVel,2f));
            }
        }

        //float dot = Mathf.Clamp(Vector2.Dot(rb.linearVelocity.normalized, (health.gameObject.transform.position - transform.position).normalized), 0, 1);

        //float fDamage = damage;
        //fDamage *= dot;

        //damage = Mathf.RoundToInt(fDamage);

        Vector2 knockbackVector = ((Vector2)health.transform.position - rb.ClosestPoint(health.transform.position)).normalized;
        knockbackVector *= 50;

        float dismemberChance = 0;
        if (damage > 20) dismemberChance = damage;

        health.TakeDamage(damage, knockbackVector, damage);
    }

    private IEnumerator BufferTimer(GameObject g, float time)
    {
        damageBuffer.Add(g);
        yield return new WaitForSeconds(time);
        damageBuffer.Remove(g);
    }
}
