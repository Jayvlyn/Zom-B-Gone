using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeapon : Weapon
{
    public override void Use()
    {
        Throw();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (_currentState == State.AIRBORNE && collision.gameObject.TryGetComponent(out Health collisionHealth))
        {
            collisionHealth.TakeDamage(Utils.MapWeightToRange(itemData.weight, 5, 100, false) + weaponData.damage, weaponData.dismembering);
        }
    }

}
