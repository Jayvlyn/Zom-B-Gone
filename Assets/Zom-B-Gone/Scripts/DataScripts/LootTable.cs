using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Table", menuName = "New Loot Table")]
public class LootTable : ScriptableObject
{
    public CollectibleData[] table = new CollectibleData[5];
    public RarityList rarityList;
	private List<float> weights;
	private List<CollectibleData>[] splitTable;

    public CollectibleData GetRandomCollectible()
    {
		CollectibleData chosenCollectible = splitTable[0][0];

		if (splitTable == null) SplitRarityList();

		Rarity chosenRarity = GetRandomRarity();

		for (int i = 0; i < rarityList.rarities.Length; i++)
		{
			if(chosenRarity == rarityList.rarities[i])
			{
				chosenCollectible = splitTable[i][Random.Range(0, splitTable[i].Count)];
			}
		}

		return chosenCollectible;
    }

	private void SplitRarityList()
	{
		splitTable = new List<CollectibleData>[rarityList.rarities.Length];

		foreach (CollectibleData c in table)
		{
			for (int i = 0; i < rarityList.rarities.Length; i++)
			{
				if(c.rarity == rarityList.rarities[i])
				{
					splitTable[i].Add(c);
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
		if (weights == null) weights = GenerateRarityWeights();

		for (int i = 0; i < weights.Count; i++)
		{
			if (roll <= weights[i])
			{
				return rarityList.rarities[i];
			}
		}
		return rarityList.rarities[0];
	}


	public Rarity GetRandomRarity(int level)
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
