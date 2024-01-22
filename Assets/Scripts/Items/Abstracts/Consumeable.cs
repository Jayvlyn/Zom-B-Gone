using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumeable : Item
{
    [SerializeField, Tooltip("Amount of time the consumable effect lasts")] float _effectTime;

    public override void Use()
    {
        // call base.Use() in consumables after their functionality
        Destroy(gameObject);
    }
}
