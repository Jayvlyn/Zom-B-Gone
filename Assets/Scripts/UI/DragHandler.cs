using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected SlotUI slotUI = null;
    [SerializeField] protected CollectibleEvent onMouseStartHoverCollectible = null;
    [SerializeField] protected VoidEvent onMouseEndHoverCollectible = null;

    private CanvasGroup canvasGroup = null;
    private Transform originalParent = null;
    private bool isHovering = false;

    public SlotUI GetSlotUI => slotUI;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnDisable()
    {
        if(isHovering)
        {
            onMouseEndHoverCollectible.Raise();
            isHovering = false;
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            onMouseEndHoverCollectible.Raise();

            originalParent = transform.parent;

            transform.SetParent(transform.parent.parent);

            canvasGroup.blocksRaycasts = false;
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            transform.position = Input.mousePosition;
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onMouseStartHoverCollectible.Raise(GetSlotUI.SlotCollectible);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onMouseEndHoverCollectible.Raise();
        isHovering = false;
    }
}
