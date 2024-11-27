using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [Header("Projectile Weapon attributes")]
    [SerializeField] protected List<Transform> firePoints;
    [SerializeField] protected List<SpriteRenderer> muzzleFlashes;
    [SerializeField] protected Animator flashAnimator;
    [SerializeField] protected GameObject muzzleLight;

    [HideInInspector] public ProjectileWeaponData projectileWeaponData;
    [HideInInspector] public bool reloading = false;
    private float shotTimer = 0;
    protected int currentAmmo;

    private CinemachineImpulseSource impulseSource;

    [SerializeField,Tooltip("Will use the sprites below when checked to display a loaded version of the weapon")] bool hasLoadedSprite;
    [SerializeField, Tooltip("Secondary sprite that shows the weapon in a loaded state.")] protected Sprite loadedSprite;
    [SerializeField, Tooltip("If sprite renderer sprite is set to something, it will be used for this sprite")] protected Sprite unloadedSprite;

    public bool IsAutomatic { get {  return projectileWeaponData.isAutomatic; } }

    public int CurrentAmmo
    {
        get { return currentAmmo; }
        set { 
            currentAmmo = value;
            UpdateAmmoCount();
        }
    }

	private void Awake()
	{
        base.Awake();
		if (itemData as ProjectileWeaponData != null)
		{
			projectileWeaponData = (ProjectileWeaponData)itemData;
            currentAmmo = projectileWeaponData.maxAmmo;
            if(itemRenderer.sprite != null) unloadedSprite = itemRenderer.sprite;

            if(hasLoadedSprite) itemRenderer.sprite = loadedSprite;
            
		}
		else Debug.Log("Invalid Data & Class Matchup");

        if(impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }
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

    private void OnDisable()
    {
        if (reloading)
        {
            CancelReload();
        }
    }

    public override void Use()
    {
        if(shotTimer <= 0)
        {
            // Raycast to check if there is a wall between the player and the end of the weapon. Player shouldn't be able to fire weapon if it is clipping through wall
            foreach (Transform firepoint in firePoints)
            {
                Vector2 pos = playerController.transform.position;
                if (inRightHand) pos = pos + (Vector2)(playerController.transform.rotation * new Vector2(holdOffset.x, 0));
                else pos = pos + (Vector2)(playerController.transform.rotation * new Vector2(-holdOffset.x, 0));

                Vector2 dir = (Vector2)firepoint.position - pos;
            
                RaycastHit2D hit = Physics2D.Raycast(pos, dir.normalized, dir.magnitude, useBlockersLm);
                if (hit.collider != null) return; // cancel use if it hits a use blocker on the way to the firepoint
            }

            Fire();
        }
    }

    private void SetReloadIndicator(bool enabled)
    {
		if (inRightHand) playerHands.rightReloadingIndicator.enabled = enabled;
		else playerHands.leftReloadingIndicator.enabled = enabled;
	}

    private void PreRemoveFromHand()
    {
        SetReloadIndicator(false);
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


    public override void PickUp(Transform parent, bool rightHand, bool adding = false)
    {
        if(parent.gameObject.TryGetComponent(out PlayerController pc)) playerController = pc;
        base.PickUp(parent, rightHand);
        CurrentAmmo = CurrentAmmo; // to update count text when property changes
        shotTimer = 0.2f;
    }

    public void Fire()
    {
        if(CurrentAmmo > 0 && !reloading)
        {
            PlayShootSound();
            if(projectileWeaponData.noiseRadius > 16 && spawnTimerRoutine == null)
            {
                spawnTimerRoutine = StartCoroutine(ArtificalSpawnTimer());
            }

            float angle = transform.eulerAngles.z;
            Vector2 direction = new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
            Vector3 shakeVelocity = new Vector3(direction.x, -direction.y, 0);

            CameraShakeManager.instance.CameraShake(impulseSource, projectileWeaponData.ssp, shakeVelocity);

            if(flashAnimator) flashAnimator.SetTrigger("Fire");

            if (muzzleLight)
            {
                float flashTime = 0.1f;
                if (projectileWeaponData.attackSpeed <= flashTime) flashTime = projectileWeaponData.attackSpeed * 0.5f;

                if(muzzleLightRoutine == null)
                {
                    muzzleLightRoutine = StartCoroutine(DoMuzzleLight(flashTime));
                }
            }

            shotTimer = weaponData.attackSpeed;
            CurrentAmmo -= projectileWeaponData.ammoConsumption;
            foreach (Transform firepoint in firePoints) 
            { 
                var bullet = Instantiate(projectileWeaponData.bulletPrefab, firepoint.transform.position, firepoint.transform.rotation);
                
                if(bullet.TryGetComponent(out Rigidbody2D bulletRb))
                {
                    bulletRb.AddForce(bullet.transform.up * projectileWeaponData.fireForce, ForceMode2D.Impulse);
                }
                if(bullet.TryGetComponent(out Bullet bulletScript))
                {
                    bulletScript.shooter = this;
                    bulletScript.ProjectileWeaponDamage = weaponData.damage;
                    bulletScript.LifeSpan = projectileWeaponData.range;
                }
            }

            if(hasLoadedSprite && CurrentAmmo <= 0)
            {
                itemRenderer.sprite = unloadedSprite;
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
        StartReload();
        yield return new WaitForSeconds(projectileWeaponData.reloadTime * mod);
        FinishReload();
    }

    private void StartReload()
    {
        SetReloadIndicator(true);
        PlayReloadStart();
        reloading = true;
        CurrentAmmo = 0;
    }

    private void FinishReload()
    {
        PlayPickupSound(); // for projectile weapons should be prime sound
        if (hasLoadedSprite) itemRenderer.sprite = loadedSprite;
        CurrentAmmo = projectileWeaponData.maxAmmo;
        reloading = false;
        SetReloadIndicator(false);
    }

    private void CancelReload()
    {
        reloading = false;
        SetReloadIndicator(false);
    }

    private Coroutine muzzleLightRoutine;
    private IEnumerator DoMuzzleLight(float time)
    {
        muzzleLight.SetActive(true);
        yield return new WaitForSeconds(time);
        muzzleLight.SetActive(false);
        muzzleLightRoutine = null;
    }

	public void UpdateAmmoCount()
	{
		if (inRightHand) playerHands.rightAmmoCount.text = currentAmmo + " / " + projectileWeaponData.maxAmmo;
		else playerHands.leftAmmoCount.text = currentAmmo + " / " + projectileWeaponData.maxAmmo;
	}

    public void PlayShootSound()
    {
        if(projectileWeaponData.shootSounds.Count > 0)
        {
            int roll = Random.Range(0, projectileWeaponData.shootSounds.Count);
            audioSource.PlayOneShot(projectileWeaponData.shootSounds[roll]);
        }
        float noise = projectileWeaponData.noiseRadius;
        if (projectileWeaponData.suppressed)
        {
            float dec = (100-projectileWeaponData.suppressionPercentage) * 0.01f;
            noise *= dec;
        }
        Utils.MakeSoundWave(transform.position, noise);
    }

    public void PlayReloadStart()
    {
        if(projectileWeaponData.reloadStart)
        {
            audioSource.PlayOneShot(projectileWeaponData.reloadStart);
        }
        Utils.MakeSoundWave(transform.position, projectileWeaponData.reloadNoiseRadius, PlayerController.isSneaking);
    }

    private Coroutine spawnTimerRoutine;
    private float artificalSpawnTimer = 3;
    private IEnumerator ArtificalSpawnTimer()
    {
        EnemySpawner.TrySpawnEnemy();
        yield return new WaitForSeconds(artificalSpawnTimer);
        spawnTimerRoutine = null;
    }
}
