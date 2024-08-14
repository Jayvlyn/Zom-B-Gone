using System;

[Serializable]
public class CollectibleContainer : ICollectibleContainer
{
    public CollectibleSlot[] collectibleSlots = new CollectibleSlot[0];

    public Action OnCollectibleUpdated = delegate { };
    public Action OnCollectibleSwapped = delegate { };

    public CollectibleContainer(int size)
    {
        collectibleSlots = new CollectibleSlot[size];
    }

    // Gets slot index based on its index under the parent, so if it is the third child to an object it will be an index of 2
    public CollectibleSlot GetSlotByIndex(int index)
    {
        return collectibleSlots[index];
    }

    public CollectibleSlot AddCollectible(CollectibleSlot collectibleSlot)
    {
        for (int i = 0; i < collectibleSlots.Length; i++)
        {
            if (collectibleSlots[i] != null)
            {
                if (collectibleSlots[i].collectible == collectibleSlot.collectible)
                {
					int remainingSpace = collectibleSlots[i].GetRemainingSpace();
                    if (collectibleSlot.quantity <= remainingSpace)
                    {
                        collectibleSlots[i].quantity += collectibleSlot.quantity;
                        collectibleSlot.quantity = 0;
                        OnCollectibleUpdated.Invoke();
                        return collectibleSlot;
                    }
                    else if(remainingSpace > 0)
                    {
                        collectibleSlots[i].quantity += remainingSpace;

                        collectibleSlot.quantity -= remainingSpace;
                    }
                }
            }
        }

        for (int i = 0; i < collectibleSlots.Length; i++)
        {
            if (collectibleSlots[i].collectible == null)
            {
                if(collectibleSlot.quantity <= collectibleSlot.collectible.MaxStack)
                {
					if (collectibleSlot.collectible as LootData && !collectibleSlots[i].allowLoot) continue;
					if (collectibleSlot.collectible as HatData && !collectibleSlots[i].allowHats) continue;
					if (collectibleSlot.collectible as ItemData && !collectibleSlots[i].allowItems) continue;

					collectibleSlots[i].collectible = collectibleSlot.collectible;
                    collectibleSlots[i].quantity = collectibleSlot.quantity;

                    collectibleSlot.quantity = 0;

                    OnCollectibleUpdated.Invoke();

                    return collectibleSlot;
                }
                else
                {
                    collectibleSlots[i] = new CollectibleSlot(collectibleSlot.collectible, collectibleSlot.collectible.MaxStack, collectibleSlots[i].allowLoot, collectibleSlots[i].allowItems, collectibleSlots[i].allowHats);

                    collectibleSlot.quantity -= collectibleSlot.collectible.MaxStack;
                }
            }
        }

        OnCollectibleUpdated.Invoke();

        return collectibleSlot;
    }

    public int GetTotalQuantity(CollectibleData collectible)
    {
        int totalCount = 0;

        foreach(var slot in collectibleSlots)
        {
            if(slot.collectible == null || slot.collectible != collectible) continue;

            totalCount += slot.quantity;
        }

        return totalCount;
    }

    public bool HasCollectible(CollectibleData collectible)
    {
        foreach(var slot in collectibleSlots)
        {
            if (slot.collectible == null || slot.collectible != collectible) continue;
            return true;
        }
        return false;
    }

    public void RemoveAt(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > collectibleSlots.Length - 1) return;

        collectibleSlots[slotIndex].collectible = null;
        collectibleSlots[slotIndex].quantity = 0;


        OnCollectibleUpdated.Invoke();
    }

    public void RemoveCollectible(CollectibleSlot collectibleSlot)
    {
        for (int i = 0; i < collectibleSlots.Length; i++)
        {
            if (collectibleSlots[i].collectible != null)
            {
                if (collectibleSlots[i].collectible == collectibleSlot.collectible)
                {
                    if (collectibleSlots[i].quantity < collectibleSlot.quantity)
                    {
                        collectibleSlot.quantity -= collectibleSlots[i].quantity;

						collectibleSlots[i].collectible = null;
						collectibleSlots[i].quantity = 0;
					}
                    else
                    {
                        collectibleSlots[i].quantity -= collectibleSlot.quantity;

                        if (collectibleSlots[i].quantity == 0)
                        {
							collectibleSlots[i].collectible = null;

							OnCollectibleUpdated.Invoke();

                            return;
                        }
                    }
                }
            }
        }
    }

    public void Swap(int indexOne, int indexTwo)
    {
        CollectibleSlot firstSlot = collectibleSlots[indexOne];
        CollectibleSlot secondSlot = collectibleSlots[indexTwo];

        //if (firstSlot == secondSlot) return;

        if(secondSlot.collectible != null)
        {
            if(firstSlot.collectible == secondSlot.collectible) // Check if same collectible, stack
			{
                if(firstSlot.quantity <= secondSlot.GetRemainingSpace()) // Enough space to stack
                {
                    secondSlot.quantity += firstSlot.quantity;

                    collectibleSlots[indexOne] = new CollectibleSlot();

                    OnCollectibleUpdated.Invoke();
                    OnCollectibleSwapped.Invoke();

                    return;
                }
            }
        }

        collectibleSlots[indexOne] = secondSlot;
        collectibleSlots[indexTwo] = firstSlot;

        OnCollectibleUpdated.Invoke();
        OnCollectibleSwapped.Invoke();
    }
}