using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Market", menuName = "New Market")]
public class MarketData : ScriptableObject
{
    private int day = 0;
    public int Day
    {
        get { return day; }
        set
        {
            day = value;
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
        int count = Random.Range(6, 11);
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

            int chosenAmount = Random.Range(0, maxAmount + 1);

            merchant.inventory.Add(chosenCollectible, chosenAmount);
            possibleCollectibles.Remove(chosenCollectible); // no duplicate keys
        }
    }

    public void RefreshBuyOffers(MerchantData merchant)
    {
        int count = Random.Range(4, 15);
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
    }

}
