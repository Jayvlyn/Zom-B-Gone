using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Firearm : Weapon
{
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected int _maxAmmo;
    [SerializeField] protected int _currentAmmo;
    [SerializeField] protected float _reloadTime; // Seconds
    [SerializeField] protected bool _isAutomatic; // Semi-Automatic or Automatic Gun?


    protected Firearm()
    {
        _maxAmmo = 10;
        _currentAmmo = _maxAmmo;
        _reloadTime = 2;
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
