using OdinSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MerchantVals
{
    [HideInInspector] public int reputationLevel;
    [HideInInspector] public int reputationExp;
    [OdinSerialize] public Dictionary<CollectibleData, int> buyOffers = new Dictionary<CollectibleData, int>(); // collectible and price
	[OdinSerialize] public Dictionary<CollectibleData, int> inventory = new Dictionary<CollectibleData, int>(); // collectible and amount
	[OdinSerialize] public Dictionary<CollectibleData, int> prices = new Dictionary<CollectibleData, int>(); // collectible and price

    public int GetNextLevelRequirement()
    {
        int baseExp = 100;
        float exponent = 1.5f;

        return Mathf.RoundToInt(baseExp * Mathf.Pow(reputationLevel, exponent));
    }

    public void GainExp(int amount)
    {
        reputationExp += amount;

        while(reputationExp >= GetNextLevelRequirement())
        {
            reputationExp -= GetNextLevelRequirement();
            reputationLevel++;
        }
    }
}
