using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Firearm : Weapon
{
    private TMP_Text _ammoCount;
    private TMP_Text _reloadingIndicator;

    [Header("Firearm attributes")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected List<Transform> firePoints;
    [SerializeField] protected List<SpriteRenderer> muzzleFlashes;
    [SerializeField] protected Animator flashAnimator;
    [SerializeField] public int _maxAmmo = 10;
    [SerializeField] protected int _currentAmmo = 10;
    [SerializeField] protected int _ammoConsumption = 1;
    [SerializeField] protected float _reloadTime = 2; // Seconds
    [SerializeField] protected float _fireForce;
    public bool _reloading = false;

    private float _shotTimer = 0;

    [SerializeField] protected bool _isAutomatic; // Semi-Automatic or Automatic Gun?
    public bool IsAutomatic { get {  return _isAutomatic; } }

    private PlayerController _pc;

    public int CurrentAmmo
    {
        get { return _currentAmmo; }
        set { 
            _currentAmmo = value;
            _ammoCount.text = _currentAmmo + " / " + _maxAmmo;
        }
    }

    protected override void Update()
    {
        if(_shotTimer > 0)
        {
            _shotTimer -= Time.deltaTime;
            if(_shotTimer < 0)_shotTimer = 0;
        }

        if(_isAutomatic && _useHeld)
        {
            Use();
        }

        base.Update();
    }

    public override void Use()
    {
        if(_shotTimer <= 0)
        {
            Fire();
        }
    }

    public override void Drop()
    {
        _reloadingIndicator.enabled = false;
        base.Drop();
    }

    public override void Throw()
    {
        _reloadingIndicator.enabled = false;
        base.Throw();
    }

    public override void PickUp(Transform parent, bool rightHand)
    {
        if(parent.gameObject.TryGetComponent(out PlayerController pc)) _pc = pc;
        if(rightHand)
        {
            _ammoCount = GameObject.FindWithTag("RightAmmoCount").GetComponent<TMP_Text>();
            _reloadingIndicator = GameObject.FindWithTag("RightReloadingIndicator").GetComponent<TMP_Text>();
        }
        else
        {
            _ammoCount = GameObject.FindWithTag("LeftAmmoCount").GetComponent<TMP_Text>();
            _reloadingIndicator = GameObject.FindWithTag("LeftReloadingIndicator").GetComponent<TMP_Text>();
        }
        base.PickUp(parent, rightHand);
        CurrentAmmo = CurrentAmmo; // update count text
    }

    public void Fire()
    {
        if(CurrentAmmo > 0 && !_reloading)
        {
            flashAnimator.SetTrigger("Fire");

            _shotTimer = _attackSpeed;
            CurrentAmmo -= _ammoConsumption;
            foreach (Transform firepoint in firePoints) 
            { 
                var bullet = Instantiate(bulletPrefab, firepoint.transform.position, firepoint.transform.rotation);
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
        else if(CurrentAmmo <= 0)
        {
            StartReload(_pc._reloadSpeedReduction);
        }
    }


    public void StartReload(float mod = 1)
    {
        if(CurrentAmmo != _maxAmmo && !_reloading)
        {
            StartCoroutine(Reload(mod));
        }
    }

    private IEnumerator Reload(float mod = 1)
    {
        _reloading = true;
        _reloadingIndicator.enabled = true;
        CurrentAmmo = 0;
        yield return new WaitForSeconds(_reloadTime * mod);
        CurrentAmmo = _maxAmmo;
        _reloading = false;
        _reloadingIndicator.enabled = false;
    }
}