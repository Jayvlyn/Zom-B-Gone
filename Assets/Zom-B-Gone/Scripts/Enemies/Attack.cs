using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private int damage;
    [NonSerialized] public float damageMultiplier = 1; // set by attacker, do not alter in this script
    private bool doDamage = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(doDamage && collision.TryGetComponent(out Health collisionHealth))
        {
            collisionHealth.TakeDamage(damage * damageMultiplier);
            doDamage = false;

            Vector3 hitTargetPosition = collisionHealth.transform.position;
            Vector3 popupVector = (hitTargetPosition - transform.position).normalized * 20f;

            bool invertRotate = popupVector.x < 0;

            DamagePopup.Create(hitTargetPosition, damage, popupVector, false, invertRotate, true);
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
