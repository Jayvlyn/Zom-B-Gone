using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour
{
    public CollectibleSlot[] collectibleSlots = new CollectibleSlot[5]; // unique to each instance of lootable
    [SerializeField] private VoidEvent lootableOpened;

    public CollectibleContainerData containerData; // container data tied to ui for all lootables

    public CollectibleContainerData backpackData;
    public CollectibleContainerData handsData;
    public CollectibleContainerData headData;


    private void Awake()
    {
        float chanceToFill = 100;
        for(int i = 0; i < collectibleSlots.Length; i++)
        {
            if (Random.Range(0f, 100f) <= chanceToFill)
            {
                chanceToFill *= 0.5f;

                CollectibleData coll = GameManager.currentZone.lootTable.GetRandomCollectible();
                if (coll == null) return;

                collectibleSlots[i].CollectibleName = coll.name;
                collectibleSlots[i].quantity = GameManager.currentZone.lootTable.GetRandomQuantity(coll);
            }
            else break;
        }
    }

    public void OpenLootable()
    {
        containerData.Container.collectibleSlots = collectibleSlots;
        containerData.onContainerCollectibleUpdated.Raise();
        lootableOpened.Raise();
    }

    public void LootAll()
    {
        for(int i = 0; i < collectibleSlots.Length; i++)
        {
            if (collectibleSlots[i].Collectible is LootData)
            {
                backpackData.AddToContainer(ref collectibleSlots[i]);
                
            }
            else if (collectibleSlots[i].Collectible is ItemData)
            {
                handsData.AddToContainer(ref collectibleSlots[i]);
                handsData.onContainerSwapped.Raise();
            }
            else if (collectibleSlots[i].Collectible is HatData)
            {
                if (headData.container.collectibleSlots[0].Collectible != null)
                {
                    backpackData.AddToContainer(ref collectibleSlots[i]);
                }
                else
                {
                    headData.AddToContainer(ref collectibleSlots[i]);
                }
            }
        }
        containerData.onContainerCollectibleUpdated.Raise();
    }
}
