using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour, IInteractable
{
    CollectibleData[] contents = new CollectibleData[5];

    private void Awake()
    {
        // do random filling of contents based on loot table
    }

    public void Interact(bool rightHand)
    {
        throw new System.NotImplementedException();
    }

    public void Interact(Head head)
    {
        throw new System.NotImplementedException();
    }
}
