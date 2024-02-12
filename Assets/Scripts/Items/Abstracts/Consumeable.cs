using System.Collections;
using UnityEngine;

public abstract class Consumeable : Item
{
    [SerializeField, Tooltip("Amount of time the consumable effect lasts (0/negative for perminance)")] 
    float _effectTime;

    public override void Use()
    {
        // call base.Use() in consumables after their functionality
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        if (_inRightHand)
        {
            _playerHands.RightObject = null; 
            _playerHands.UsingRight = false;
            _inRightHand = false;
        }
        else
        {
            _playerHands.LeftObject = null; 
            _playerHands.UsingLeft = false;

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
