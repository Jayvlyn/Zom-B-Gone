using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalineShot : Consumeable
{
    [SerializeField] private float _instantStaminaRecovery = 0;
    [SerializeField] private float _staminaRecoverySpeed = 0;
    [SerializeField] private float _moveSpeedMod = 0;

    private float playerRecoverySpeed;
    private float playerSpeedMod;

    public override void Use()
    {
        CachePlayerData();

        InstantStaminaRecovery();

        StaminaRecoveryChange();

        MoveSpeedIncrease();
        
        base.Use(); // logic for consuming item // Calls restore player
    }
    public override void RestorePlayer()
    {
        playerController._staminaRecoverySpeed = playerRecoverySpeed;
        playerController._speedModifier = playerSpeedMod;
    }

    public void CachePlayerData()
    {
        playerRecoverySpeed = playerController._staminaRecoverySpeed;
        playerSpeedMod = playerController._speedModifier;
    }

    public void InstantStaminaRecovery()
    {
        if (playerController.currentStamina + _instantStaminaRecovery <= playerController._maxStamina)
            playerController.currentStamina += _instantStaminaRecovery;
        else
            playerController.currentStamina = playerController._maxStamina;
    }

    public void StaminaRecoveryChange()
    {
        playerController._staminaRecoverySpeed = _staminaRecoverySpeed;
    }

    public void MoveSpeedIncrease()
    {
        playerController._speedModifier = _moveSpeedMod;
    }
}
