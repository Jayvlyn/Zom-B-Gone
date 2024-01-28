using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firearm : Weapon
{
    [Header("Firearm attributes")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected List<Transform> firePoints;
    [SerializeField] protected int _maxAmmo = 10;
    [SerializeField] protected int _currentAmmo = 10;
    [SerializeField] protected float _reloadTime = 2; // Seconds
    [SerializeField] protected bool _isAutomatic; // Semi-Automatic or Automatic Gun?
    [SerializeField] protected float _fireForce;

    public override void Use()
    {
        Fire();
    }

    public void Fire()
    {
        foreach (Transform firepoint in firePoints) 
        { 
            var bullet = Instantiate(bulletPrefab, firepoint.transform.position, firepoint.transform.rotation);
            _currentAmmo -= 1;
            if(bullet.TryGetComponent(out Rigidbody2D bulletRb))
            {
                bulletRb.AddForce(transform.up * _fireForce, ForceMode2D.Impulse);
            }
            if(bullet.TryGetComponent(out Bullet bulletScript))
            {
                bulletScript.FirearmDamage = _damage;
                bulletScript.LifeSpan = _range;
            }

        }
    }

    public void StartReload()
    {
        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(_reloadTime);
        _currentAmmo = _maxAmmo;
    }
}
