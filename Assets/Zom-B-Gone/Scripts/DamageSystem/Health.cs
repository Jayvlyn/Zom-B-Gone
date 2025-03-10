using System;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField,Tooltip("Will be auto-filled for player")] Slider healthBar;
    [SerializeField,Range(1,1000)] int _maxHealth = 100;
    [SerializeField,Range(0,1000)] int _currentHealth = 100;

    [Header("Only fill for window's health")]
    public Glass glass;

    private void Start()
    {
        if(gameObject.CompareTag("Player"))
        {
            healthBar = GameObject.FindWithTag("Health").GetComponent<Slider>();
        }
        else
        {
            if (healthBar != null)
                healthBar.gameObject.SetActive(false);
        }
    }

    public int MaxHealth {  
        get { return _maxHealth; } 
        set {  _maxHealth = value; } 
    }

    public int CurrentHealth
    {
        get { return _currentHealth; }
        set {
            if (value >= _maxHealth)
            {
                _currentHealth = _maxHealth;
                
			}
            else if (value <= 0)
            {
                _currentHealth = 0;
                if (!gameObject.CompareTag("Player") && healthBar != null) 
                    healthBar.gameObject.SetActive(false);
                OnDeath();
            }
            else _currentHealth = value;
            
            if(healthBar != null)
            {
                healthBar.value = _currentHealth / (float)_maxHealth;
				if (!gameObject.CompareTag("Player") && !healthBar.gameObject.activeSelf && _currentHealth < _maxHealth && _currentHealth > 0) 
                    healthBar.gameObject.SetActive(true);
			}
        }
    }

    private Enemy enemyOwner;
    private Vector2 mostRecentPopupVec;
    public void TakeDamage(float damage, Vector2 knockbackVector, float dismemeberChance = 0, bool isCritical = false, Vector3 popupVector = default, bool invertPopupRotate = default, float decapitateChance = 0, Color popupColor = default)
    {
        mostRecentPopupVec = popupVector;
        DamagePopup.PopupType popupType = DamagePopup.PopupType.DEFAULT;

        if (damage <= 0) return;
        float cr = damage;
        if (isCritical) cr *= 1.6f;
        int incomingDamage = Mathf.RoundToInt(cr);
        if (gameObject.TryGetComponent(out PlayerController pc))
        {
            popupType = DamagePopup.PopupType.PLAYER;


            #region hat buff
            if(pc.head.wornHat != null)
            {
                incomingDamage -= pc.head.wornHat.hatData.defense;
            }
            #endregion

            if(incomingDamage > 0)
            {
                // Camera Shake
                CameraShakeManager.instance.CameraShake(pc.impulseSource, CodeMonkey.Assets.i.playerDamagedSSP, popupVector.normalized);

                // Damage Sound
                AudioManager.Instance.Play(CodeMonkey.Assets.i.playeDamageSounds[UnityEngine.Random.Range(0, CodeMonkey.Assets.i.playeDamageSounds.Length)]);

                // Blood Vignette
                float damageEffectIntensity = Mathf.Clamp01((float)incomingDamage / 50);
			    ScreenDamageEffectController.DamageEffect.DoDamageEffect(damageEffectIntensity);
            }
		}
        else if (gameObject.CompareTag("Enemy"))
        {
            popupType = DamagePopup.PopupType.ENEMY;

            if (enemyOwner == null) {
                enemyOwner = gameObject.GetComponent<Enemy>();
            }

            #region hat buff
            if(enemyOwner.head != null && enemyOwner.head.wornHat != null)
            {
                incomingDamage -= enemyOwner.head.wornHat.hatData.defense;
            }
            #endregion

			if (enemyOwner.rigidBody)
			{
				enemyOwner.rigidBody.AddForce(knockbackVector);
			}

            enemyOwner.OnHit(incomingDamage, dismemeberChance, decapitateChance);

		}
        else if(knockbackVector != Vector2.zero && gameObject.CompareTag("Obstacle"))
        {
            if(gameObject.TryGetComponent(out Rigidbody2D rb))
            {
                rb.AddForce(knockbackVector * 100);
            }
        }

        if (incomingDamage < 0) return;

        CurrentHealth = CurrentHealth - incomingDamage;

        DamagePopup.Create(transform.position, incomingDamage, popupVector, isCritical, invertPopupRotate, popupType, popupColor);
    }

    public void OnDeath()
    {
        if(gameObject.TryGetComponent(out Enemy enemy)) 
        {
            enemy.OnDeath();
        }
        else if(gameObject.TryGetComponent(out Glass glass))
        {
            Vector2 upDir = gameObject.transform.parent.up.normalized;
            float dot = Vector2.Dot(mostRecentPopupVec.normalized, upDir);

            if (dot > 0) glass.window.ShatterGlass(false);
            else glass.window.ShatterGlass(true);
        }
        else if (gameObject.CompareTag("Player"))
        {
            if(!PlayerController.godMode) CodeMonkey.Assets.i.onPlayerDied.Raise();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
