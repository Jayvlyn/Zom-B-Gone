using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should be placed on the slot holder that has all slots as children
public class ContainerResizer : MonoBehaviour
{
	[SerializeField] public CollectibleContainerData containerData = null;
	[SerializeField] private string slotPrefabName;


    private ContainerSlotInitializer slotInitializer;
	private RectTransform backgroundRect;
    private int rowSize;

	private void Awake()
	{
		slotInitializer = GetComponent<ContainerSlotInitializer>();
	}

	private void Start()
	{
		backgroundRect = slotInitializer.backgroundRect;
		rowSize = slotInitializer.rowSize;
	}

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
		else if (fullSlotsInLastRow == 0)
		{
			// tiny tiny bug with this method, a specific case where it will leave two empty rows instead of one, not that serious

			// start at the last slot excluding the very last row
			int secondLastRowEndSlotIndex = lastSlotIndex - rowSize;
			int emptySlots = 0;

			for (int i = secondLastRowEndSlotIndex; i > 0; i--)
			{
                if (containerData.Container.collectibleSlots[i].collectible == null)
                {
                    emptySlots++;
                }
				else
				{
                    // Once you find a non empty slot, stop checking for more empty slots
                    break;
				}
            }

			if(emptySlots > rowSize) // at least one empty row to remove
			{
				RemoveRows(emptySlots / rowSize);
			}
		}
	}

    private void IncreaseRow()
    {
        CollectibleSlot[] cachedSlots = containerData.Container.collectibleSlots;

        containerData.size += rowSize;
        containerData.Container = new CollectibleContainer(containerData.size);
        containerData.Container.OnCollectibleUpdated += containerData.onContainerCollectibleUpdated.Raise;
        containerData.Container.OnCollectibleSwapped += containerData.onContainerCollectibleSwapped.Raise;

        for (int i = 0; i < cachedSlots.Length; i++)
        {
            containerData.Container.collectibleSlots[i] = cachedSlots[i];
        }

        // Makes an empty slot with the same allows
        CollectibleSlot slotTemplate = cachedSlots[0];
        slotTemplate.collectible = null;
        slotTemplate.quantity = 0;

        for (int i = (cachedSlots.Length + rowSize - 1); i >= cachedSlots.Length; i--)
        {
            containerData.Container.collectibleSlots[i] = slotTemplate;
        }

        // loop and instantiate more slots, making them children of this
        Object newSlot = Resources.Load(slotPrefabName);
        int lastSlotIndex = containerData.Container.collectibleSlots.Length - 1 + rowSize;
        for (int i = lastSlotIndex; i > (lastSlotIndex - rowSize); i--)
        {
            // new slot with this as the parent
            Instantiate(newSlot, transform);
        }

        //backgroundRect.offsetMin = new Vector2(0, backgroundRect.offsetMin.y + slotInitializer.rowBackgroundIncrement);
        backgroundRect.sizeDelta = new Vector2(backgroundRect.sizeDelta.x, backgroundRect.sizeDelta.y + slotInitializer.rowBackgroundIncrement);
    }

    private void RemoveRows(int rowAmountToRemove)
    {
        CollectibleSlot[] cachedSlots = containerData.Container.collectibleSlots;

		int slotAmountToRemove = rowSize * rowAmountToRemove;

        containerData.size -= slotAmountToRemove;
        containerData.Container = new CollectibleContainer(containerData.size);
        containerData.Container.OnCollectibleUpdated += containerData.onContainerCollectibleUpdated.Raise;
        containerData.Container.OnCollectibleSwapped += containerData.onContainerCollectibleSwapped.Raise;

        for (int i = 0; i < cachedSlots.Length - slotAmountToRemove; i++)
        {
            containerData.Container.collectibleSlots[i] = cachedSlots[i];
        }


        int lastSlotIndex = containerData.Container.collectibleSlots.Length - 1 + slotAmountToRemove;
        for (int i = lastSlotIndex; i > (lastSlotIndex - slotAmountToRemove); i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        backgroundRect.sizeDelta = new Vector2(backgroundRect.sizeDelta.x, backgroundRect.sizeDelta.y - slotInitializer.rowBackgroundIncrement);
        //backgroundRect.offsetMin = new Vector2(0, backgroundRect.offsetMin.y - slotInitializer.rowBackgroundIncrement * rowAmountToRemove);
    }

    private void RemoveRow()
    {
        CollectibleSlot[] cachedSlots = containerData.Container.collectibleSlots;

        containerData.size -= rowSize;
        containerData.Container = new CollectibleContainer(containerData.size);
        containerData.Container.OnCollectibleUpdated += containerData.onContainerCollectibleUpdated.Raise;
        containerData.Container.OnCollectibleSwapped += containerData.onContainerCollectibleSwapped.Raise;

        for (int i = 0; i < cachedSlots.Length - rowSize; i++)
        {
            containerData.Container.collectibleSlots[i] = cachedSlots[i];
        }


        int lastSlotIndex = containerData.Container.collectibleSlots.Length - 1 + rowSize;
        for (int i = lastSlotIndex; i > (lastSlotIndex - rowSize); i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        backgroundRect.sizeDelta = new Vector2(backgroundRect.sizeDelta.x, backgroundRect.sizeDelta.y - slotInitializer.rowBackgroundIncrement);
        //backgroundRect.offsetMin = new Vector2(0, backgroundRect.offsetMin.y + slotInitializer.rowBackgroundIncrement);
    }
}
