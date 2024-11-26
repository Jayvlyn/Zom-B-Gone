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

	public void DealDamage(Health targetHealth, float inputDamage = -1, bool crit = false)
    {
        if(targetHealth.glass)
        {
            Vector2 playerDir = (playerController.gameObject.transform.position - targetHealth.transform.position).normalized;
			Vector2 upDir = targetHealth.transform.parent.up.normalized;
			float dot = Vector2.Dot(playerDir, upDir);
			if (dot > 0) targetHealth.glass.window.ShatterGlass(true);
			else targetHealth.glass.window.ShatterGlass(false);
        }
        else
        {
            float damage = weaponData.damage;
            if (inputDamage != -1) damage = inputDamage;
            #region hat buff
            if (playerHead.wornHat != null)
            {
                damage += playerHead.wornHat.hatData.damageIncrease;
                damage *= playerHead.wornHat.hatData.damageMultiplier;
            }
            #endregion

            Vector3 popupVector = (targetHealth.transform.position - playerHead.transform.position).normalized * 20f;
            bool invertRotate = popupVector.x < 0; // invert when enemy is on left of player

            Vector2 knockbackVector = (targetHealth.transform.position - playerHead.transform.position).normalized * weaponData.knockbackPower;

		    targetHealth.TakeDamage(damage, knockbackVector, weaponData.dismemberChance, crit, popupVector, invertRotate, weaponData.dismemberChance/3);
        }

    }
}
