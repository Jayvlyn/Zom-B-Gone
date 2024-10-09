using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Market", menuName = "New Market")]
public class MarketData : ScriptableObject
{
    private int day = 1;
    public int Day
    {
        get { return day; }
        set
        {
            if (value < 1 || value > 4) return;
            if (value == 4) day = 1;
            else day = value;

            foreach (var merchant in merchants)
            {
                RefreshMerchantInventory(merchant);
                RefreshBuyOffers(merchant);
            }
        }
    }

    public MerchantData[] merchants = new MerchantData[3];
    [SerializeField] CollectibleList hatList;
    [SerializeField] CollectibleList itemList;
    [SerializeField] CollectibleList lootList;

    public void RefreshMerchantInventory(MerchantData merchant)
    {
        merchant.vals.inventory.Clear();
        merchant.vals.prices.Clear();

        int count = Random.Range(6, 9);
        List<CollectibleData> possibleCollectibles = new List<CollectibleData>();

        if (merchant.dealsHats) possibleCollectibles.AddRange(hatList.collectibles);
        if (merchant.dealsItems) possibleCollectibles.AddRange(itemList.collectibles);
        if (merchant.dealsLoot) possibleCollectibles.AddRange(lootList.collectibles);

        for (int i = 0; i < count; i++)
        {
            CollectibleData chosenCollectible = possibleCollectibles[Random.Range(0, possibleCollectibles.Count)];

            int maxAmount;
            if (chosenCollectible is ItemData) maxAmount = 3;
            else if (chosenCollectible is HatData) maxAmount = 1;
            else maxAmount = 10; // Loot data

            int chosenAmount = Random.Range(1, maxAmount + 1);

            merchant.vals.inventory.Add(chosenCollectible, chosenAmount);
            possibleCollectibles.Remove(chosenCollectible); // no duplicate keys

            int price = DeterminePrice(chosenCollectible);

            merchant.vals.prices.Add(chosenCollectible, price);
        }
    }

    public void RefreshBuyOffers(MerchantData merchant)
    {
        int count = Random.Range(4, 9);
        List<CollectibleData> possibleCollectibles = new List<CollectibleData>();

        if (merchant.dealsHats) possibleCollectibles.AddRange(hatList.collectibles);
        if (merchant.dealsItems) possibleCollectibles.AddRange(itemList.collectibles);
        if (merchant.dealsLoot) possibleCollectibles.AddRange(lootList.collectibles);

        CollectibleData[] newBuyOffers = new CollectibleData[count];

        for (int i = 0; i < count; i++)
        {
            CollectibleData chosenCollectible = possibleCollectibles[Random.Range(0, possibleCollectibles.Count)];

            newBuyOffers[i] = chosenCollectible;

            possibleCollectibles.Remove(chosenCollectible);
        }

        merchant.vals.buyOffers = newBuyOffers;
    }

    public int DeterminePrice(CollectibleData c)
    {
        // Determine price
        int price = 10;
        float preRandomMult = Random.Range(0.8f, 1.5f);
        price = Mathf.RoundToInt(price * preRandomMult);
             if (c.rarity.Name == "Common")          price = Mathf.RoundToInt(price * 0.8f);
        else if (c.rarity.Name == "Valuable")        price = Mathf.RoundToInt(price * 1.1f);
        else if (c.rarity.Name == "Very Valuable")   price = Mathf.RoundToInt(price * 1.3f);
        else if (c.rarity.Name == "Super Valuable")  price = Mathf.RoundToInt(price * 1.6f);
        else if (c.rarity.Name == "Super Legendary") price = Mathf.RoundToInt(price * 2f);

        float postRandomMult = Random.Range(0.9f, 1.1f);
        price = Mathf.RoundToInt(price * postRandomMult);

        int randomAdd = Random.Range(-(price / 5), price / 5);
        price += randomAdd;

        return price;


    }

}
