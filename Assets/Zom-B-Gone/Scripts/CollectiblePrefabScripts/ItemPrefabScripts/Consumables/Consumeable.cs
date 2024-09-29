using System.Collections;
using UnityEngine;

public abstract class Consumeable : Item
{
    [HideInInspector] public ConsumableData consumableData;

    // Cached data to restore to original stats
    private float cachedPlayerRecoverySpeed;
    private float cahcedPlayerSpeedMod;

    private void Awake()
    {
        base.Awake();
        if (itemData as ConsumableData != null)
        {
            consumableData = (ConsumableData)itemData;
        }
        else Debug.Log("Invalid Data & Class Matchup");
    }

    public override void Use()
    {
        CacheOriginalPlayerData();

        #region bonus method calls --------

        InstantStaminaRecovery();

        StaminaRecoveryChange();

        MoveSpeedIncrease();

        #endregion ------------------------

        Quantity--;
        if(Quantity <= 0)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            if (inRightHand)
            {
                playerHands.RightObject = null;
                playerHands.UsingRight = false;
                inRightHand = false;
            }
            else
            {
                playerHands.LeftObject = null;
                playerHands.UsingLeft = false;

            }


            if (consumableData.effectTime > 0)
            {
                // Will start consume process, includes restoration of player stats
                StartCoroutine(DestroyTimer());
            }
            else
            {
                // Will not restore stats, permanent effect.
                Destroy(gameObject);
            }
        }

    }

    public void CacheOriginalPlayerData()
    {
        cachedPlayerRecoverySpeed = playerData.staminaRecoverySpeed;
        cahcedPlayerSpeedMod = playerData.speedModifier;
    }

    /// <summary>
    /// Revert temporary changes made to player using cached values
    /// </summary>
    public virtual void RestorePlayer()
    {
        playerData.staminaRecoverySpeed = cachedPlayerRecoverySpeed;
        playerData.speedModifier = cahcedPlayerSpeedMod;
    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(consumableData.effectTime);
        RestorePlayer();
        Destroy(gameObject);
    }


    #region bonus methods ----------------------------------------

    public void InstantStaminaRecovery()
    {
        if (playerController.currentStamina + consumableData.instantStaminaRecovery <= playerData.maxStamina)
            playerController.currentStamina += consumableData.instantStaminaRecovery;
        else
            playerController.currentStamina = playerData.maxStamina;
    }

    public void StaminaRecoveryChange()
    {
        playerData.staminaRecoverySpeed = consumableData.staminaRecoverySpeed;
    }

    public void MoveSpeedIncrease()
    {
        playerData.speedModifier = consumableData.moveSpeedMod;
    }

    #endregion -------------------------------------------------------
}
