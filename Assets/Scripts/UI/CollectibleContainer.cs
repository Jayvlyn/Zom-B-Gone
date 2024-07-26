using System;

[Serializable]
public class CollectibleContainer : ICollectibleContainer
{
    private CollectibleSlot[] collectibleSlots = new CollectibleSlot[0];

    public Action OnCollectibleUpdated = delegate { };

    public CollectibleContainer(int size) => collectibleSlots = new CollectibleSlot[size];

    public CollectibleSlot GetSlotByIndex(int index) => collectibleSlots[index];

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
                    collectibleSlots[i] = collectibleSlot;

                    collectibleSlot.quantity = 0;

                    OnCollectibleUpdated.Invoke();

                    return collectibleSlot;
                }
                else
                {
                    collectibleSlots[i] = new CollectibleSlot(collectibleSlot.collectible, collectibleSlot.collectible.MaxStack);

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

        collectibleSlots[slotIndex] = new CollectibleSlot();

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

                        collectibleSlots[i] = new CollectibleSlot();
                    }
                    else
                    {
                        collectibleSlots[i].quantity -= collectibleSlot.quantity;

                        if (collectibleSlots[i].quantity == 0)
                        {
                            collectibleSlots[i] = new CollectibleSlot();

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

        if (firstSlot == secondSlot) return;

        if(secondSlot.collectible != null) // Check if same item, stack
        {
            if(firstSlot.collectible == secondSlot.collectible)
            {
                if(firstSlot.quantity <= secondSlot.GetRemainingSpace())
                {
                    secondSlot.quantity += firstSlot.quantity;

                    collectibleSlots[indexOne] = new CollectibleSlot();

                    OnCollectibleUpdated.Invoke();

                    return;
                }
            }
        }

        collectibleSlots[indexOne] = secondSlot;
        collectibleSlots[indexTwo] = firstSlot;

        OnCollectibleUpdated.Invoke();
    }
}