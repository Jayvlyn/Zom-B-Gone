using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectibleContainerSlot : SlotUI, IDropHandler
{
    [SerializeField] private bool allowLoot = false;
    [SerializeField] private bool allowItems = false;
    [SerializeField] private bool allowHats = false;
    [SerializeField] private CollectibleContainerData containerData = null;
    [SerializeField] private TextMeshProUGUI collectibleQuantityText = null;

    public override CollectibleData SlotCollectible
    {
        get { return CollectibleSlot.collectible; }
        set { }
    }

    public CollectibleSlot CollectibleSlot => containerData.Container.GetSlotByIndex(SlotIndex);

    public override void OnDrop(PointerEventData eventData) // dropping item into slot
    {
        // drag handler of the slot you were hovering over when you let go of left click
        DragHandler dragHandler = eventData.pointerDrag.GetComponent<DragHandler>();

        if (dragHandler == null) return;

		if ((dragHandler.GetSlotUI as CollectibleContainerSlot) != null) 
        {
            CollectibleContainerSlot otherSlot = dragHandler.GetSlotUI as CollectibleContainerSlot;

            CollectibleData otherCollectible = otherSlot.containerData.Container.collectibleSlots[otherSlot.SlotIndex].collectible;

            if (otherCollectible as LootData && !allowLoot) return; 
            if (otherCollectible as HatData  && !allowHats) return; 
            if (otherCollectible as ItemData && !allowItems) return;
            if (SlotCollectible as LootData && !otherSlot.allowLoot) return;
            if (SlotCollectible as HatData && !otherSlot.allowHats) return;
            if (SlotCollectible as ItemData && !otherSlot.allowItems) return;

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

					containerData.Container.collectibleSlots[otherSlotIndex] = new CollectibleSlot();

					return;
				}
			}
		}

        otherCollectibleSlot.containerData.Container.collectibleSlots[otherSlotIndex] = thisCollectibleSlot;
        containerData.Container.collectibleSlots[SlotIndex] = otherSlot;
	}

}