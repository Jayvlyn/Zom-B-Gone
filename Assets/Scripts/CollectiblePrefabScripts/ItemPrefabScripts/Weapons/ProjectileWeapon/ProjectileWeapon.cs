using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    private TMP_Text ammoCount;
    private TMP_Text reloadingIndicator;


    [Header("Projectile Weapon attributes")]
    [SerializeField] protected List<Transform> firePoints;
    [SerializeField] protected List<SpriteRenderer> muzzleFlashes;
    [SerializeField] protected Animator flashAnimator;

    [HideInInspector] public ProjectileWeaponData projectileWeaponData;
    [HideInInspector] public bool reloading = false;
    private float shotTimer = 0;
    protected int currentAmmo;


    public bool IsAutomatic { get {  return projectileWeaponData.isAutomatic; } }

    public int CurrentAmmo
    {
        get { return currentAmmo; }
        set { 
            currentAmmo = value;
            ammoCount.text = currentAmmo + " / " + projectileWeaponData.maxAmmo;
        }
    }

	private void Awake()
	{
        base.Awake();
		if (itemData as ProjectileWeaponData != null)
		{
			projectileWeaponData = (ProjectileWeaponData)itemData;
            currentAmmo = projectileWeaponData.maxAmmo;
		}
		else Debug.Log("Invalid Data & Class Matchup");
	}

	protected override void Update()
    {
        if(shotTimer > 0)
        {
            shotTimer -= Time.deltaTime;
            if(shotTimer < 0)shotTimer = 0;
        }

        if(projectileWeaponData.isAutomatic && useHeld)
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

    private void PreRemoveFromHand()
    {
        reloadingIndicator.enabled = false;
    }

    public override void Drop()
    {
        PreRemoveFromHand();
        base.Drop();
    }

    public override void Throw()
    {
        PreRemoveFromHand();
        base.Throw();
    }


    public override void PickUp(Transform parent, bool rightHand)
    {
        if(parent.gameObject.TryGetComponent(out PlayerController pc)) playerController = pc;
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
            CurrentAmmo -= projectileWeaponData.ammoConsumption;
            foreach (Transform firepoint in firePoints) 
            { 
                var bullet = Instantiate(projectileWeaponData.bulletPrefab, firepoint.transform.position, firepoint.transform.rotation);
                
                if(bullet.TryGetComponent(out Rigidbody2D bulletRb))
                {
                    bulletRb.AddForce(transform.up * projectileWeaponData.fireForce, ForceMode2D.Impulse);
                }
                if(bullet.TryGetComponent(out Bullet bulletScript))
                {
                    bulletScript.shooter = this;
                    bulletScript.ProjectileWeaponDamage = weaponData.damage;
                    bulletScript.LifeSpan = projectileWeaponData.range;
                }
            }
            
        }
        else if(CurrentAmmo <= 0)
        {
            StartReload(playerData.reloadSpeedReduction);
        }
    }


    public void StartReload(float mod = 1)
    {
        if(CurrentAmmo != projectileWeaponData.maxAmmo && !reloading)
        {
            StartCoroutine(Reload(mod));
        }
    }

    private IEnumerator Reload(float mod = 1)
    {
        reloading = true;
        reloadingIndicator.enabled = true;
        CurrentAmmo = 0;
        yield return new WaitForSeconds(projectileWeaponData.reloadTime * mod);
        CurrentAmmo = projectileWeaponData.maxAmmo;
        reloading = false;
        reloadingIndicator.enabled = false;
    }
}
