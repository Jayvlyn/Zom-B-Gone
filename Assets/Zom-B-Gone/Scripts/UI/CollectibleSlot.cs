using System;

[Serializable]
public struct CollectibleSlot
{
    public CollectibleData collectible;
    public int quantity;

	public bool allowLoot;
	public bool allowItems;
	public bool allowHats;

	public CollectibleSlot(CollectibleData collectible, int quantity, bool allowLoot = true, bool allowItems = true, bool allowHats = true)
    {
        this.collectible = collectible;
        this.quantity = quantity;
        this.allowLoot = allowLoot;
        this.allowItems = allowItems;
        this.allowHats = allowHats;
    }

    public int GetRemainingSpace()
    {
        return collectible.MaxStack - quantity;
    }

    public static bool operator == (CollectibleSlot a, CollectibleSlot b) { return a.Equals(b); }
    public static bool operator != (CollectibleSlot a, CollectibleSlot b) { return !a.Equals(b); }
}
