using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private Vector3 initialMousePosition;
    private Vector3 initialElementPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialMousePosition = Input.mousePosition;
        initialElementPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector3 offset = Input.mousePosition - initialMousePosition;
            transform.position = initialElementPosition + offset;
        }
    }

}
