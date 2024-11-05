using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectibleContainer
{
    CollectibleSlot AddCollectible(ref CollectibleSlot collectibleSlot);

    void RemoveCollectible(CollectibleSlot collectibleSlot);
    void RemoveAt(int slotIndex);
    void Swap(int indexOne, int indexTwo);
    bool HasCollectible(CollectibleData collectible);
    int GetTotalQuantity(CollectibleData collectible);
}
