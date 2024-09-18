
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

	public void DealDamage(Health targetHealth, float inputDamage = -1)
    {
        float damage = weaponData.damage;
        if (inputDamage != -1) damage = inputDamage;

        TryDealKnockback(targetHealth);

        #region hat buff
        if (playerHead.wornHat != null)
        {
            damage += playerHead.wornHat.hatData.damageIncrease;
            damage *= playerHead.wornHat.hatData.damageMultiplier;
        }
        #endregion

        Vector3 popupVector = (targetHealth.transform.position - playerHead.transform.position).normalized * 20f;
        bool invertRotate = popupVector.x < 0; // invert when enemy is on left of player

        targetHealth.TakeDamage(damage, weaponData.dismemberChance, 1, false, popupVector, invertRotate);
    }

    public void TryDealKnockback(Health targetHealth)
    {
        if (targetHealth && targetHealth.gameObject.TryGetComponent(out Rigidbody2D hitRb)) hitRb.AddForce(transform.parent.up * weaponData.knockbackPower, ForceMode2D.Impulse);
    }
}
