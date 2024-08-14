using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should be placed on the slot holder that has all slots as children
public class ContainerResizer : MonoBehaviour
{
	[SerializeField] public CollectibleContainerData containerData = null;
    [SerializeField] public int rowSize = 6;
	[SerializeField] private string slotPrefabName;

	public void CheckResize()
	{
		int lastSlotIndex = containerData.Container.collectibleSlots.Length - 1;
		int fullSlotsInLastRow = 0;
		for (int i = lastSlotIndex; i > (lastSlotIndex-rowSize); i--)
		{
			if (containerData.Container.collectibleSlots[i].collectible != null)
			{
				fullSlotsInLastRow++;
			}
		}
		if (fullSlotsInLastRow >= rowSize)
		{
			IncreaseRow();
		}
		else if (fullSlotsInLastRow == 0 && (lastSlotIndex+1) >= rowSize)
		{ // Reduce row if there is more than one row and last row is empty
			ReduceRow();
		}
	}

	private void ReduceRow()
	{
		//Debug.Log("Reduce");

		CollectibleSlot[] cachedSlots = containerData.Container.collectibleSlots;

		containerData.size -= rowSize;
		containerData.Container = new CollectibleContainer(containerData.size);

		// loop and destroy last row of slots in children
		int lastSlotIndex = containerData.Container.collectibleSlots.Length - 1 - rowSize;
		for (int i = lastSlotIndex; i > (lastSlotIndex - rowSize); i--)
		{
			Destroy(transform.GetChild(i).gameObject);
			//Debug.Log("Destroy Slot at index: " + i);
		}



	}

	private void IncreaseRow()
	{
		//Debug.Log("Increase");

		CollectibleSlot[] cachedSlots = containerData.Container.collectibleSlots;

		containerData.size += rowSize;
		containerData.Container = new CollectibleContainer(containerData.size);

		// loop and instantiate more slots, making them children of this
		Object newSlot = Resources.Load(slotPrefabName);
		int lastSlotIndex = containerData.Container.collectibleSlots.Length - 1 + rowSize;
		for (int i = lastSlotIndex; i > (lastSlotIndex - rowSize); i--)
		{
			Instantiate(newSlot, transform);
			//Debug.Log("new slot");
		}



	}
}
