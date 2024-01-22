using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Firearm
{
    public Pistol()
    {
        // Item
        _name = "Pistol";
        _quality = 100;
        _weight = 1; // Avg 9mm pistol weight
        // Weapon
        _damage = 35; // Regular zombie will have 100 health, takes 3 shots to kill
        _range = 50;//m
    }

    public override void Use()
    {
        // Shoot gun!
    }

    void Start()
    {
        
    }

    void Update()
    {
        base.Update();
    }
}
