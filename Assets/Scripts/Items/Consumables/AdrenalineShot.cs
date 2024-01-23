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
        _playerController._staminaRecoverySpeed = playerRecoverySpeed;
        _playerController._speedModifier = playerSpeedMod;
    }

    public void CachePlayerData()
    {
        playerRecoverySpeed = _playerController._staminaRecoverySpeed;
        playerSpeedMod = _playerController._speedModifier;
    }

    public void InstantStaminaRecovery()
    {
        if (_playerController._currentStamina + _instantStaminaRecovery <= _playerController._maxStamina)
            _playerController._currentStamina += _instantStaminaRecovery;
        else
            _playerController._currentStamina = _playerController._maxStamina;
    }

    public void StaminaRecoveryChange()
    {
        _playerController._staminaRecoverySpeed = _staminaRecoverySpeed;
    }

    public void MoveSpeedIncrease()
    {
        _playerController._speedModifier = _moveSpeedMod;
    }
}
