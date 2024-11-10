using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChanger : MonoBehaviour
{
	public CollectibleContainerSlot handSlot;
	public bool rightHand = false;
	private PlayerController playerController;

	private void Awake()
	{
		playerController = FindFirstObjectByType<PlayerController>();
	}

	public void CheckItemChange()
	{
		if (rightHand) DoChecks(playerController.hands.rightItem);
		else		   DoChecks(playerController.hands.leftItem);
		
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
		string itemName = handSlot.SlotCollectible.name;
		GameObject prefab = Resources.Load<GameObject>(itemName);
		GameObject itemObject = Instantiate(prefab, playerController.hands.transform.position, playerController.hands.transform.rotation);
		Item heldItem = itemObject.GetComponent<Item>();
		heldItem.Quantity = handSlot.CollectibleSlot.quantity;
		heldItem.Interact(rightHand, playerController);
		if (rightHand)
		{
			playerController.hands.RightObject = itemObject;
			playerController.hands.UsingRight = true;
		}
		else
		{
			playerController.hands.LeftObject = itemObject;
			playerController.hands.UsingLeft = true;
		}
	}

	private void RemoveItemFromHand(Item heldItem)
	{
		Destroy(heldItem.gameObject);
		if (rightHand) playerController.hands.UsingRight = false;
		else playerController.hands.UsingLeft = false;
	}
}
