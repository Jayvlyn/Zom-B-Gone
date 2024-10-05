using OdinSerializer;
using OdinSerializer.Utilities;
using System;
using UnityEngine;

[Serializable]
public struct CollectibleSlot
{
    [SerializeField] private CollectibleData collectible;
	public CollectibleData Collectible
    {
        get {
            if (collectible == null && !string.IsNullOrEmpty(collectibleName))
            {
                collectible = Resources.Load<CollectibleData>(collectibleName);
            }
            return collectible; 
        }
    }

    [HideInInspector, SerializeField] private string collectibleName;
    public string CollectibleName
    {
        get {
            if (string.IsNullOrEmpty(collectibleName) && collectible != null) 
                collectibleName = collectible.name;

            return collectibleName;
        }
        set
        {
            collectibleName = value;

            if(string.IsNullOrEmpty(value)) collectible = null;
            else collectible = Resources.Load<CollectibleData>(collectibleName);
        }
    }


    public int quantity;

	public bool allowLoot;
	public bool allowItems;
	public bool allowHats;

	public CollectibleSlot(CollectibleData collectible, string collectibleName, int quantity, bool allowLoot = true, bool allowItems = true, bool allowHats = true)
    {
        this.collectible = collectible;
        this.collectibleName = collectibleName;
        this.quantity = quantity;
        this.allowLoot = allowLoot;
        this.allowItems = allowItems;
        this.allowHats = allowHats;
	}

    public int GetRemainingSpace()
    {
        return Collectible.MaxStack - quantity;
    }

    public static bool operator == (CollectibleSlot a, CollectibleSlot b) { return a.Equals(b); }
    public static bool operator != (CollectibleSlot a, CollectibleSlot b) { return !a.Equals(b); }
}
