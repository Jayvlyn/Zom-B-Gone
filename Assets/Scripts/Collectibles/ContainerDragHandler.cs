using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerDragHandler : DragHandler
{
    public override void OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnPointerUp(eventData);

            if(eventData.hovered.Count == 0)
            {
                // if in base, send back to original spot, otherwise when in game: 
                // drop item on ground
            }
        }
    }
}
