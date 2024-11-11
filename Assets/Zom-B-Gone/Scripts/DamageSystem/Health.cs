using System;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField,Tooltip("Will be auto-filled for player")] Slider healthBar;
    [SerializeField,Range(1,1000)] int _maxHealth = 100;
    [SerializeField,Range(0,1000)] int _currentHealth = 100;

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
    public void TakeDamage(float damage, Vector2 knockbackVector, float dismemeberChance = 0, bool isCritical = false, Vector3 popupVector = default, bool invertPopupRotate = default, float decapitateChance = 0)
    {
        DamagePopup.PopupType popupType = DamagePopup.PopupType.DEFAULT;

        if (damage < 0) return;
        int incomingDamage = Mathf.RoundToInt(damage);
        #region hat buff
        if (gameObject.CompareTag("Player"))
        {
            popupType = DamagePopup.PopupType.PLAYER;

            if(gameObject.TryGetComponent(out Head head) && head.wornHat != null)
            {
                incomingDamage -= head.wornHat.hatData.defense;
            }
        }
        #endregion
        else if (gameObject.CompareTag("Enemy"))
        {
            popupType = DamagePopup.PopupType.ENEMY;

            if (enemyOwner == null) {
                enemyOwner = gameObject.GetComponent<Enemy>();
            }

            #region hat buff
            if(enemyOwner.head.wornHat != null)
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

        CurrentHealth = CurrentHealth - incomingDamage;

        DamagePopup.Create(transform.position, incomingDamage, popupVector, isCritical, invertPopupRotate, popupType);
    }

    public void OnDeath()
    {
        if(gameObject.TryGetComponent(out Enemy enemy)) 
        {
            enemy.OnDeath();
        }
        else if (!gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
