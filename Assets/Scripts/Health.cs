using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField,Range(1,1000)] int _maxHealth = 100;
    [SerializeField,Range(0,1000)] int _currentHealth = 100;

    private void Start()
    {
        if(gameObject.tag == "Player")
        {
            healthBar = GameObject.FindWithTag("Health").GetComponent<Slider>();
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
            if (value > _maxHealth) _currentHealth = _maxHealth;
            else if (value < 0) _currentHealth = 0;
            else _currentHealth = value;
            
            if(healthBar != null)
            {
                healthBar.value = _currentHealth / (float)_maxHealth;
            }
        }
    }
}
