using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILootContainer
{
    LootSlot AddLoot(LootSlot lootSlot);

    void RemoveLoot(LootSlot lootSlot);
    void RemoveAt(int slotIndex);
    void Swap(int indexOne, int indexTwo);
    bool HasLoot(Loot loot);
    int GetTotalQuantity(Loot loot);
}
