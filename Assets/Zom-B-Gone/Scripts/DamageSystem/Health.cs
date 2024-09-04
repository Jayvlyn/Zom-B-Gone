using System;
using System.Collections;
using System.Collections.Generic;
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
    public void TakeDamage(int damage, float dismemeberChance = 0, float knockback = 1)
    {
        CurrentHealth = CurrentHealth - damage;
        if(gameObject.CompareTag("Enemy"))
        {
            if(enemyOwner != null) enemyOwner.OnHit(damage, dismemeberChance);
            else if(gameObject.TryGetComponent(out Enemy enemy))
            {
                enemyOwner = enemy;
                enemyOwner.OnHit(damage, dismemeberChance);
            }
            
        }
    }

    public void TakeDamage(float damage, float dismemeberChance = 0, float knockback = 1)
    {
        float incomingDamage = damage;
        #region hat buff
        if (gameObject.CompareTag("Player") && gameObject.TryGetComponent(out Head head) && head.wornHat != null)
        {
            incomingDamage -= head.wornHat.hatData.defense;
        }
        #endregion
        TakeDamage(Mathf.RoundToInt(incomingDamage), dismemeberChance, knockback);
    }

    public void OnDeath()
    {
        if(gameObject.TryGetComponent(out Enemy enemy)) 
        {
            enemy.OnDeath();
            //Destroy(gameObject);
        }
    }
}
