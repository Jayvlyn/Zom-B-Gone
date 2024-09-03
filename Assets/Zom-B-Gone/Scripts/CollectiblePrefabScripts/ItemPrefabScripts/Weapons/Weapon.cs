
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

        targetHealth.TakeDamage(damage, weaponData.dismemberChance);

        Vector3 hitTargetPosition = targetHealth.transform.position;
        Vector3 popupVector = (hitTargetPosition - playerHead.transform.position).normalized * 10f;

        bool invertRotate = popupVector.x < 0; // invert when enemy is on left of player

        DamagePopup.Create(hitTargetPosition, damage, popupVector, false, invertRotate);

    }

    public void TryDealKnockback(Health targetHealth)
    {
        if (targetHealth.gameObject.TryGetComponent(out Rigidbody2D hitRb)) hitRb.AddForce(transform.parent.up * weaponData.knockbackPower, ForceMode2D.Impulse);
    }
}
