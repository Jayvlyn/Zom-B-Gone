using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectibleContainerSlot : SlotUI, IDropHandler
{
    [SerializeField] private CollectibleContainerData containerData = null;
    [SerializeField] private TextMeshProUGUI collectibleQuantityText = null;

    public override CollectibleData SlotCollectible
    {
        get { return CollectibleSlot.collectible; }
        set { }
    }

    public CollectibleSlot CollectibleSlot => containerData.Container.GetSlotByIndex(SlotIndex);

    public override void OnDrop(PointerEventData eventData)
    {
        DragHandler dragHandler = eventData.pointerDrag.GetComponent<DragHandler>();

        if (dragHandler == null) return;

        if((dragHandler.GetSlotUI as CollectibleContainerSlot) != null)
        {
            containerData.Container.Swap(dragHandler.GetSlotUI.SlotIndex, SlotIndex);
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

}