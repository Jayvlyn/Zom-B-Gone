
using System;

[Serializable]
public struct LootSlot
{
    public Loot loot;
    public int quantity;

    public LootSlot(Loot loot, int quantity)
    {
        this.loot = loot;
        this.quantity = quantity;
    }

    public int GetRemainingSpace()
    {
        return loot.MaxStack - quantity;
    }

    public static bool operator == (LootSlot a, LootSlot b) { return a.Equals(b); }
    public static bool operator != (LootSlot a, LootSlot b) { return !a.Equals(b); }
}
