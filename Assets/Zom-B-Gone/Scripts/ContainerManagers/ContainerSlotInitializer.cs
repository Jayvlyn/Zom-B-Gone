using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should be placed on the slot holder that has all slots as children
public class ContainerSlotInitializer : MonoBehaviour
{
    [SerializeField] public CollectibleContainerData containerData = null;
    [SerializeField] private string slotPrefabName;

    void Start()
    {
        // Child count is how many slot prefabs exist right now
        if(transform.childCount < containerData.size)
        {
            // Makes an empty slot with the same allows
            // makes template with last slot, as containers like backpack have a first slot that is different from the rest
            CollectibleSlot slotTemplate = containerData.Container.collectibleSlots[containerData.size-1]; 
            slotTemplate.collectible = null;
            slotTemplate.quantity = 0;

            Object newSlot = Resources.Load(slotPrefabName);

            int missingSlotCount = containerData.size - transform.childCount;
            for (int i = 0; i < missingSlotCount; i++)
            {
                // New slot with this as the parent
                Instantiate(newSlot, transform);
            }

            containerData.onContainerCollectibleUpdated.Raise();
        }
    }

}
