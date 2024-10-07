using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectibleContainerSlot : SlotUI, IDropHandler
{
    [SerializeField] public CollectibleContainerData containerData = null;
    [SerializeField] private TextMeshProUGUI collectibleQuantityText = null;

    public VoidEvent containerSwappedEvent = null;

    public override CollectibleData SlotCollectible
    {
        get { return CollectibleSlot.Collectible; }
        set { }
    }

    public CollectibleSlot CollectibleSlot
    {
        get { return containerData.Container.GetSlotByIndex(SlotIndex); }
    }

    public override void OnDrop(PointerEventData eventData) // dropping collectible into slot
    {
        // drag handler of the slot you were hovering over when you let go of left click
        DragHandler dragHandler = eventData.pointerDrag.GetComponent<DragHandler>();

        if (dragHandler == null) return;

		if (dragHandler.GetSlotUI as CollectibleContainerSlot) 
        {
            CollectibleContainerSlot otherSlot = dragHandler.GetSlotUI as CollectibleContainerSlot;

            CollectibleData otherCollectible = otherSlot.containerData.Container.collectibleSlots[otherSlot.SlotIndex].Collectible;

            //if (SlotCollectible as LootData && !otherSlot.CollectibleSlot.allowLoot) return;
            //if (SlotCollectible as HatData && !otherSlot.CollectibleSlot.allowHats) return;
            //if (SlotCollectible as ItemData && !otherSlot.CollectibleSlot.allowItems) return;
            if (otherSlot.CollectibleSlot.Collectible as LootData && !CollectibleSlot.allowLoot) return;
            if (otherSlot.CollectibleSlot.Collectible as HatData && !CollectibleSlot.allowHats) return;
            if (otherSlot.CollectibleSlot.Collectible as ItemData && !CollectibleSlot.allowItems) return;

            if (otherSlot.containerData == containerData)
            {
                containerData.Container.Swap(otherSlot.SlotIndex, SlotIndex);
            }
            else // Slots have different containers, try to transfer containers
            {
                // Transfer between containers
                SwapBetweenContainers(otherSlot, otherSlot.SlotIndex);

                containerData.Container.OnCollectibleUpdated.Invoke();
				otherSlot.containerData.Container.OnCollectibleUpdated.Invoke();
            }
        }
    }

    public override void UpdateSlotUI()
    {
        if(CollectibleSlot.Collectible == null) // causes out of bounds error on storage shrink
        {
            EnableSlotUI(false);
            return;
        }
        EnableSlotUI(true);

        icomImage.sprite = CollectibleSlot.Collectible.Icon;
        collectibleQuantityText.text = CollectibleSlot.quantity > 1 ? CollectibleSlot.quantity.ToString() : "";
    }

    protected override void EnableSlotUI(bool enable)
    {
        base.EnableSlotUI(enable);
        collectibleQuantityText.enabled = enable;
    }

    protected void SwapBetweenContainers(CollectibleContainerSlot otherCollectibleSlot, int otherSlotIndex)
    {
        CollectibleSlot thisCollectibleSlot = containerData.Container.GetSlotByIndex(SlotIndex); // Required for mutability, cant use "CollectibleSlot"
        CollectibleSlot otherSlot = otherCollectibleSlot.containerData.Container.GetSlotByIndex(otherSlotIndex);

		if (otherSlot.Collectible == SlotCollectible) // Check if same collectible, stack
		{
			if (otherSlot.quantity <= thisCollectibleSlot.GetRemainingSpace()) // Enough space to stack
			{
				thisCollectibleSlot.quantity += otherSlot.quantity;
                containerData.Container.collectibleSlots[SlotIndex].quantity = thisCollectibleSlot.quantity;

                //otherCollectibleSlot.containerData.Container.collectibleSlots[otherSlotIndex].Collectible = null;
                otherCollectibleSlot.containerData.Container.collectibleSlots[otherSlotIndex].CollectibleName = null;
                otherCollectibleSlot.containerData.Container.collectibleSlots[otherSlotIndex].quantity = 0;

			}
            else // not enough space to stack, but should move as much as possible
            {
                int amountFilled = thisCollectibleSlot.Collectible.MaxStack - thisCollectibleSlot.quantity;
                containerData.Container.collectibleSlots[SlotIndex].quantity = thisCollectibleSlot.Collectible.MaxStack;

                otherCollectibleSlot.containerData.Container.collectibleSlots[otherSlotIndex].quantity -= amountFilled;
            }
		}
		
        else
        {
            //if (thisCollectibleSlot.collectible as LootData && !otherSlot.allowLoot) return;
            //if (thisCollectibleSlot.collectible as HatData && !otherSlot.allowHats) return;
            //if (thisCollectibleSlot.collectible as ItemData && !otherSlot.allowItems) return;

            if (otherSlot.Collectible as LootData && !thisCollectibleSlot.allowLoot) return;
            if (otherSlot.Collectible as HatData && !thisCollectibleSlot.allowHats) return;
            if (otherSlot.Collectible as ItemData && !thisCollectibleSlot.allowItems) return;


            //otherCollectibleSlot.containerData.Container.collectibleSlots[otherSlotIndex].Collectible = thisCollectibleSlot.Collectible;
            otherCollectibleSlot.containerData.Container.collectibleSlots[otherSlotIndex].CollectibleName = thisCollectibleSlot.CollectibleName;
            otherCollectibleSlot.containerData.Container.collectibleSlots[otherSlotIndex].quantity = thisCollectibleSlot.quantity;

            //containerData.Container.collectibleSlots[SlotIndex].Collectible = otherSlot.Collectible;
            containerData.Container.collectibleSlots[SlotIndex].CollectibleName = otherSlot.CollectibleName;
            containerData.Container.collectibleSlots[SlotIndex].quantity = otherSlot.quantity;

            if(containerSwappedEvent) containerSwappedEvent.Raise();
            if(otherCollectibleSlot.containerSwappedEvent) otherCollectibleSlot.containerSwappedEvent.Raise();
        }

        containerData.onContainerCollectibleSwapped.Raise();
        otherCollectibleSlot.containerData.onContainerCollectibleSwapped.Raise();

	}

}