
using UnityEngine;

public abstract class Weapon : Item
{
    //[Header("Weapon Attributes")]
    [HideInInspector] public WeaponData weaponData;

	protected void Awake()
	{
        base.Awake();
        if(itemData as WeaponData != null) 
        { 
            weaponData = (WeaponData)itemData;
        }
        else Debug.Log("Invalid Data & Class Matchup");
	}

	public void DealDamage(Health targetHealth)
    {
        float damage = weaponData.damage;

        TryDealKnockback(targetHealth);

        #region hat buff
        if (playerHead.wornHat != null)
        {
            damage += playerHead.wornHat.hatData.damageIncrease;
            damage *= playerHead.wornHat.hatData.damageMultiplier;
        }
        #endregion

        targetHealth.TakeDamage(damage, weaponData.dismembering);
    }

    public void TryDealKnockback(Health targetHealth)
    {
        if (targetHealth.gameObject.TryGetComponent(out Rigidbody2D hitRb)) hitRb.AddForce(transform.parent.up * weaponData.knockbackPower, ForceMode2D.Impulse);
    }
}
