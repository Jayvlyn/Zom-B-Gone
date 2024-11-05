using GameEvents;
using System;
using System.Diagnostics;

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
        if (collectibleSlots.Length > index) return collectibleSlots[index];
        else return new CollectibleSlot();
    }

    public CollectibleSlot AddCollectible(ref CollectibleSlot collectibleSlot)
    {
        for (int i = 0; i < collectibleSlots.Length; i++)
        {
            if (collectibleSlots[i] != null)
            {
                if (collectibleSlots[i].Collectible == collectibleSlot.Collectible)
                {
					int remainingSpace = collectibleSlots[i].GetRemainingSpace();
                    if (collectibleSlot.quantity <= remainingSpace)
                    {
                        collectibleSlots[i].quantity += collectibleSlot.quantity;
                        collectibleSlot.quantity = 0;
                        collectibleSlot.CollectibleName = null;
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
            if (collectibleSlots[i].Collectible == null)
            {
                if(collectibleSlot.quantity <= collectibleSlot.Collectible.MaxStack)
                {
					if (collectibleSlot.Collectible as LootData && !collectibleSlots[i].allowLoot) continue;
					if (collectibleSlot.Collectible as HatData && !collectibleSlots[i].allowHats) continue;
					if (collectibleSlot.Collectible as ItemData && !collectibleSlots[i].allowItems) continue;

					//collectibleSlots[i].Collectible = collectibleSlot.Collectible;
					collectibleSlots[i].CollectibleName = collectibleSlot.CollectibleName;
                    collectibleSlots[i].quantity = collectibleSlot.quantity;

                    collectibleSlot.quantity = 0;
                    collectibleSlot.CollectibleName = null;

                    OnCollectibleUpdated.Invoke();

                    return collectibleSlot;
                }
                else
                {
                    //collectibleSlots[i] = new CollectibleSlot(collectibleSlot.collectible, collectibleSlot.collectible.MaxStack, collectibleSlots[i].allowLoot, collectibleSlots[i].allowItems, collectibleSlots[i].allowHats);
                    //collectibleSlots[i].Collectible = collectibleSlot.Collectible;
                    collectibleSlots[i].CollectibleName = collectibleSlot.CollectibleName;
                    collectibleSlots[i].quantity = collectibleSlot.Collectible.MaxStack;

                    collectibleSlot.quantity -= collectibleSlot.Collectible.MaxStack;
                    if(collectibleSlot.quantity <= 0) collectibleSlot.CollectibleName = null;
                }
            }
        }

        OnCollectibleUpdated.Invoke();

        return collectibleSlot;
    }

    /// <summary>
    /// Adds collectible in the next empty slot, ignoring which type of collectible the slot allows and does not stack on previously existing collectibles
    /// </summary>
    /// <param name="collectibleSlot"></param>
    /// <returns>index of the slot the collectible(s) was stored in</returns>
    public int AddCollectibleNoStackIgnoreAllows(CollectibleSlot collectibleSlot)
    {
        for (int i = 0; i < collectibleSlots.Length; i++)
        {
            if (collectibleSlots[i].Collectible == null)
            {
                //collectibleSlots[i].Collectible = collectibleSlot.Collectible;
                collectibleSlots[i].CollectibleName = collectibleSlot.CollectibleName;
                collectibleSlots[i].quantity = collectibleSlot.quantity;

                collectibleSlot.quantity = 0;

                OnCollectibleUpdated.Invoke();
                return i;
            }
        }
        return -1;
    }

    public int GetTotalQuantity(CollectibleData collectible)
    {
        int totalCount = 0;

        foreach(var slot in collectibleSlots)
        {
            if(slot.Collectible == null || slot.Collectible != collectible) continue;

            totalCount += slot.quantity;
        }

        return totalCount;
    }

    public bool HasCollectible(CollectibleData collectible)
    {
        foreach(var slot in collectibleSlots)
        {
            if (slot.Collectible == null || slot.Collectible != collectible) continue;
            return true;
        }
        return false;
    }

    public void RemoveAt(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > collectibleSlots.Length - 1) return;

        //collectibleSlots[slotIndex].Collectible = null;
        collectibleSlots[slotIndex].CollectibleName = null;
        collectibleSlots[slotIndex].quantity = 0;


        OnCollectibleUpdated.Invoke();
    }

    public void RemoveCollectible(CollectibleSlot collectibleSlot)
    {
        for (int i = 0; i < collectibleSlots.Length; i++)
        {
            if (collectibleSlots[i].Collectible != null)
            {
                if (collectibleSlots[i].Collectible == collectibleSlot.Collectible)
                {
                    if (collectibleSlots[i].quantity < collectibleSlot.quantity)
                    {
                        collectibleSlot.quantity -= collectibleSlots[i].quantity;

						//collectibleSlots[i].Collectible = null;
						collectibleSlots[i].CollectibleName = null;
						collectibleSlots[i].quantity = 0;
					}
                    else
                    {
                        collectibleSlots[i].quantity -= collectibleSlot.quantity;

                        if (collectibleSlots[i].quantity == 0)
                        {
							//collectibleSlots[i].Collectible = null;
							collectibleSlots[i].CollectibleName = null;

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

        if(firstSlot.Collectible == secondSlot.Collectible) // Check if same collectible, stack
		{
            if(firstSlot.quantity <= secondSlot.GetRemainingSpace()) // Enough space to stack
            {
                collectibleSlots[indexTwo].quantity += firstSlot.quantity;

                //collectibleSlots[indexOne].Collectible = null;
                collectibleSlots[indexOne].CollectibleName = null;
                collectibleSlots[indexOne].quantity = 0;
            }
            else // not enough space to stack, but should move as much as possible
            {
                int amountFilled = secondSlot.Collectible.MaxStack - secondSlot.quantity;
                collectibleSlots[indexTwo].quantity = secondSlot.Collectible.MaxStack;

                collectibleSlots[indexOne].quantity -= amountFilled;
            }

        }
        else
        {
            //if (firstSlot.collectible as LootData && !secondSlot.allowLoot) return;
            //if (firstSlot.collectible as HatData && !secondSlot.allowHats) return;
            //if (firstSlot.collectible as ItemData && !secondSlot.allowItems) return;
            if (secondSlot.Collectible as LootData && !firstSlot.allowLoot) return;
            if (secondSlot.Collectible as HatData && !firstSlot.allowHats) return;
            if (secondSlot.Collectible as ItemData && !firstSlot.allowItems) return;

            //collectibleSlots[indexOne].Collectible = secondSlot.Collectible;
            collectibleSlots[indexOne].CollectibleName = secondSlot.CollectibleName;
            collectibleSlots[indexOne].quantity = secondSlot.quantity;

            //collectibleSlots[indexTwo].Collectible = firstSlot.Collectible;
            collectibleSlots[indexTwo].CollectibleName = firstSlot.CollectibleName;
            collectibleSlots[indexTwo].quantity = firstSlot.quantity;
        }
    

        OnCollectibleUpdated.Invoke();
        OnCollectibleSwapped.Invoke();
    }
}