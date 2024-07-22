
using UnityEngine;

public abstract class Weapon : Item
{
    [SerializeField] public WeaponData weaponData;

    protected void DealDamage(Health targetHealth)
    {
        float damage = weaponData.damage;
        #region hat buff
        if (playerHead.wornHat != null)
        {
            damage += playerHead.wornHat.hatData.damageIncrease;
            damage *= playerHead.wornHat.hatData.damageMultiplier;
        }
        #endregion
        targetHealth.TakeDamage(damage, weaponData.dismembering);
    }
}
