using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerDragHandler : DragHandler
{
    [SerializeField] private CollectibleDropper collectibleDropper = null;

    public override void OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnPointerUp(eventData);
            if(eventData.hovered.Count == 0)
            {
                CollectibleContainerSlot thisSlot = GetSlotUI as CollectibleContainerSlot;
                collectibleDropper.Activate(thisSlot.SlotIndex);
            }
			#region old stuff
			//for (int i = 0; i < eventData.hovered.Count; i++)
			//{
			//    if (eventData.hovered[i].layer != LayerMask.NameToLayer("UI"))
			//    {
			//        CollectibleContainerSlot thisSlot = GetSlotUI as CollectibleContainerSlot;
			//        collectibleDestroyer.Activate(thisSlot.CollectibleSlot, thisSlot.SlotIndex);
			//    }
			//}


			//if(eventData.hovered.Count == 0)
			//{
			//    BackpackSlot thisSlot = GetSlotUI as BackpackSlot;
			//    lootDestroyer.Activate(thisSlot.LootSlot, thisSlot.SlotIndex);
			//}
			#endregion
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
        if (eventData.button == PointerEventData.InputButton.Left)
        {
			base.OnPointerDown(eventData);
			PlayerController.mouseHeldIcon = this;
        }
    }
}
