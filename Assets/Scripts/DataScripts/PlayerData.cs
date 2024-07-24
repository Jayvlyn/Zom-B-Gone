using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Player")]
public class PlayerData : ScriptableObject
{
    [Header("Stats")]
    public float walkSpeed = 5;
    public float runSpeed = 9;
    public float sneakSpeed = 3;
    public float speedModifier = 1;
    public float maxStamina = 15;
    public float staminaRecoverySpeed = 2;
    public float staminaRecoveryDelay = 2f;
    public float velocityChangeSpeed = 0.17f;
    public float reloadSpeedReduction = 1f;
}
