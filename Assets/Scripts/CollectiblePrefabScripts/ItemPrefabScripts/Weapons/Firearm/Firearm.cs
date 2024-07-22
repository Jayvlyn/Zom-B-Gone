using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Firearm : Weapon
{
    private TMP_Text ammoCount;
    private TMP_Text reloadingIndicator;

    public FirearmData firearmData;

    [Header("Firearm attributes")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected List<Transform> firePoints;
    [SerializeField] protected List<SpriteRenderer> muzzleFlashes;
    [SerializeField] protected Animator flashAnimator;

    [HideInInspector] public bool reloading = false;
    private float shotTimer = 0;
    protected int currentAmmo = 10;

    [SerializeField] protected bool _isAutomatic; // Semi-Automatic or Automatic Gun?
    public bool IsAutomatic { get {  return _isAutomatic; } }

    private PlayerController _pc;

    public int CurrentAmmo
    {
        get { return currentAmmo; }
        set { 
            currentAmmo = value;
            ammoCount.text = currentAmmo + " / " + firearmData.maxAmmo;
        }
    }

    protected override void Update()
    {
        if(shotTimer > 0)
        {
            shotTimer -= Time.deltaTime;
            if(shotTimer < 0)shotTimer = 0;
        }

        if(_isAutomatic && useHeld)
        {
            Use();
        }

        base.Update();
    }

    public override void Use()
    {
        if(shotTimer <= 0)
        {
            Fire();
        }
    }

    public override void Drop()
    {
        reloadingIndicator.enabled = false;
        base.Drop();
    }

    public override void Throw()
    {
        reloadingIndicator.enabled = false;
        base.Throw();
    }

    public override void PickUp(Transform parent, bool rightHand)
    {
        if(parent.gameObject.TryGetComponent(out PlayerController pc)) _pc = pc;
        if(rightHand)
        {
            ammoCount = GameObject.FindWithTag("RightAmmoCount").GetComponent<TMP_Text>();
            reloadingIndicator = GameObject.FindWithTag("RightReloadingIndicator").GetComponent<TMP_Text>();
        }
        else
        {
            ammoCount = GameObject.FindWithTag("LeftAmmoCount").GetComponent<TMP_Text>();
            reloadingIndicator = GameObject.FindWithTag("LeftReloadingIndicator").GetComponent<TMP_Text>();
        }
        base.PickUp(parent, rightHand);
        CurrentAmmo = CurrentAmmo; // update count text
    }

    public void Fire()
    {
        if(CurrentAmmo > 0 && !reloading)
        {
            flashAnimator.SetTrigger("Fire");

            shotTimer = weaponData.attackSpeed;
            CurrentAmmo -= firearmData.ammoConsumption;
            foreach (Transform firepoint in firePoints) 
            { 
                var bullet = Instantiate(bulletPrefab, firepoint.transform.position, firepoint.transform.rotation);
                
                if(bullet.TryGetComponent(out Rigidbody2D bulletRb))
                {
                    bulletRb.AddForce(transform.up * firearmData.fireForce, ForceMode2D.Impulse);
                }
                if(bullet.TryGetComponent(out Bullet bulletScript))
                {
                    bulletScript.shooter = this;
                    bulletScript.FirearmDamage = weaponData.damage;
                    bulletScript.LifeSpan = firearmData.range;
                }
            }
            
        }
        else if(CurrentAmmo <= 0)
        {
            StartReload(_pc.reloadSpeedReduction);
        }
    }


    public void StartReload(float mod = 1)
    {
        if(CurrentAmmo != firearmData.maxAmmo && !reloading)
        {
            StartCoroutine(Reload(mod));
        }
    }

    private IEnumerator Reload(float mod = 1)
    {
        reloading = true;
        reloadingIndicator.enabled = true;
        CurrentAmmo = 0;
        yield return new WaitForSeconds(firearmData.reloadTime * mod);
        CurrentAmmo = firearmData.maxAmmo;
        reloading = false;
        reloadingIndicator.enabled = false;
    }
}
