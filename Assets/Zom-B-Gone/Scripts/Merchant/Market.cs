using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class Market : MonoBehaviour
{
    [SerializeField] MarketData marketData;
    [SerializeField] GameObject buyOptionPrefab;
    [SerializeField] GameObject sellOptionPrefab;
    [SerializeField] GameObject merchantInterestPrefab;
    private MerchantBuyOptionRefs buyOptionRefs;

    [Header("UI Refs")]
    [SerializeField] Image merchantImage;
    [SerializeField] TMP_Text merchantNameText;
    [SerializeField] SuperTextMesh merchantDialogue;
    [SerializeField] RectTransform sellingItemHolder;
    [SerializeField] TMP_Text goldCount;
    [SerializeField] RectTransform scrollingBackground;
    [SerializeField] float backgroundIncrementPerItem;

    [Header("Sell UI Refs")]
    [SerializeField] RectTransform sellScrollingBackground;
    [SerializeField] RectTransform buyingItemHolder;

    [SerializeField] RectTransform interestItemHolder;


    [HideInInspector] public MerchantData loadedMerchant;

	public void LoadMerchant(string name)
    {
        foreach (var merchant in marketData.merchants)
        {
            if (merchant.name == name)
            {
                loadedMerchant = merchant;
            }
        }

        merchantImage.sprite = loadedMerchant.merchantSprite;
        merchantNameText.text = loadedMerchant.merchantName;
        merchantNameText.color = loadedMerchant.nameColor;
        UpdatePlayerGoldText();

        MerchantSaySomething();

	}

    public void UpdatePlayerGoldText()
    {
		goldCount.text = SaveManager.currentSave.gold.ToString();
	}

    public void MerchantSaySomething()
    {
        int dialogueIndex = UnityEngine.Random.Range(0, loadedMerchant.dialogueOptions.Length);
        merchantDialogue.text = "<readDelay="+loadedMerchant.talkSpeed+">" + loadedMerchant.dialogueOptions[dialogueIndex];
        //merchantDialogue.Read();
    }

    public void OnOpenInterest()
    {
        for (int i = interestItemHolder.childCount - 1; i >= 0; i--) Destroy(interestItemHolder.GetChild(i).gameObject);
        for (int i = buyingItemHolder.childCount - 1; i >= 0; i--) Destroy(buyingItemHolder.GetChild(i).gameObject);
        sellScrollingBackground.sizeDelta = new Vector2(sellScrollingBackground.sizeDelta.x, 1080);

        foreach (CollectibleData collectible in loadedMerchant.vals.buyOffers.Keys)
        {
            GameObject interest = Instantiate(merchantInterestPrefab, interestItemHolder);
            MerchantInterestRefs refs = interest.GetComponent<MerchantInterestRefs>();
            refs.offeredPrice.text = loadedMerchant.vals.buyOffers[collectible].ToString();
            refs.hoverableCollectible.CollectibleData = collectible;
        }
    }

    public void OnOpenBuy()
    {
        for (int i = sellingItemHolder.childCount - 1; i >= 0; i--) Destroy(sellingItemHolder.GetChild(i).gameObject);
        scrollingBackground.sizeDelta = new Vector2(scrollingBackground.sizeDelta.x, 0);

        foreach (CollectibleData collectible in loadedMerchant.vals.inventory.Keys)
        {
            scrollingBackground.sizeDelta = new Vector2(scrollingBackground.sizeDelta.x, scrollingBackground.sizeDelta.y + backgroundIncrementPerItem);
            GameObject option = Instantiate(buyOptionPrefab, sellingItemHolder);
            MerchantBuyOptionRefs refs = option.GetComponent<MerchantBuyOptionRefs>();
            refs.loadedMerchant = loadedMerchant;

            refs.hoverableCollectible.CollectibleData = collectible;

			refs.unitPrice.text = loadedMerchant.vals.prices[collectible].ToString();

            refs.buyTotal.text = refs.unitPrice.text; // starts at one selected

            int maxAmt = loadedMerchant.vals.inventory[collectible];

			refs.maxAmount.text = maxAmt.ToString();
            if (maxAmt == 1) refs.plusButton.interactable = false;

            refs.minusButton.interactable = false;

        }
    }

    public void OnOpenSell()
    {
        for (int i = interestItemHolder.childCount - 1; i >= 0; i--) Destroy(interestItemHolder.GetChild(i).gameObject);
        for (int i = buyingItemHolder.childCount - 1; i >= 0; i--) Destroy(buyingItemHolder.GetChild(i).gameObject);
        sellScrollingBackground.sizeDelta = new Vector2(sellScrollingBackground.sizeDelta.x, 0);


        foreach (CollectibleData collectible in loadedMerchant.vals.buyOffers.Keys)
        {
            int maxAmt = 0;

            if(collectible is LootData)
            {
                foreach (var slot in marketData.backpackData.container.collectibleSlots)
                {
                    if(slot.Collectible == collectible)
                    {
                        maxAmt += slot.quantity;
                    }
                }
                foreach (var slot in marketData.lootLockerData.container.collectibleSlots)
                {
                    if (slot.Collectible == collectible)
                    {
                        maxAmt += slot.quantity;
                    }
                }

            }
            else if(collectible is ItemData)
            {
                foreach (var slot in marketData.itemLockerData.container.collectibleSlots)
                {
                    if (slot.Collectible == collectible)
                    {
                        maxAmt += slot.quantity;
                    }
                }
            }
            else if (collectible is HatData)
            {
                foreach (var slot in marketData.hatLockerData.container.collectibleSlots)
                {
                    if (slot.Collectible == collectible)
                    {
                        maxAmt += slot.quantity;
                    }
                }
            }

            if(maxAmt > 0)
            {
                sellScrollingBackground.sizeDelta = new Vector2(sellScrollingBackground.sizeDelta.x, sellScrollingBackground.sizeDelta.y + backgroundIncrementPerItem);
                GameObject option = Instantiate(sellOptionPrefab, buyingItemHolder);
                MerchantBuyOptionRefs refs = option.GetComponent<MerchantBuyOptionRefs>();
                refs.loadedMerchant = loadedMerchant;

                refs.hoverableCollectible.CollectibleData = collectible;

                refs.unitPrice.text = loadedMerchant.vals.buyOffers[collectible].ToString();

                refs.buyTotal.text = refs.unitPrice.text; // starts at one selected

                refs.maxAmount.text = maxAmt.ToString();
                if (maxAmt == 1) refs.plusButton.interactable = false;

                refs.minusButton.interactable = false;
            }


        }
    }
}
