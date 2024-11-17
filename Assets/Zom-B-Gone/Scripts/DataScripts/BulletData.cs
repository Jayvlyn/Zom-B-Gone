using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

[CreateAssetMenu(fileName = "New Bullet", menuName = "New Bullet")]
public class BulletData : ScriptableObject
{
    public float damageMultiplier = 1;
    [Tooltip("How many enemies can the bullet pierce through?")] public int piercingPower = 0;
    public bool wallPiercing = false;
    public bool residual = false;
    public float spin = 0;

    public TransformEvent enterEvent;
    public TransformEvent exitEvent;
}
