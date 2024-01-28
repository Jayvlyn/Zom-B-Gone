
using UnityEngine;

public abstract class Weapon : Item
{
    [Header("Weapon attributes")]
    [SerializeField] protected int _damage;
    [SerializeField] protected float _range; // m / km
}
