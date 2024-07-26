using System;

[Serializable]
public struct CollectibleSlot
{
    public CollectibleData collectible;
    public int quantity;

    public CollectibleSlot(CollectibleData collectible, int quantity)
    {
        this.collectible = collectible;
        this.quantity = quantity;
    }

    public int GetRemainingSpace()
    {
        return collectible.MaxStack - quantity;
    }

    public static bool operator == (CollectibleSlot a, CollectibleSlot b) { return a.Equals(b); }
    public static bool operator != (CollectibleSlot a, CollectibleSlot b) { return !a.Equals(b); }
}
