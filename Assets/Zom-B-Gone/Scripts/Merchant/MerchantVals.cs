using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MerchantVals
{
    [HideInInspector] public int reputationLevel;
    [HideInInspector] public int reputationExp;
    public Dictionary<CollectibleData, int> buyOffers = new Dictionary<CollectibleData, int>(); // collectible and price
    public Dictionary<CollectibleData, int> inventory = new Dictionary<CollectibleData, int>(); // collectible and amount
    public Dictionary<CollectibleData, int> prices = new Dictionary<CollectibleData, int>(); // collectible and price
}
