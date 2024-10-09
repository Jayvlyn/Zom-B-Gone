using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverableCollectible : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private CollectibleData collectibleData;
	public CollectibleData CollectibleData
	{
		get { return collectibleData; }
		set { collectibleData = value;
			collectibleImage.sprite = collectibleData.icon;
		}

	}
	[SerializeField] private Image collectibleImage;
	[SerializeField] protected CollectibleEvent onMouseStartHoverCollectible = null;
	[SerializeField] protected VoidEvent onMouseEndHoverCollectible = null;


	private bool isHovering = false;
	private void OnDisable()
	{
		if (isHovering)
		{
			onMouseEndHoverCollectible.Raise();
			isHovering = false;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		onMouseStartHoverCollectible.Raise(collectibleData);
		isHovering = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		onMouseEndHoverCollectible.Raise();
		isHovering = false;
	}
}
