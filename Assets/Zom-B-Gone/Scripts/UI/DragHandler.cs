using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected SlotUI slotUI = null;
    [SerializeField] protected CollectibleEvent onMouseStartHoverCollectible = null;
    [SerializeField] protected VoidEvent onMouseEndHoverCollectible = null;

    private CanvasGroup canvasGroup = null;
    private Transform originalParent = null;
    private bool isHovering = false;
    private bool lerpToMouse = false;
    private float lerpSpeed = 12f;

    public SlotUI GetSlotUI => slotUI;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

	private void Update()
	{
		
	}

    private IEnumerator LerpToMouse()
    {
        lerpToMouse = true;
        while(lerpToMouse)
        {
            transform.position = Vector3.Lerp(transform.position, Input.mousePosition, lerpSpeed * Time.deltaTime);
            yield return null;
        }
        lerpToMouse = false;
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


            //if (PlayerController.holdingSneak)
            if(false)
            { // auto swap to other open container.

                // find what container this is, and what would make the most sense to move into;
                CollectibleContainerSlot thisSlot = transform.parent.GetComponent<CollectibleContainerSlot>();
                CollectibleContainerData thisContainer = thisSlot.containerData;
                switch (thisContainer.containerType)
                {
                    case ContainerType.HANDS:
                        break;
                    case ContainerType.LOOTABLE:
                        break;
                    case ContainerType.HEAD:
                        break;
                    case ContainerType.LOCKER:
                        break;
                    case ContainerType.BACKPACK:
                        break;
                    case ContainerType.WORKBENCH:
                        break;
                }

                CollectibleData collectible = thisSlot.CollectibleSlot.collectible;
                if (collectible as ItemData)
                {

                }
                else if (collectible as HatData)
                {

                }
                else if (collectible as LootData)
                {

                }
            }
            else // click and start dragging
            {
                originalParent = transform.parent;

                Transform newParent = transform.parent.parent;
                for(int i = 0; i < 20; i++)
                {
                    //if(!newParent.gameObject.TryGetComponent(out CanvasScaler cs))
                    if(!newParent.CompareTag("HUD"))
                    {
                        newParent = newParent.parent;
                    }
                    else
                    {
                        break;
                    }
                }

                transform.SetParent(newParent);

                canvasGroup.blocksRaycasts = false;

                StartCoroutine(LerpToMouse());
            }

		}
	}

	public virtual void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (lerpToMouse)
            {
                lerpToMouse = false;
                StopCoroutine(LerpToMouse());
            }

            transform.position = Input.mousePosition;
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
			if (lerpToMouse)
			{
				lerpToMouse = false;
				StopCoroutine(LerpToMouse());
			}

			sendBackToSlot();
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

    public void sendBackToSlot()
    {
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        canvasGroup.blocksRaycasts = true;
        onMouseEndHoverCollectible.Raise();
        isHovering = false;
    }
}
