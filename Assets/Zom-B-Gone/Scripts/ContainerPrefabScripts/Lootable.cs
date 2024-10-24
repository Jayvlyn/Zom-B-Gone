using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour, IInteractable
{
    public CollectibleSlot[] collectibleSlots = new CollectibleSlot[5]; // unique to each instance of lootable
    [SerializeField] private VoidEvent lootableOpened;

    public CollectibleContainerData containerData; // container data tied to ui for all lootables

    private void Awake()
    {
        float chanceToFill = 100;
        for(int i = 0; i < collectibleSlots.Length; i++)
        {
            if (Random.Range(0f, 100f) <= chanceToFill)
            {
                chanceToFill *= 0.5f;

                CollectibleData coll = GameManager.currentZoneLootTable.GetRandomCollectible();
                if (coll == null) return;

                collectibleSlots[i].CollectibleName = coll.name;
                collectibleSlots[i].quantity = GameManager.currentZoneLootTable.GetRandomQuantity(coll);
            }
            else break;
        }
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
