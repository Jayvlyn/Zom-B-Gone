using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChanger : MonoBehaviour
{
	public CollectibleContainerSlot handSlot;
	public bool rightHand = false;
	private Hands playerHands;

	private void Awake()
	{
		playerHands = FindObjectOfType<Hands>();
	}

	public void CheckItemChange()
	{
		if (rightHand) DoChecks(playerHands.rightItem);
		else		   DoChecks(playerHands.leftItem);
		
	}

	private void DoChecks(Item heldItem)
	{
		if (handSlot.SlotCollectible == null && heldItem != null) // remove item in hand from world (data not lost, exists in inventory)
		{
			RemoveItemFromHand(heldItem);
		}

		else if (handSlot.SlotCollectible != null && heldItem == null) // add item in hand
		{ 
			SpawnNewItemInHand();
		}

		else if (handSlot.SlotCollectible != null && heldItem != null) // swap item in hand
		{
			if (heldItem.itemData.name != handSlot.SlotCollectible.name)
			{
				RemoveItemFromHand(heldItem);
				SpawnNewItemInHand();
			}
		}
	}


	private void SpawnNewItemInHand()
	{
		string hatName = handSlot.SlotCollectible.name;
		GameObject prefab = Resources.Load<GameObject>(hatName);
		GameObject itemObject = Instantiate(prefab, playerHands.transform.position, playerHands.transform.rotation);
		Item heldItem = itemObject.GetComponent<Item>();
		heldItem.Interact(rightHand);
		if (rightHand)
		{
			playerHands.RightObject = itemObject; 
			playerHands.UsingRight = true;
		}
		else
		{
			playerHands.LeftObject = itemObject; 
			playerHands.UsingLeft = true;
		}
	}

	private void RemoveItemFromHand(Item heldItem)
	{
		Destroy(heldItem.gameObject);
		if (rightHand) playerHands.UsingRight = false;
		else playerHands.UsingLeft = false;
	}
}
