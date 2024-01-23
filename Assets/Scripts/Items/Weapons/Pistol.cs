using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Firearm
{
    public override void Use()
    {
        // Shoot gun!
    }

    void Start()
    {
        
    }

    void Update()
    {
        Debug.Log(_currentState.ToString());
        base.Update();
    }
}
