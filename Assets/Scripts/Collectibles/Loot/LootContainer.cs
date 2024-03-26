using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootContainer : ILootContainer
{
    private LootSlot[] lootSlots = new LootSlot[0];

    public Action OnLootUpdated = delegate { };

    public LootContainer(int size) => lootSlots = new LootSlot[size];

    public LootSlot GetSlotByIndex(int index) => lootSlots[index];

    public LootSlot AddLoot(LootSlot lootSlot)
    {
        for (int i = 0; i < lootSlots.Length; i++)
        {
            if (lootSlots[i] != null)
            {
                if (lootSlots[i].loot == lootSlot.loot)
                {
                    int remainingSpace = lootSlots[i].GetRemainingSpace();
                    if (lootSlot.quantity <= remainingSpace)
                    {
                        lootSlots[i].quantity += lootSlot.quantity;
                        lootSlot.quantity = 0;
                        OnLootUpdated.Invoke();
                        return lootSlot;
                    }
                    else if(remainingSpace > 0)
                    {
                        lootSlots[i].quantity += remainingSpace;

                        lootSlot.quantity -= remainingSpace;
                    }
                }
            }
        }

        for (int i = 0; i < lootSlots.Length; i++)
        {
            if (lootSlots[i].loot == null)
            {
                if(lootSlot.quantity <= lootSlot.loot.MaxStack)
                {
                    lootSlots[i] = lootSlot;

                    lootSlot.quantity = 0;

                    OnLootUpdated.Invoke();

                    return lootSlot;
                }
                else
                {
                    lootSlots[i] = new LootSlot(lootSlot.loot, lootSlot.loot.MaxStack);

                    lootSlot.quantity -= lootSlot.loot.MaxStack;
                }
            }
        }

        OnLootUpdated.Invoke();

        return lootSlot;
    }

    public int GetTotalQuantity(Loot loot)
    {
        int totalCount = 0;

        foreach(var slot in lootSlots)
        {
            if(slot.loot == null || slot.loot != loot) continue;

            totalCount += slot.quantity;
        }

        return totalCount;
    }

    public bool HasLoot(Loot loot)
    {
        foreach(var slot in lootSlots)
        {
            if (slot.loot == null || slot.loot != loot) continue;
            return true;
        }
        return false;
    }

    public void RemoveAt(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > lootSlots.Length - 1) return;

        lootSlots[slotIndex] = new LootSlot();

        OnLootUpdated.Invoke();
    }

    public void RemoveLoot(LootSlot lootSlot)
    {
        for (int i = 0; i < lootSlots.Length; i++)
        {
            if (lootSlots[i].loot != null)
            {
                if (lootSlots[i].loot == lootSlot.loot)
                {
                    if (lootSlots[i].quantity < lootSlot.quantity)
                    {
                        lootSlot.quantity -= lootSlots[i].quantity;

                        lootSlots[i] = new LootSlot();
                    }
                    else
                    {
                        lootSlots[i].quantity -= lootSlot.quantity;

                        if (lootSlots[i].quantity == 0)
                        {
                            lootSlots[i] = new LootSlot();

                            OnLootUpdated.Invoke();

                            return;
                        }
                    }
                }
            }
        }
    }

    public void Swap(int indexOne, int indexTwo)
    {
        LootSlot firstSlot = lootSlots[indexOne];
        LootSlot secondSlot = lootSlots[indexTwo];

        if (firstSlot == secondSlot) return;

        if(secondSlot.loot != null) // Check if same item, stack
        {
            if(firstSlot.loot == secondSlot.loot)
            {
                if(firstSlot.quantity <= secondSlot.GetRemainingSpace())
                {
                    secondSlot.quantity += firstSlot.quantity;

                    lootSlots[indexOne] = new LootSlot();

                    OnLootUpdated.Invoke();

                    return;
                }
            }
        }

        lootSlots[indexOne] = secondSlot;
        lootSlots[indexTwo] = firstSlot;

        OnLootUpdated.Invoke();
    }
}