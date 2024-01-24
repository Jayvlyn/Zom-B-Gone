using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Firearm
{
    public override void Use()
    {
        // Shoot gun!
        _playerController.gameObject.GetComponent<Health>().CurrentHealth = _playerController.gameObject.GetComponent<Health>().CurrentHealth - 1;
    }

    void Start()
    {
        
    }

    void Update()
    {
        base.Update();
    }
}
