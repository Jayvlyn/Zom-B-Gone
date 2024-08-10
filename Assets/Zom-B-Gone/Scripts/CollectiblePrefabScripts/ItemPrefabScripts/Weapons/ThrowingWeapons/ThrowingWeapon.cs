using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeapon : Weapon
{
    //[Header("Throwing Weapon Attributes")]
    
    [HideInInspector] public ThrowingWeaponData throwingWeaponData;

	private void Awake()
	{
		base.Awake();
		if (itemData as ThrowingWeaponData != null)
		{
			throwingWeaponData = (ThrowingWeaponData)itemData;
		}
		else Debug.Log("Invalid Data & Class Matchup");
	}

	public override void Use()
    {
        Throw();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == State.AIRBORNE && collision.gameObject.TryGetComponent(out Health collisionHealth))
        {
            collisionHealth.TakeDamage(Utils.MapWeightToRange(itemData.weight, 5, 100, false) + weaponData.damage, weaponData.dismemberChance);
        }
    }

}
