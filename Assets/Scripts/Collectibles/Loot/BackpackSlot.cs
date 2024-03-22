using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackpackSlot : SlotUI, IDropHandler
{
    [SerializeField] private Backpack backpack = null;
    [SerializeField] private TextMeshProUGUI lootQuantityText = null;

    public override Collectible SlotCollectible
    {
        get { return LootSlot.loot; }
        set { }
    }

    public LootSlot LootSlot => backpack.Container.GetSlotByIndex(SlotIndex);

    public override void OnDrop(PointerEventData eventData)
    {
        DragHandler dragHandler = eventData.pointerDrag.GetComponent<DragHandler>();

        if (dragHandler == null) return;

        if((dragHandler.GetSlotUI as BackpackSlot) != null)
        {
            backpack.Container.Swap(dragHandler.GetSlotUI.SlotIndex, SlotIndex);
        }
    }

    public override void UpdateSlotUI()
    {
        if(LootSlot.loot == null)
        {
            EnableSlotUI(false);
            return;
        }
        EnableSlotUI(true);

        icomImage.sprite = LootSlot.loot.Icon;
        lootQuantityText.text = LootSlot.quantity > 1 ? LootSlot.quantity.ToString() : "";
    }

    protected override void EnableSlotUI(bool enable)
    {
        base.EnableSlotUI(enable);
        lootQuantityText.enabled = enable;
    }

}