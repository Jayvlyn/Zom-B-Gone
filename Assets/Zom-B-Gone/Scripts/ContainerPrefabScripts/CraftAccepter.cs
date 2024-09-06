using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftAccepter : MonoBehaviour, IPointerDownHandler
{
    public VoidEvent onCraftAccepted;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onCraftAccepted.Raise();
        }
    }
}
