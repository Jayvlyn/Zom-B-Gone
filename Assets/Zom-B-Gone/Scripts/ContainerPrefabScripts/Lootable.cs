using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour, IInteractable
{
    public CollectibleSlot[] collectibleSlots = new CollectibleSlot[5];
    [SerializeField] private VoidEvent lootableOpened;

    public CollectibleContainerData containerData;

    private void Awake()
    {

        // do random filling of contents based on loot table
    }

    public void Interact(bool rightHand)
    {
        containerData.Container.collectibleSlots = collectibleSlots;
        containerData.onContainerCollectibleUpdated.Raise();
        lootableOpened.Raise();
        //gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void OnLootableClosed()
    {
        //gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public void Interact(Head head)
    {
        throw new System.NotImplementedException();
    }
}
