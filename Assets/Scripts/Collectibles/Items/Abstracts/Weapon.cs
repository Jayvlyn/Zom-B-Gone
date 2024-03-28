
using UnityEngine;

public abstract class Weapon : Item
{
    [Header("Weapon attributes")]
    [SerializeField] protected int _damage;
    [SerializeField] protected float _range; // m / km
    [SerializeField] protected float _attackSpeed; // time between attacks
    public bool dismembering = false;

    protected void DealDamage(Health targetHealth)
    {
        float damage = _damage;
        #region hat buff
        if (playerHead.wornHat != null)
        {
            damage += playerHead.wornHat.damageIncrease;
            damage *= playerHead.wornHat.damageMultiplier;
        }
        #endregion
        targetHealth.TakeDamage(damage, dismembering);
    }
}
