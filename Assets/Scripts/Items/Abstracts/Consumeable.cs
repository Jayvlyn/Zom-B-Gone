using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class Consumeable : Item
{
    [SerializeField, Tooltip("Amount of time the consumable effect lasts (0/negative for perminance)")] 
    float _effectTime;

    public override void Use()
    {
        // call base.Use() in consumables after their functionality
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        if (inRightHand)
        {
            _playerHands._rightObject = null; 
            _playerHands._usingRight = false;
            inRightHand = false;
        }
        else
        {
            _playerHands._leftObject = null; 
            _playerHands._usingLeft = false;

        }

        if (_effectTime > 0) StartCoroutine(DestroyTimer());
        else Destroy(this.gameObject);
    }

    /// <summary>
    /// Revert temporary changes made to player
    /// </summary>
    public abstract void RestorePlayer();

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(_effectTime);
        RestorePlayer();
        Destroy(this.gameObject);
    }
}
