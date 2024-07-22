using System.Collections;
using UnityEngine;

public abstract class Consumeable : Item
{
    public ConsumableData consumableData;

    public override void Use()
    {
        // call base.Use() in consumables after their functionality
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

        if (consumableData.effectTime > 0) StartCoroutine(DestroyTimer());
        else Destroy(this.gameObject);
    }

    /// <summary>
    /// Revert temporary changes made to player
    /// </summary>
    public abstract void RestorePlayer();

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(consumableData.effectTime);
        RestorePlayer();
        Destroy(this.gameObject);
    }
}
