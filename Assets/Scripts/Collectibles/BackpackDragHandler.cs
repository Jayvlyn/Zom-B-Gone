using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackpackDragHandler : DragHandler
{
    [SerializeField] private LootDestroyer lootDestroyer = null;

    public override void OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnPointerUp(eventData);

            for (int i = 0; i < eventData.hovered.Count; i++)
            {
                if (eventData.hovered[i].layer != LayerMask.NameToLayer("UI"))
                {
                    BackpackSlot thisSlot = GetSlotUI as BackpackSlot;
                    lootDestroyer.Activate(thisSlot.LootSlot, thisSlot.SlotIndex);
                }
            }

            //if(eventData.hovered.Count == 0)
            //{
            //    BackpackSlot thisSlot = GetSlotUI as BackpackSlot;
            //    lootDestroyer.Activate(thisSlot.LootSlot, thisSlot.SlotIndex);
            //}
        }
    }
}
