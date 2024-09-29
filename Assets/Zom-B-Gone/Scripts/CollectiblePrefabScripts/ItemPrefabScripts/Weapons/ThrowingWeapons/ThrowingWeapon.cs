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
        if(Quantity > 1)
        {
            Quantity--;
            GameObject prefab = Resources.Load<GameObject>(throwingWeaponData.name);
            GameObject thrownObject = Instantiate(prefab, transform.position, transform.rotation);
            Item thrownItem = thrownObject.GetComponent<Item>();
            thrownItem.ChangeState(ItemState.AIRBORNE);

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - new Vector2(transform.position.x, transform.position.y)).normalized;

            float throwForce = Utils.MapWeightToRange(thrownItem.itemData.weight, 10, 20, true);

            thrownItem.rb.velocity = direction * throwForce;

            if (spinThrow)
            {
                float spinForce = Utils.MapWeightToRange(itemData.weight, 100, 700, true);
                if (!inRightHand) spinForce *= -1;
                thrownItem.rb.angularVelocity = spinForce;
            }
        }
        else
        {
            Throw();
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == ItemState.AIRBORNE && collision.gameObject.TryGetComponent(out Health collisionHealth))
        {
            collisionHealth.TakeDamage(Utils.MapWeightToRange(itemData.weight, 5, 100, false) + weaponData.damage, weaponData.dismemberChance);
        }
    }

}
