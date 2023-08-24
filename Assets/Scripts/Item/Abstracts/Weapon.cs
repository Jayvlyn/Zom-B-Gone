
using UnityEngine;

public abstract class Weapon : Item
{

    [SerializeField] protected int _damage;
    [SerializeField] protected float _range; // m / km


    protected Weapon()
    {
        _damage = 10;
        _range = 3;//m
    }

    protected Weapon(int damage, float range)
    {
        _damage = damage;
        _range = range;
    }
}
