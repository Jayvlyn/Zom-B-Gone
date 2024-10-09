using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MerchantVals
{
    [HideInInspector] public int reputationLevel;
    [HideInInspector] public int reputationExp;
    [HideInInspector] public CollectibleData[] buyOffers;
    public Dictionary<CollectibleData, int> inventory = new Dictionary<CollectibleData, int>(); // collectible and amount
    public Dictionary<CollectibleData, int> prices = new Dictionary<CollectibleData, int>(); // collectible and price
}
