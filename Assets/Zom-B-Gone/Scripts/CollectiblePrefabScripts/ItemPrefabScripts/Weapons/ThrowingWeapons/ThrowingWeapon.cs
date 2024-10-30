using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeapon : Weapon
{
    //[Header("Throwing Weapon Attributes")]
    
    [HideInInspector] public ThrowingWeaponData throwingWeaponData;

    protected Item lastThrownItem;


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
            lastThrownItem = thrownObject.GetComponent<Item>();
            lastThrownItem.ChangeState(ItemState.AIRBORNE);

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - new Vector2(transform.position.x, transform.position.y)).normalized;

            float throwForce = Utils.MapWeightToRange(lastThrownItem.itemData.weight, 10, 20, true);

            lastThrownItem.rb.linearVelocity = direction * throwForce;

            if (spinThrow)
            {
                float spinForce = Utils.MapWeightToRange(itemData.weight, 100, 700, true);
                if (!inRightHand) spinForce *= -1;
                lastThrownItem.rb.angularVelocity = spinForce;
            }
        }
        else
        {
            lastThrownItem = this;
            Throw();
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == ItemState.AIRBORNE && collision.gameObject.TryGetComponent(out Health collisionHealth))
        {
            Vector2 knockbackVector = (collisionHealth.transform.position - transform.position).normalized * weaponData.knockbackPower;
            collisionHealth.TakeDamage(Utils.MapWeightToRange(itemData.weight, 5, 100, false) + weaponData.damage, knockbackVector, weaponData.dismemberChance);
        }
    }

}
