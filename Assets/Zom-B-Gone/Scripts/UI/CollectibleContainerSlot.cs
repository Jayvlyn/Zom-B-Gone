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

    [Tooltip("Optional Event")] public VoidEvent OnContainerSwapped;

    public override CollectibleData SlotCollectible
    {
        get { return CollectibleSlot.collectible; }
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

		if ((dragHandler.GetSlotUI as CollectibleContainerSlot) != null) 
        {
            CollectibleContainerSlot otherSlot = dragHandler.GetSlotUI as CollectibleContainerSlot;

            CollectibleData otherCollectible = otherSlot.containerData.Container.collectibleSlots[otherSlot.SlotIndex].collectible;

            if (otherCollectible as LootData && !CollectibleSlot.allowLoot) return; 
            if (otherCollectible as HatData  && !CollectibleSlot.allowHats) return; 
            if (otherCollectible as ItemData && !CollectibleSlot.allowItems) return;
            if (SlotCollectible as LootData && !otherSlot.CollectibleSlot.allowLoot) return;
            if (SlotCollectible as HatData && !otherSlot.CollectibleSlot.allowHats) return;
            if (SlotCollectible as ItemData && !otherSlot.CollectibleSlot.allowItems) return;

            if(otherSlot.containerData == containerData)
            {
                containerData.Container.Swap(otherSlot.SlotIndex, SlotIndex);
            }
            else // Slots have different containers, try to transfer containers
            {
                // Transfer between containers
                SwapBetweenContainers(otherSlot, otherSlot.SlotIndex);

                containerData.Container.OnCollectibleUpdated.Invoke();
                containerData.Container.OnCollectibleSwapped.Invoke();
				otherSlot.containerData.Container.OnCollectibleUpdated.Invoke();
                otherSlot.containerData.Container.OnCollectibleSwapped.Invoke();
            }
        }
    }

    public override void UpdateSlotUI()
    {
        if(CollectibleSlot.collectible == null)
        {
            EnableSlotUI(false);
            return;
        }
        EnableSlotUI(true);

        icomImage.sprite = CollectibleSlot.collectible.Icon;
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

		if (SlotCollectible != null)
		{
			if (otherSlot.collectible == SlotCollectible) // Check if same collectible, stack
			{
				if (otherSlot.quantity <= thisCollectibleSlot.GetRemainingSpace()) // Enough space to stack
				{
					thisCollectibleSlot.quantity += otherSlot.quantity;

					containerData.Container.collectibleSlots[otherSlotIndex] = new CollectibleSlot(null, 0, containerData.Container.collectibleSlots[otherSlotIndex].allowLoot, containerData.Container.collectibleSlots[otherSlotIndex].allowItems, containerData.Container.collectibleSlots[otherSlotIndex].allowHats);

					return;
				}
			}
		}

        otherCollectibleSlot.containerData.Container.collectibleSlots[otherSlotIndex] = thisCollectibleSlot;
        containerData.Container.collectibleSlots[SlotIndex] = otherSlot;

        if(OnContainerSwapped != null)
        {
            OnContainerSwapped.Raise();
        }
        if(otherCollectibleSlot.OnContainerSwapped != null)
        {
            otherCollectibleSlot.OnContainerSwapped.Raise();

		}

	}

}