using CodeMonkey;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Security.AccessControl;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Market", menuName = "New Market")]
public class MarketData : ScriptableObject
{
    public int cycleCount = 0;
    public int TotalDays => (cycleCount * daysPerCycle) + day;
    public int daysPerCycle = 4;
    [SerializeField] private int day = 1;
    public int Day
    {
        get { return day; }
        set
        {
            if (value < 1 || value > daysPerCycle + 1) return;
            if (value == daysPerCycle + 1)
            {
                cycleCount++;
                day = 1;
                foreach (var merchant in merchants)
                {
                    RefreshDealingCollectibles(merchant);
                    RefreshMerchantInventory(merchant);
                    RefreshBuyOffers(merchant);
                }
            }
            else day = value;
        }
    }

    public Color OneDayLeftColor = Color.red;
    public Color TwoDaysLeftColor = Color.magenta;
    public Color ThreeDaysLeftColor = Color.yellow;

    public MerchantData[] merchants = new MerchantData[3];

    public CollectibleContainerData backpackData;
    public CollectibleContainerData lootLockerData;
    public CollectibleContainerData hatLockerData;
    public CollectibleContainerData itemLockerData;

    private List<CollectibleData> dealingCollectibles = new List<CollectibleData>();

    public void RefreshDealingCollectibles(MerchantData merchant)
    {
        dealingCollectibles.Clear();
        dealingCollectibles.AddRange(merchant.lootTable.table);
    }

    public void RefreshMerchantInventory(MerchantData merchant)
    {
        merchant.vals.inventory.Clear();
        merchant.vals.prices.Clear();

		float repModifier = merchant.vals.reputationLevel;

		int baseCount = 3 + (int)repModifier;

		if (repModifier <= 0) repModifier = 1;
		else repModifier = Mathf.CeilToInt(Mathf.Sqrt(repModifier));

		int count = Random.Range(baseCount, Mathf.RoundToInt((baseCount) + (2 * repModifier)));

		for (int i = 0; i < count; i++)
        {
            if (dealingCollectibles.Count <= 1) return;

            Rarity chosenRarity = merchant.lootTable.GetRandomRarity(0);

            int attempts = 0;
            while(!LootTable.HasRarity(chosenRarity, dealingCollectibles))
            {
				chosenRarity = merchant.lootTable.GetRandomRarity(0);
                if (attempts > 10) return;
                attempts++;
			}


			CollectibleData chosenCollectible = dealingCollectibles[Random.Range(0, dealingCollectibles.Count)];
            while (chosenCollectible.rarity != chosenRarity)
            {
				chosenCollectible = dealingCollectibles[Random.Range(0, dealingCollectibles.Count)];
			}

			int maxAmount;
            if (chosenCollectible is ItemData) maxAmount = 4;
            else if (chosenCollectible is HatData) maxAmount = 2;
            else maxAmount = 10; // Loot data

            int chosenAmount = Random.Range(1, maxAmount + 1);

            merchant.vals.inventory.Add(chosenCollectible.Name, chosenAmount);
            dealingCollectibles.Remove(chosenCollectible); // no duplicate keys

            int price = DeterminePrice(chosenCollectible);
			price = DiscountPriceWithRep(price, merchant);

			merchant.vals.prices.Add(chosenCollectible.Name, price);

        }
    }

    public void RefreshBuyOffers(MerchantData merchant)
    {
        merchant.vals.buyOffers.Clear();

        float repModifier = merchant.vals.reputationLevel;

		int baseCount = 4 + (int)repModifier;

        if (repModifier <= 0) repModifier = 1;
        else repModifier = Mathf.CeilToInt(Mathf.Sqrt(repModifier));

        int count = Random.Range(baseCount, Mathf.RoundToInt((baseCount) + (4*repModifier)));

        for (int i = 0; i < count; i++)
        {
            if (dealingCollectibles.Count <= 1) break;
            CollectibleData chosenCollectible = dealingCollectibles[Random.Range(0, dealingCollectibles.Count)];

            dealingCollectibles.Remove(chosenCollectible);

            int price = DeterminePrice(chosenCollectible);
            price = IncreaseOfferWithRep(price, merchant);

            merchant.vals.buyOffers.Add(chosenCollectible.Name, price);
        }
    }

    public int DeterminePrice(CollectibleData c)
    {
        // Determine price
        int price = 10;
        if(c is ItemData)
        {
            price += 500;
        }
        else if (c is HatData)
        {
            price += 100;
        }
        float preRandomMult = Random.Range(0.8f, 1.5f);
        price = Mathf.RoundToInt(price * preRandomMult);
             if (c.rarity.Name == "Common")          price = Mathf.RoundToInt(price * 0.3f);
        else if (c.rarity.Name == "Valuable")        price = Mathf.RoundToInt(price * 0.6f);
        //else if (c.rarity.Name == "Very Valuable")   price = Mathf.RoundToInt(price * 1.3f);
        else if (c.rarity.Name == "Super Valuable")  price = Mathf.RoundToInt(price * 1.6f);
        else if (c.rarity.Name == "Super Legendary") price = Mathf.RoundToInt(price * 2.5f);

        float postRandomMult = Random.Range(0.9f, 1.1f);
        price = Mathf.RoundToInt(price * postRandomMult);

        int randomAdd = Random.Range(-(price / 5), price / 5);
        price += randomAdd;

        return price;
    }

    public int DiscountPriceWithRep(int price, MerchantData merchant)
    {
        int newPrice = price;
        float mod = 1 + Mathf.Sqrt(merchant.vals.reputationLevel);
        newPrice = Mathf.RoundToInt(price / mod);
        return newPrice;
    }

	public int IncreaseOfferWithRep(int price, MerchantData merchant)
	{
		int newPrice = price;
		float mod = 1 + Mathf.Sqrt(merchant.vals.reputationLevel);
		newPrice = Mathf.RoundToInt(price * mod);
		return newPrice;
	}

}
