using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Table", menuName = "New Loot Table")]
public class LootTable : ScriptableObject
{
    public CollectibleData[] table = new CollectibleData[5];
    public RarityList rarityList;
	private List<float> weights;
	private List<CollectibleData>[][] splitTable; // 2d array of collectibles

    //[0][x] Hats of rarity x
    //[1][x] Items of rarity x
    //[2][x] Loot of rarity x

    /* Ex:
	 *  
	 * [List of Common Hats]	[List of Valuable Hats]		[List of Very Valuable Hats]	[List of Super Valuable Hats]	[List of Super Legendary Hats]
	 * [List of Common Items]	[List of Valuable Items]	[List of Very Valuable Items]	[List of Super Valuable Items]	[List of Super Legendary Items]
	 * [List of Common Loot]	[List of Valuable Loot]		[List of Very Valuable Loot]	[List of Super Valuable Loot]	[List of Super Legendary Loot]
	 */

	public HatData GetRandomHat()
	{
        if (splitTable == null) SplitRarityList();

		HatData chosenHat = new HatData();
        Rarity chosenRarity = GetRandomRarity();

        for (int i = 0; i < rarityList.rarities.Length; i++)
        {
            if (chosenRarity == rarityList.rarities[i])
            {
				int j = 0; // hat

                // random collectible of collectible type j, of rarity i
                int originali = i;
                while (splitTable[j][i].Count <= 0) // if no collectibles for this type and rarity, lower rarity by one
                {
                    i--;
                    if (i < 0) // no collectibles of type at all;
                    {
						return null;
                    }
                }
                chosenHat = splitTable[j][i][Random.Range(0, splitTable[j][i].Count)] as HatData;
                break;
            }
        }

        if (!chosenHat.name.Equals("Unnamed")) return chosenHat;
        else return null;
    }

    public CollectibleData GetRandomCollectible()
    {
		if (splitTable == null) SplitRarityList();

		CollectibleData chosenCollectible = new LootData();

		Rarity chosenRarity = GetRandomRarity();

		for (int i = 0; i < rarityList.rarities.Length; i++)
		{
			if(chosenRarity == rarityList.rarities[i])
			{
				// i = rarity index
				// j = collectible type index
				int j = -1;

				int collTypeRoll = Random.Range(1, 101);
				if(collTypeRoll < 70) // 70 percent will be loot
				{
					j = 2;
				}
				else if(collTypeRoll < 90) // 20 percent will be items
				{
					j = 1;
				}
				else // 10 percent will be hats
				{
					j = 0;
				}

				// random collectible of collectible type j, of rarity i
				int originali = i;
				while (splitTable[j][i].Count <= 0) // if no collectibles for this type and rarity, lower rarity by one
				{
					i--;
                    if (i < 0) // no collectibles of type at all, change collectible type
					{
						i = originali;
						j--;
						
						if (j < 0) // cant find nothin, give up
                        {
							return null;
						}
					}
				}
				chosenCollectible = splitTable[j][i][Random.Range(0, splitTable[j][i].Count)];
				break;
			}
		}

		if (!chosenCollectible.name.Equals("Unnamed")) return chosenCollectible;
		else return null;
    }

	public int GetRandomQuantity(CollectibleData collectible)
	{
		float randomValue = Random.Range(0f, 1f);
		int quantity = Mathf.CeilToInt(Mathf.Pow(randomValue, 5) * collectible.MaxStack);
		return Mathf.Clamp(quantity, 1, collectible.MaxStack);
	}

	private void SplitRarityList()
	{
		splitTable = new List<CollectibleData>[3][];
		for (int i = 0; i < splitTable.Length; i++) // loops 3 times, making a set of lists, one list for each rarity
		{
			splitTable[i] = new List<CollectibleData>[rarityList.rarities.Length];
			for(int k = 0; k < rarityList.rarities.Length; k++) // initialize each rarity list
			{
				splitTable[i][k] = new List<CollectibleData>();
			}
		}
		 
		foreach (CollectibleData c in table) // find which list a collectible goes into in the 2D array
		{
			int rowIndex = -1;
			if(c is HatData)		rowIndex = 0;
            else if (c is ItemData) rowIndex = 1;
            else if (c is LootData) rowIndex = 2;
            

			if (rowIndex >= 0)
			{
                for (int i = 0; i < rarityList.rarities.Length; i++)
                {
                    if (c.rarity == rarityList.rarities[i])
                    {
                        splitTable[rowIndex][i].Add(c);
                    }
                }
            }
			
		}
	}

	private List<float> GenerateRarityWeights(float decayFactor = 0.5f)
	{
		List<float> weights = new List<float>();
		float totalWeight = 0f;

		// Calculate the total weight
		for (int i = 0; i < rarityList.rarities.Length; i++)
		{
			totalWeight += Mathf.Pow(decayFactor, i);
		}

		// Normalize the weights so they sum to 100
		float cumulativeWeight = 0f;
		for (int i = 0; i < rarityList.rarities.Length; i++)
		{
			float weight = (Mathf.Pow(decayFactor, i) / totalWeight) * 100f;
			cumulativeWeight += weight;
			
			weights.Add(cumulativeWeight);
		}
		return weights;
	}

	public Rarity GetRandomRarity()
	{
		float roll = Random.Range(0f, 100f);
		if (weights == null || weights.Count <= 0) weights = GenerateRarityWeights();
		for (int i = 0; i < weights.Count; i++)
		{
			if (roll <= weights[i])
			{
				return rarityList.rarities[i];
			}
		}
		return rarityList.rarities[0];
	}


	public Rarity GetRandomRarity(int level) // level not implemented yet
	{
		float roll = Random.Range(0f, 100f);
		weights = GenerateRarityWeights();

		for (int i = 0; i < weights.Count; i++)
		{
			if (roll <= weights[i])
			{
				return rarityList.rarities[i];
			}
		}
		return rarityList.rarities[0];
	}

	public bool HasRarity(Rarity r)
	{
		foreach (CollectibleData c in table)
		{
			if(c.rarity == r) return true;
		}
		return false;
	}

	public static bool HasRarity(Rarity r, CollectibleData[] arr)
	{
		foreach (CollectibleData c in arr)
		{
			if (c.rarity == r) return true;
		}
		return false;
	}

	public static bool HasRarity(Rarity r, List<CollectibleData> arr)
	{
		foreach (CollectibleData c in arr)
		{
			if (c.rarity == r) return true;
		}
		return false;
	}

}
