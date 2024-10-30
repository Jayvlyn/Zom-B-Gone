using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int knockback = 1;
    [NonSerialized] public float damageMultiplier = 1; // set by attacker, do not alter in this script
    private bool doDamage = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(doDamage && collision.TryGetComponent(out Health collisionHealth))
        {
            doDamage = false;

            Vector3 popupVector = (collisionHealth.transform.position - transform.position).normalized * 20f;
            bool invertRotate = popupVector.x < 0;

            collisionHealth.TakeDamage(damage * damageMultiplier, Vector2.zero, 0, false, popupVector, invertRotate);

        }
    }

    #region animation-events

    public void OnDamage()
    {
        doDamage = true;
    }

    public void OnAnimEnd()
    {
        Destroy(gameObject);
    }

    #endregion
}
